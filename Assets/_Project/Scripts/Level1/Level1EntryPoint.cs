using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.StateMachine;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Scripts.Level1
{
    public class Level1EntryPoint : IStartable, ITickable
    {
        private MovementSystem _movementSystem;
        private GameStateMachine _gameStateMachine;
        private MapGenerator _generator;
        private ObjectPool _objectPool;

        public Level1EntryPoint(MovementSystem movementSystem, GameStateMachine gameStateMachine, MapGenerator generator)
        {
            _movementSystem = movementSystem;
            _gameStateMachine = gameStateMachine;
            _generator = generator;
        }
        
        public void Start()
        {
            _gameStateMachine.Enter<PrepareState>();    
            _movementSystem.Start();
            _generator.Start();
            
        }

        public void Tick()
        {
            _movementSystem.Tick(Time.deltaTime);
			_gameStateMachine.Tick(Time.deltaTime);
            _generator.Tick(Time.deltaTime);
        }
    }
}