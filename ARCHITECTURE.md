# Карта проекта CityTrack2

## Структура папок

Весь код проекта расположен в `Assets/_Project/Scripts/`.

```
Assets/_Project/Scripts/
├── Camera/         — следование камеры за игроком (CameraSystem, CameraView, CameraSettings)
├── Core/           — базовые интерфейсы и глобальная точка входа (Game, GameEntryPoint, IGameTick, IGameStart)
├── DI/             — RootScope (глобальный VContainer скоуп)
├── Graphics/
│   └── Parralax/   — фоновый параллакс-скроллинг (ParallaxSystem, ParallaxView)
├── Level1/         — скоуп и точка входа уровня (Level1Scope, Level1EntryPoint, DebugPanel)
├── Map/            — генерация карты, чанки и платформы (MapGenerator, ChunkView, PlatformView, PlatformConfig, MapContextView, MapGeneratorView)
├── Money/          — внутриигровая валюта (MoneySystem, MoneyView)
├── Movement/       — физика полёта, скорости, модификаторы (MovementSystem, MovementSettings, MovementStats, MovementView, MovementStatsLogger)
├── ObjectPools/    — пулы объектов (ObjectPool<T>, ChunkPool, PlatformPool)
├── Player/         — игрок, уровень полёта, коллизии (PlayerView, PlayerSystem, MapLayer)
├── PTS/            — очки, комбо-серии, множители (ScoreSystem, ScoreView)
├── StateMachine/   — стейт-машина игрового цикла (GameStateMachine, IGameState, PrepareState, RunState, DefeatState)
├── Upgrades/       — магазин улучшений между забегами (UpgradeSystem, UpgradeConfig, UpgradeDefiniton, UpgradeID, UpgradeView)
```

## DI-контейнер (VContainer)

### RootScope (`Scripts/DI/RootScope.cs`)
- `Game` — Singleton
- `GameEntryPoint` — entry point (`IStartable`, `ITickable`)

### Level1Scope (`Scripts/Level1/Level1Scope.cs`)
Дочерний скоуп уровня. Регистрации:

- **Компоненты сцены** (RegisterComponent): `PlayerView`, `MovementView`, `MapContextView`, `MapGeneratorView`, `CameraView`, `ParallaxView`, `ScoreView`, `MoneyView`, `UpgradeView`
- **Singleton-сервисы**:
  - `MovementSystem` (с `WithParameter(movementSettings)`)
  - `CameraSystem` (с `WithParameter(cameraSettings)`)
  - `GameStateMachine`
  - `PlayerSystem`
  - `MapGenerator`
  - `ChunkPool`, `PlatformPool`
  - `ParallaxSystem`
  - `ScoreSystem`
  - `MoneySystem`
  - `MovementStats`
  - `UpgradeSystem` (с `WithParameter(upgradeConfig)`)
  - `MovementStatsLogger` (под `#if UNITY_EDITOR`)
- **Состояния** (Singleton): `PrepareState`, `RunState`, `DefeatState`
- **Точка входа**: `Level1EntryPoint` (`IStartable`, `ITickable`, `IDisposable`)

## Инициализация уровня

`Level1EntryPoint.Start()`:
1. `GameStateMachine.Enter<PrepareState>()` — вход в состояние подготовки
2. Подписка `MovementSystem.Defeated` → `GameStateMachine.Enter<DefeatState>()`
3. Ручной вызов `Start()` у систем: `MovementSystem`, `MapGenerator`, `PlayerSystem`, `ScoreSystem` (и `MovementStatsLogger` в редакторе)

## Игровой цикл (Tick)

Единственная точка обновления сцены — `Level1EntryPoint.Tick()`:

```
1. MovementSystem.Tick(dt)      — режим полёта, расчёт скоростей, перемещение чанков/игрока, вращение модели, проверка поражения
2. GameStateMachine.Tick(dt)    — делегирует текущему IGameState.Tick(dt)
3. MapGenerator.Tick(dt)        — (пока пустой, подготовлен для процедурной генерации)
4. CameraSystem.Tick(dt)        — следование камеры за игроком с мёртвой зоной
5. ParallaxSystem.Tick(dt)      — смещение фоновых слоёв
6. ScoreSystem.Tick(dt)         — начисление очков за пройденное расстояние
7. MovementStatsLogger.Tick(dt) — сбор телеметрии полёта (под #if UNITY_EDITOR)
```

Независимых `Update()` в MonoBehaviour нет (кроме `DebugPanel` под `#if UNITY_EDITOR`).

