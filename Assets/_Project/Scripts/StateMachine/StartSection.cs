using _Project.Scripts.Map;
using _Project.Scripts.Movement;

namespace _Project.Scripts.StateMachine
{
    public class StartSection : IGameState
    {
        MovementSystem _movementSystem;

        public StartSection(MovementSystem movementSystem)
        {
            _movementSystem = movementSystem;
        }
        
        public void Enter()
        {
            _movementSystem.StartMoving();
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}