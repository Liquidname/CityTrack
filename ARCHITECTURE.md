# Карта проекта CityTrack2

## Структура папок

Весь код проекта расположен в `Assets/_Project/Scripts/`. Папки `Assets/Scripts/Runtime/` нет.

```
Assets/_Project/Scripts/
├── Core/           — базовые интерфейсы и глобальная точка входа (Game, GameEntryPoint, IGameTick, IGameStart)
├── DI/             — RootScope (глобальный VContainer скоуп)
├── Level1/         — Level1Scope, Level1EntryPoint, DebugPanel
├── Map/            — генерация карты: MapGenerator, ChunkView, MapContextView, MapGeneratorView, пулы
├── Movement/       — MovementSystem, MovementSettings, MovementView
├── ObjectPools/    — ChunkPool, PlatformPool
├── Player/         — PlayerView (PlayerVIew.cs), PlayerSystem
├── StateMachine/   — GameStateMachine, IGameState, состояния (PrepareState, RunState, DefeatState)
```

## DI-контейнер (VContainer)

### RootScope (`Scripts/DI/RootScope.cs`)
- `Game` — Singleton
- `GameEntryPoint` — entry point (`IStartable`, `ITickable`)

### Level1Scope (`Scripts/Level1/Level1Scope.cs`)
Дочерний скоуп уровня. Регистрации:

- **Компоненты сцены** (RegisterComponent): `PlayerView`, `MovementView`, `MapContextView`, `MapGeneratorView`
- **Singleton-сервисы**: `MovementSystem` (с `WithParameter(movementSettings)`), `GameStateMachine`, `PlayerSystem`, `MapGenerator`, `ChunkPool`, `PlatformPool`
- **Состояния** (Transient): `PrepareState`, `RunState`, `DefeatState`
- **Точка входа**: `Level1EntryPoint` (`IStartable`, `ITickable`)

## Игровой цикл (Tick)

Единственная точка обновления сцены — `Level1EntryPoint.Tick()`:

```
1. MovementSystem.Tick(dt)    — расчёт скоростей, перемещение чанков и игрока, вращение модели
2. GameStateMachine.Tick(dt)  — делегирует текущему IGameState.Tick(dt)
3. MapGenerator.Tick(dt)      — генерация карты (пока пустой)
```

Независимых `Update()` в MonoBehaviour нет (кроме `DebugPanel` под `#if UNITY_EDITOR`).

## Модель движения (`MovementSystem`)

- Игрок зафиксирован по оси X в мировом пространстве.
- **Движение по X**: чанки карты смещаются навстречу игроку (`chunk.Translate(direction * VelocityX * dt)`).
- **Движение по Y**: игрок перемещается вертикально (`playerView.Translate(direction * VelocityY * dt)`).
- Скорости `VelocityX` / `VelocityY` — публичные свойства `MovementSystem` (get; private set), НЕ вынесены в отдельный класс.
- Два режима полёта (`FlightState`):
  - `DIVE` (пикирование) — зажат ЛКМ: гравитация + forceDive ускоряют вниз, diveBrake тормозит по X.
  - `GLIDE` (планирование) — ЛКМ отпущена: плавное снижение к glideDescentSpeed, разгон по X через glideAcceleration + lift.
- `VelocityX` зажимается через `Mathf.Clamp` к `[minSpeed, MaxHorizontalSpeed]` в `ComputeHorizontalVelocity`. В текущей конфигурации `MinHorizontalSpeed = 4 м/с`.
- Начальная скорость: `VelocityX = MovementSettings.HorizontalSpeed` (18 м/с).

### Настройки (`MovementSettings`)

Сериализуемый класс (не ScriptableObject), передаётся через `WithParameter` в `Level1Scope`. Ключевые поля:

