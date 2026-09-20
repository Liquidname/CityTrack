using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.StateMachine
{
    public class DefeatState : IGameState
    {
        private readonly GameStateMachine _sfm;

        public DefeatState(GameStateMachine stateMachine)
        {
            _sfm = stateMachine;
        }
        public void Enter()
        {
            
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