## Модель движения (`MovementSystem`)

Реализует `IGameTick`, `IGameStart`, `IDisposable`.

- Игрок зафиксирован по оси X в мировом пространстве.
- **Движение по X**: чанки карты смещаются навстречу игроку.
- **Движение по Y**: игрок перемещается вертикально.
- Скорости `VelocityX` / `VelocityY` — публичные свойства (get; private set).
- Два режима полёта (`FlightState`):
  - `DIVE` — зажат ЛКМ: гравитация + forceDive ускоряют вниз, diveBrake тормозит по X. `_diveEntrySpeed` запоминает скорость входа в пикирование для ограничения потери через `diveSpeedLossLimit`.
  - `GLIDE` — ЛКМ отпущена: плавное снижение к `glideDescentSpeed`, разгон по X через `glideAcceleration + lift`.
- Событие `Defeated` — вызывается при недостаточной скорости для отскока или падении ниже `InstantLooseY`.
- **Защита от проваливания**: `CheckObstacleRoof` использует `Physics.SphereCastAll` вниз от игрока для обнаружения крыши препятствия при быстром падении.

### Модификаторы от апгрейдов (`MovementStats`)

DTO-класс, хранящий бонусы из системы улучшений. Используется `MovementSystem` для расчёта эффективных значений:
- `MaxSpeedBonus` → `EffectiveMaxHorizontalSpeed = MaxHorizontalSpeed + MaxSpeedBonus`
- `ArmorProgress` → `EffectiveReduceXonObstacleHit = Lerp(ReduceXonObstacleHit, MaxReduceXonObstacleHit, ArmorProgress)`

### Настройки (`MovementSettings`)

Сериализуемый класс, передаётся через `WithParameter` в `Level1Scope`. Ключевые группы:

| Группа | Параметры | Назначение |
|---|---|---|
| Скорости | `launchSpeed` (18), `minHorizontalSpeed` (4), `maxHorizontalSpeed` (35), `minVerticalSpeed` (-35), `maxVerticalSpeed` (25) | Границы скоростей |
| Ускорения | `gravity` (20), `forceDive` (15) | Силы при пикировании |
| Планирование | `glideDescentSpeed` (1), `glideBrake` (8), `glideAcceleration` (5), `drag` (0.15), `glideLiftCoefficient` (0.35) | Параметры режима GLIDE |
| Пикирование | `safeDiveAngle` (15°), `diveSpeedLossRate` (0.5), `diveBrake` (5), `diveSpeedLossLimit` (0.6) | Потеря скорости при DIVE |
| Отскок | `bounceYBoostMultiplier` (1.4), `bounceVelocityXDivisor` (4), `minBounceSpeed` (4) | Отскок от платформ |
| Препятствия | `reduceXonObstacleHit` (0.7), `maxReduceXonObstacleHit` (0.85), `minRoofBounceForce` (4) | Столкновение с препятствиями |
| Поражение | `instantLooseY` | Мировая Y-координата мгновенного проигрыша |

## Физические взаимодействия

Используются 3D коллайдеры (Collider, не Collider2D). Обработка через `OnTriggerEnter` в `PlayerView`.

### Столкновение с платформой (`Platform` tag)

`PlayerView.HitThePlatform(PlatformView)` → подписаны 3 системы:

1. **`MovementSystem.OnPlayerBounce`**:
   - `VelocityX < minBounceSpeed` → `Defeated` (поражение).
   - Иначе: `AddBounceForce(bounceYBoostMultiplier, platform.PlatformBoostMultiplier)`.
   - Формула отскока: `VelocityY += (VelocityX / bounceVelocityXDivisor) * platformMultiplier * settingsMultiplier`.
2. **`ScoreSystem.OnPlayerHitPlatform`**:
   - Быстрый удар (`VelocityX > MinimalVelocityXToExtraPTS`): мгновенные очки + `chain++`.
   - Медленный удар: снижение серии комбо на `ChainReduceBySlowPlatformHit`.
   - `BUILDING_SAVER`: бонусные очки через `OnPlayerSaveFromLowLayer()`.
3. **`MovementStatsLogger`** (редактор): логирование события.

### Столкновение с препятствием (`Obstacle` tag)

`PlayerView.HitTheObstacle(Vector2 normal)` → `MovementSystem.CalculateObstacleBounce`:

