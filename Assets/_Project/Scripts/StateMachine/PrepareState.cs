using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.Player;
using _Project.Scripts.PTS;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.StateMachine
{
    public class PrepareState : IGameState
    {
        private readonly GameStateMachine _sfm;
        private readonly MapGenerator _mapGenerator;
        private readonly MovementSystem _movementSystem;
        private readonly ScoreSystem _scoreSystem;
        private readonly PlayerSystem _playerSystem;

        public PrepareState(GameStateMachine sfm, MapGenerator mapGenerator, MovementSystem movementSystem, ScoreSystem scoreSystem, PlayerSystem playerSystem)
        {
            _sfm = sfm;
            _mapGenerator = mapGenerator;
            _movementSystem = movementSystem;
            _scoreSystem = scoreSystem;
            _playerSystem = playerSystem;
        }
        
        public void Enter()
        {
            _mapGenerator.ResetMap();
            _movementSystem.ResetMovement();
            _scoreSystem.ResetScore();
            _playerSystem.ResetPlayer();
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