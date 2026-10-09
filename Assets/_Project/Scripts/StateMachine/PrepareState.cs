using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.Player;
using _Project.Scripts.PTS;
using _Project.Scripts.Upgrades;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.StateMachine
{
    public class PrepareState : IGameState
    {
        private readonly GameStateMachine _sfm;
        private readonly MapGeneratorSystem _mapGeneratorSystem;
        private readonly MovementSystem _movementSystem;
        private readonly ScoreSystem _scoreSystem;
        private readonly PlayerSystem _playerSystem;
        private readonly UpgradeSystem _upgradeSystem;

        public PrepareState(GameStateMachine sfm, MapGeneratorSystem mapGeneratorSystem, MovementSystem movementSystem, ScoreSystem scoreSystem, 
            PlayerSystem playerSystem, UpgradeSystem upgradeSystem)
        {
            _sfm = sfm;
            _mapGeneratorSystem = mapGeneratorSystem;
            _movementSystem = movementSystem;
            _scoreSystem = scoreSystem;
            _playerSystem = playerSystem;
            _upgradeSystem = upgradeSystem;
        }
        
        public void Enter()
        {
            _mapGeneratorSystem.ResetMap();
            _movementSystem.ResetMovement();
            _scoreSystem.ResetScore();
            _playerSystem.ResetPlayer();
            
            _upgradeSystem.EnableShop(true);
        }

        public void Tick(float deltaTime)
        {
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                _sfm.Enter<StartSection>();
            };
        }

        public void Exit()
        {
            _upgradeSystem.EnableShop(false);
        }
    }
}