| Удар | Условие | Эффект |
|---|---|---|
| Сверху (крыша) | `normal.y > 0.7` | Слабый отскок вверх, `VelocityX *= EffectiveReduceXonObstacleHit` |
| Снизу (потолок) | `normal.y < -0.7` | `VelocityY = 0` + отскок вниз, `VelocityX *= EffectiveReduceXonObstacleHit` |
| Боковой (стена) | `|normal.x| > 0.7` | `VelocityX *= EffectiveReduceXonObstacleHit * -1` (реверс), `_diveEntrySpeed = 0` |

При `VelocityX < minBounceSpeed` после удара — поражение.

`ScoreSystem` при любом ударе в препятствие сбрасывает `chain = 0`.

### Окно (`Window` tag)

`PlayerView.BreakTheWindow` → переключает `PlayerSystem.FlightLevel` между `ROOFS` и `BUILDING`, начисляет мгновенные очки, обновляет множитель очков.

## Генерация карты (`MapGenerator`)

Реализует `IGameTick`, `IGameStart`, `IDisposable`.

- При старте (`ResetMap`) спавнит 3 начальных чанка.
- Подписана на `MovementView.DestroyTriggered` / `SpawnTriggered` для конвейерной замены чанков.
- Каждый чанк (`SpawnChunk`) получает случайное количество платформ:
  - `MinPlatformsTop..MaxPlatformsTop` платформ на крышах (`MapLayer.ROOFS`)
  - `MinPlatformsBottom..MaxPlatformsBottom` внутри здания (`MapLayer.BUILDING`)
- Позиция следующего чанка: `X += Random(MinSpace, MaxSpace) + Length`, `Y = Random(MinHeight, MaxHeight)`.
- `Tick()` пока пустой.
- При утилизации чанка (`ReturnChunk`) перебирает `chunk.Platforms` и возвращает платформы в `PlatformPool`.

### Контейнеры данных карты

- **`ChunkView`**: MonoBehaviour на префабе чанка. Хранит `length`, контейнер `Transform platforms`, высоты этажей (`skyFloorYHeight`, `roofsFloorYHeight`, `buildingFloorYHeight`).
- **`MapContextView`**: MonoBehaviour на сцене. Ссылки на массивы префабов чанков и платформ, корни пулов (`PlatformRoofsRoot`, `PlatformBuildingRoot`, `PoolRoot`), позицию спавна.
- **`MapGeneratorView`**: MonoBehaviour на сцене. Настройки генерации: расстояние между зданиями, диапазон высот, количество платформ, родитель чанков.

## Платформы

### PlatformView
MonoBehaviour на префабе платформы. Сериализует:
- `PlatformType _platformType` — тип поведения (`REGULAR`, `BUILDING_SAVER`)
- `float platformBoostMultiplier` — множитель силы отскока, уникальный для префаба
- `MapLayer CurrentLayer` — слой размещения (устанавливается динамически через `SetLayer`)

### PlatformConfig
ScriptableObject-заготовка (пока содержит только `displayName`). Предназначен для идентификации типа платформы в системе улучшений.

### PlatformType (`Map/PlatformView.cs`)
```csharp
public enum PlatformType { REGULAR, BUILDING_SAVER }
```
- `REGULAR` — стандартная платформа.
- `BUILDING_SAVER` — спасательная платформа нижнего уровня (бонусные очки при отскоке).

### MapLayer (`Player/MapLayer.cs`)
```csharp
public enum MapLayer { SKY, ROOFS, BUILDING }
```
Определяет текущий слой высоты платформы или уровень полёта игрока.

### Текущие префабы
| Префаб | PlatformType | platformBoostMultiplier |
|---|---|---|
| Platform1 | REGULAR | 1.0 |
| Platform2 | REGULAR | 2.7 |
| Platform 3 | REGULAR | 1.0 |

## Пулы объектов

### ObjectPool\<T>
Универсальный пул для `Component`-ов. Принимает массив префабов, при `Get` выбирает случайный из них или возвращает из очереди. Предварительно создаёт `initialCapacity` (5) экземпляров каждого префаба.

### ChunkPool
Наследует `ObjectPool<ChunkView>`. Принимает данные из `MapContextView`.

### PlatformPool
Управляет двумя внутренними `ObjectPool<PlatformView>`:
- `_roofsPool` (префабы `PlatformRoofsPrefabs`, корень `PlatformRoofsRoot`)
- `_buildingsPool` (префабы `PlatformBuildingPrefabs`, корень `PlatformBuildingRoot`)
- `Get(MapLayer, position, parent)` достаёт платформу из нужного пула и вызывает `SetLayer`.
- `Return(PlatformView)` возвращает по `CurrentLayer`.

## Стейт-машина (`GameStateMachine`)

