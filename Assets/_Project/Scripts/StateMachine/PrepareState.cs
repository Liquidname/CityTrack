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
            throw new System.NotImplementedException();
        }

        public void Tick()
        {
            if(EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                _sfm.Enter<>();
            };
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}