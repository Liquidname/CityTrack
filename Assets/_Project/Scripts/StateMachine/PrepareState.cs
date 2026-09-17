using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.StateMachine
{
    public class PrepareState : IGameState
    {
        private GameStateMachine _sfm;

        public PrepareState(GameStateMachine sfm)
        {
            _sfm = sfm;
        }
        
        public void Enter()
        {
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