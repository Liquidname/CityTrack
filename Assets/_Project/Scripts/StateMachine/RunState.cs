using _Project.Scripts.Map;

namespace _Project.Scripts.StateMachine
{
    public class RunState : IGameState
    {
        MovementSystem _movementSystem;

        public RunState(MovementSystem movementSystem)
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
            _movementSystem.StopMoving();
        }
    }
}