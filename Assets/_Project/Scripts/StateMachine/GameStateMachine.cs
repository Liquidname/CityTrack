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
        
        public string GetState()
        {
            if (_current != null)
            {
                return _current.GetType().Name;
            }

            return null;
        }
    }
}
