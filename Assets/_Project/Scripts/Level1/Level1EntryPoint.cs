using System;
using _Project.Scripts.Camera;
using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.ObjectPools;
using _Project.Scripts.StateMachine;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Scripts.Level1
{
    public class Level1EntryPoint : IStartable, ITickable, IDisposable
    {
        private MovementSystem _movementSystem;
        private GameStateMachine _gameStateMachine;
        private MapGenerator _generator;
        private CameraSystem _cameraSystem;

        public Level1EntryPoint(MovementSystem movementSystem, GameStateMachine gameStateMachine, MapGenerator generator, CameraSystem cameraSystem)
        {
            _movementSystem = movementSystem;
            _gameStateMachine = gameStateMachine;
            _generator = generator;
            _cameraSystem = cameraSystem;
        }
        
        public void Start()
        {
            _gameStateMachine.Enter<PrepareState>();
            
            _movementSystem.Defeated += () => _gameStateMachine.Enter<DefeatState>();
            
            _movementSystem.Start();
            _generator.Start();
        }
        
        public void Dispose()
        {
            
        }
        
        public void Tick()
        {
            _movementSystem.Tick(Time.deltaTime);
            _gameStateMachine.Tick(Time.deltaTime);
            _generator.Tick(Time.deltaTime);
            _cameraSystem.Tick(Time.deltaTime);
        }
    }
}