Реализует `IGameTick`. Резолвит состояния через `IObjectResolver.Resolve<T>()`. Состояния зарегистрированы как **Singleton**.

Жизненный цикл забега:

```
PrepareState → (клик) → RunState → (Defeated) → DefeatState → (клик) → PrepareState
```

- **PrepareState**: `Enter()` — сброс карты, движения, очков, игрока. `Tick()` — ожидание клика (не над UI).
- **RunState**: `Enter()` → `MovementSystem.StartMoving()`. `Exit()` → `StopMoving()`. `Tick()` пустой.
- **DefeatState**: `Enter()` пустой. `Tick()` — ожидание клика для перехода в `PrepareState`.

## Игрок

### PlayerView (`Player/PlayerVIew.cs`)
MonoBehaviour. Хранит `direction` (Vector3.up), ссылку на `playerModel` (GameObject).
События из `OnTriggerEnter`:
- `HitThePlatform(PlatformView)` — столкновение с платформой
- `HitTheObstacle(Vector2 normal)` — столкновение с препятствием (вычисляется нормаль по ClosestPoint)
- `BreakTheWindow()` — прохождение через окно

### PlayerSystem
Реализует `IGameStart`. Управляет вращением модели (`SetPlayerModelRotation`) и отслеживает текущий уровень полёта (`FlightLevel`). При `BreakTheWindow` переключает `FlightLevel` между `ROOFS` и `BUILDING`.

## Камера (`CameraSystem`)

Реализует `IGameTick`. Следит за игроком по Y с мёртвой зоной:
- Вычисляет допустимый коридор `[minAllowedCamY, maxAllowedCamY]` из порогов видимости (`TopThreshold`, `BottomThreshold`)
- Стремится к начальной позиции `_initialY`, но удерживает игрока в границах экрана
- Разное сглаживание при подъёме (`SmoothTimeUp`) и спуске (`SmoothTimeDown`)
- Ограничение по `MinY` / `MaxY`

## Параллакс (`ParallaxSystem`)

Реализует `IGameTick`. Делегирует в `ParallaxView.Tick(velocityX, dt)`.
`ParallaxView` — MonoBehaviour с массивом слоёв (`ParallaxLayerData[]`). Каждый слой содержит массив `Transform[] parts` и `speedMultiplier`. Смещает части слоя влево пропорционально скорости, при выходе за левый край экрана перемещает вправо за самую правую часть.

## Очки (`ScoreSystem`)

Реализует `IGameTick`, `IGameStart`, `IDisposable`.

- **Пассивные очки**: `VelocityX * dt * PtsByMetrMultiplier` каждый кадр.
- **Мгновенные очки**: при отскоке от платформы (`PlatformBounceInstantPTS`), разбитии окна (`BreakWindowInstantPTS`), спасении с нижнего уровня (`BackToRoofsInstantPts`).
- **Серия комбо (chain)**: увеличивается при быстром ударе по платформе, снижается при медленном, сбрасывается при ударе в препятствие. Множитель: `1 + 0.25 * chain`.
- **Множитель зоны**: `RoofsMultiplier` (1.0) / `InBuildingMultiplier` (1.4) — переключается при разбитии окна. Хранится в `multiplier`, но пока не применяется в формуле `AddScore`.
- **High Score**: обновляется при сбросе.
- **Конвертация в деньги**: `Score / 10` при `ResetScore()`.

Настройки хранятся в `ScoreView` (MonoBehaviour на сцене).

## Валюта (`MoneySystem`)

- `Money` — текущий баланс.
- `AddMoney(int)` / `TrySpendMoney(int)` — добавление и списание.
- В редакторе начисляет 1000 стартовых монет.
- Отображение через `MoneyView.UpdateMoneyText`.

## Система улучшений (`UpgradeSystem`)

Магазин между забегами. Покупки сохраняются между смертями (в пределах сессии).

### Компоненты

- **`UpgradeID`** — enum идентификаторов: `MAX_SPEED_BONUS`, `ARMOR`, `BASE_PLATFORM_BOUNCE_MULTIPLIER` (не подключён).
- **`UpgradeDefiniton`** — структура: `id`, `DisplayName`, `MaxLevel`, `BaseCost`, `CostGrowthRate`, `ValuePerLevel`.
  - Стоимость: `BaseCost * CostGrowthRate^level` (экспоненциальный рост).
  - Значение: `ValuePerLevel * level` (линейный). Для `ARMOR`: квадратичный с насыщением.