| Параметр | Значение | Назначение |
|---|---|---|
| horizontalSpeed | 18 | Начальная скорость X |
| minHorizontalSpeed | 4 | Минимальная скорость X (clamp) |
| maxHorizontalSpeed | 35 | Максимальная скорость X |
| gravity | 20 | Ускорение свободного падения |
| forceDive | 15 | Дополнительное ускорение вниз при пикировании |
| glideDescentSpeed | 1 | Целевая скорость снижения при планировании |
| glideBrake | 8 | Торможение вертикальной скорости при планировании |
| glideAcceleration | 5 | Разгон вперёд при планировании |
| drag | 0.15 | Доля потери скорости X за секунду |
| glideLiftCoefficient | 0.35 | Конверсия скорости снижения в разгон |
| diveBrake | 5 | Торможение по X при пикировании |
| diveSpeedLossLimit | 0.6 | Доля скорости входа, ниже которой пикирование не снижает X |
| bounceYBoostMultiplier | 1.4 | Множитель отскока от платформы |
| reduceXonObstacleHit | 0.7 | Множитель потери X при ударе сверху об препятствие |
| minBounceSpeed | 4 | Минимальная скорость X для отскока от платформы (иначе поражение) |

## Физические взаимодействия

Используются 3D коллайдеры (Collider, не Collider2D). Обработка через `OnTriggerEnter` в `PlayerView`.

- **Платформы** (`Platform` tag): `PlayerView.HitThePlatform` → `MovementSystem.OnPlayerBounce`:
  - Если `VelocityX < minBounceSpeed`: фиксируется поражение (`Debug.Log("Loose")`), отскок не выполняется.
  - Иначе: `AddBounceForce(bounceYBoostMultiplier)` — отскок вверх пропорционально `VelocityX / 4`.
- **Препятствия** (`Obstacle` tag): `PlayerView.HitTheObstacle(normal)` → `MovementSystem.CalculateObstacleBounce`:
  - Удар сверху (`normal.y > 0.7`): `VelocityX *= reduceXonObstacleHit` + слабый отскок вверх.
  - Удар снизу / потолок (`normal.y < -0.7`): `VelocityX *= reduceXonObstacleHit` + отскок вниз.
  - Боковой удар (`|normal.x| > 0.7`): `VelocityX *= reduceXonObstacleHit * -1` (реверс с потерей скорости).

## Стейт-машина (`GameStateMachine`)

Реализует `IGameTick`. Резолвит состояния через `IObjectResolver.Resolve<T>()`.

- **PrepareState**: ожидание клика для старта → `Enter<RunState>()`. Игнорирует клик над UI (`EventSystem.IsPointerOverGameObject()`).
- **RunState**: `Enter()` → `MovementSystem.StartMoving()`, `Exit()` → `MovementSystem.StopMoving()`. Tick пустой.
- **DefeatState**: заглушка (только `Debug.Log("Defeat: Enter")`). На данный момент ничем не вызывается — нет триггера перехода.

Состояния зарегистрированы как `Transient` — создаются заново при каждом `Resolve`. Это аллокация при каждом переходе.

## Ключевые интерфейсы

- **`IGameTick`** (`Core/IGameTick.cs`): `void Tick(float deltaTime)` — единый интерфейс обновления.
- **`IGameState`** (`StateMachine/IGameState.cs`): `void Enter()` (default empty), `void Tick(float deltaTime)`, `void Exit()`.
- **`IGameStart`** (`Core/IGameStart.cs`): `void Start()` — ручная инициализация.

## View-слой

- **`PlayerView`** (`Player/PlayerVIew.cs`): MonoBehaviour. Хранит `direction` (Vector3.up), ссылку на `playerModel` (GameObject). Бросает события `HitThePlatform`, `HitTheObstacle` из `OnTriggerEnter`.
- **`MovementView`** (`Movement/MovementView.cs`): MonoBehaviour. Хранит `direction` для смещения чанков. Бросает `DestroyTriggered`/`SpawnTriggered` по тегу `DestroyTrigger`.
- **`PlayerSystem`** (`Player/PlayerSystem.cs`): устанавливает вращение `playerModel` через `SetPlayerModelRotation`.

## Известные особенности

- Имя файла `PlayerVIew.cs` содержит опечатку (заглавная I).
- `MovementSystem` подписывается на события `PlayerView` в `Start()` и отписывается в `Dispose()`.
- `MapGenerator.Tick()` на данный момент пустой.
- `Game.Tick()` на RootScope-уровне пустой.
- В `AddBounceForce` есть magic number (`VelocityX / 4`), отмеченный комментарием для выноса в настройки.
