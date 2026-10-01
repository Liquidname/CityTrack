using _Project.Scripts.Movement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.StateMachine
{
    public class DefeatState : IGameState
    {
        private readonly GameStateMachine _sfm;
        private readonly MovementSystem _movementSystem;

        public DefeatState(GameStateMachine stateMachine,  MovementSystem movementSystem)
        {
            _sfm = stateMachine;
            _movementSystem = movementSystem;
        }
        public void Enter()
        {
            _movementSystem.StopMoving();
        }

        public void Tick(float deltaTime)
        {
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                _sfm.Enter<PrepareState>();
            };
        }

        public void Exit()
        {
        }
    }
}