- **`UpgradeConfig`** — `ScriptableObject`, массив `UpgradeDefiniton[]`. Метод `Get(UpgradeID)` для поиска.
- **`UpgradeSystem`** — singleton-сервис:
  - `Dictionary<UpgradeID, int> _levels` — текущие уровни.
  - Подписка на `UpgradeView.buttonPressed`.
  - Списание через `MoneySystem.TrySpendMoney`.
  - Применение в `MovementStats` через `ApplyStat`.
  - Ветка `BASE_PLATFORM_BOUNCE_MULTIPLIER` в `ApplyStat` — **пустая** (не реализована).
- **`UpgradeView`** — MonoBehaviour. Массив `UpgradeSlot[]` (каждый слот: `UpgradeID`, `Button`, тексты стоимости и уровня). Событие `buttonPressed(UpgradeID)`.

### Текущие апгрейды

| UpgradeID | Эффект | Формула значения | Статус |
|---|---|---|---|
| `MAX_SPEED_BONUS` | +бонус к макс. горизонтальной скорости | `ValuePerLevel * level` | ✅ Работает |
| `ARMOR` | Снижение потери скорости при ударе об препятствие | Квадратичная кривая с насыщением | ✅ Работает |
| `BASE_PLATFORM_BOUNCE_MULTIPLIER` | Множитель отскока от платформ | — | ❌ Не подключён |

## Ключевые интерфейсы

- **`IGameTick`** (`Core/IGameTick.cs`): `void Tick(float deltaTime)` — единый интерфейс обновления.
- **`IGameState`** (`StateMachine/IGameState.cs`): `void Enter()` (default empty), `void Tick(float deltaTime)`, `void Exit()`.
- **`IGameStart`** (`Core/IGameStart.cs`): `void Start()` — ручная инициализация.

## View-слой

| View | Файл | Назначение |
|---|---|---|
| `PlayerView` | `Player/PlayerVIew.cs` | Столкновения (`OnTriggerEnter`), события `HitThePlatform` / `HitTheObstacle` / `BreakTheWindow` |
| `MovementView` | `Movement/MovementView.cs` | Направление смещения чанков, события `DestroyTriggered` / `SpawnTriggered` из триггеров |
| `CameraView` | `Camera/CameraView.cs` | Управление позицией камеры по Y, доступ к `Camera` |
| `ParallaxView` | `Graphics/Parralax/ParallaxView.cs` | Слои параллакса, бесконечный скроллинг с переносом частей |
| `ScoreView` | `PTS/ScoreView.cs` | UI очков/комбо/множителя/рекорда + настройки начисления (хранит параметры) |
| `MoneyView` | `Money/MoneyView.cs` | UI баланса валюты |
| `UpgradeView` | `Upgrades/UpgradeView.cs` | UI магазина улучшений (слоты с кнопками покупки) |
| `MapContextView` | `Map/MapContextView.cs` | Ссылки на префабы и корни пулов |
| `MapGeneratorView` | `Map/MapGeneratorView.cs` | Настройки генерации (расстояния, высоты, количество платформ) |
| `ChunkView` | `Map/ChunkView.cs` | Данные чанка: длина, контейнер платформ, высоты этажей |
| `PlatformView` | `Map/PlatformView.cs` | Тип платформы, множитель отскока, текущий слой |

## Отладка и телеметрия

- **`MovementStatsLogger`** (`Movement/MovementStatsLogger.cs`):
  - Под `#if UNITY_EDITOR`. Реализует `IGameTick`, `IGameStart`, `IDisposable`.
  - Логирует телеметрию скоростей X/Y с интервалом 0.5 с, столкновения с платформами и препятствиями.
  - Подписка на `Defeated` для вывода сводного отчёта (`RUN STATISTICS`).
- **`DebugPanel`** (`Level1/DebugPanel.cs`):
  - MonoBehaviour (под `#if UNITY_EDITOR`) для отображения параметров на экране.

## Известные особенности

- Имя файла `PlayerVIew.cs` содержит опечатку (заглавная I).
- `PlatformConfig` — ScriptableObject-заготовка (пока только `displayName`, не используется).
- `UpgradeID.BASE_PLATFORM_BOUNCE_MULTIPLIER` заведён, но ветка `ApplyStat` пустая.
- `ScoreSystem.multiplier` (множитель зоны) хранится, обновляется, отображается в UI, но не участвует в формуле `AddScore`.
- `MapGenerator.Tick()` пустой.
- `Game.Tick()` на RootScope-уровне пустой.
- В `GameStateMachine` есть закомментированный блок старого дизайна состояний.
