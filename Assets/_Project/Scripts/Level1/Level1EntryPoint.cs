using _Project.Scripts.Map;
using _Project.Scripts.StateMachine;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Scripts.Level1
{
    public class Level1EntryPoint : IStartable, ITickable
    {
        private MovementSystem _movementSystem;
        private GameStateMachine _gameStateMachine;

        public Level1EntryPoint(MovementSystem movementSystem, GameStateMachine gameStateMachine)
        {
            _movementSystem = movementSystem;
            _gameStateMachine = gameStateMachine;
        }
        
        public void Start()
        {
            _gameStateMachine.Enter<PrepareState>();    
            
        }

        public void Tick()
        {
            _movementSystem.Tick(Time.deltaTime);
			_gameStateMachine.Tick(Time.deltaTime);
        }
    }
}