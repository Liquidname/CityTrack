using UnityEngine;

namespace _Project.Scripts.StateMachine
{
    public class DefeatState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Defeat: Enter");
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
