using _Project.Scripts.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.StateMachine
{
    public class GameStateMachine : IGameTick
    {
        IObjectResolver _resolver;
        private IGameState _current;
        
        public GameStateMachine(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void Enter<T>() where T : IGameState
        {
            if (_current != null)
            {
                Debug.Log("Exit state: " + _current.GetType().Name);
                _current.Exit();
            }

            _current = _resolver.Resolve<T>();
            
            Debug.Log("Enter state: " + _current.GetType().Name);
            _current.Enter();
        }

        public void Tick(float deltaTime)
        {
            _current.Tick(deltaTime);
        }
    }
}

/* Сегодня уже спать пойду, вот в нейронке это было, 10 сентября дописать всё надо
// ===== Состояния =====
public class PreparationState : IRunState
{
    private readonly RunStateMachine _fsm;

    public PreparationState(RunStateMachine fsm) => _fsm = fsm;

    public void Enter()
    {
        Debug.Log("Preparation: Enter");
        // спавн уровня, обратный отсчёт, UI подготовки
    }

    public void Tick()
    {
        // например, ждём таймер или тап игрока
        // if (готово) _fsm.Enter<GameplayState>();
    }

    public void Exit() => Debug.Log("Preparation: Exit");
}
*/