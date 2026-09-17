using UnityEngine;

namespace _Project.Scripts.StateMachine
{
    public interface IGameState
    {
        void Enter()
        {
        }
        void Tick(float deltaTime);
        void Exit();
    }
}