using _Project.Scripts.Map;
using _Project.Scripts.Movement;

namespace _Project.Scripts.StateMachine
{
    public class StartSection : IGameState
    {
        private MovementSystem _movementSystem;
        private GameStateMachine _sfm;

        public StartSection(MovementSystem movementSystem,  GameStateMachine sfm)
        {
            _movementSystem = movementSystem;
            _sfm = sfm;
        }
        
        public void Enter()
        {
            _movementSystem.StartMoving();
        }

        public void Tick(float deltaTime)
        {
            // Hardcode, just idk where to put it
            if (_movementSystem.PassedDistance >= 500)
            {
                _sfm.Enter<SecondMapSectionState>();
            }
        }

        public void Exit()
        {
        }
    }
}