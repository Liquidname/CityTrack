using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.StateMachine
{
    public class PrepareState : IGameState
    {
        private readonly GameStateMachine _sfm;
        private readonly MapGenerator _mapGenerator;
        private readonly MovementSystem _movementSystem;

        public PrepareState(GameStateMachine sfm, MapGenerator mapGenerator, MovementSystem movementSystem)
        {
            _sfm = sfm;
            _mapGenerator = mapGenerator;
            _movementSystem = movementSystem;
        }
        
        public void Enter()
        {
            _mapGenerator.ResetMap();
            _movementSystem.ResetMovement();
        }

        public void Tick(float deltaTime)
        {
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                _sfm.Enter<RunState>();
            };
        }

        public void Exit()
        {
        }
    }
}