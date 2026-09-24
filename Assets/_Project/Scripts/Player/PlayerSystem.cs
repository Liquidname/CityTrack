using _Project.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerSystem : IGameStart
    {
        private PlayerView _playerView;
        
        private FlightLevel _flightLevel;
        
        public FlightLevel FlightLevel => _flightLevel;
        
        public PlayerSystem(PlayerView playerView)
        {
            _playerView = playerView;
        }

        public void SetPlayerModelRotation(Quaternion targetRotation)
        {
            // Can be made smooth by Lerp if needed
            _playerView.PlayerModel.transform.rotation = targetRotation;
        }
        
        public void Start()
        {
            ResetPlayer();

            _playerView.BreakTheWindow += OnPlayerBreakTheWindow;
        }

        private void OnPlayerBreakTheWindow()
        {
            if (_flightLevel == FlightLevel.ROOFS)
            {
                _flightLevel = FlightLevel.BUILDING;
            } else if (_flightLevel == FlightLevel.BUILDING)
            {
                _flightLevel = FlightLevel.ROOFS;
            }
        }

        public void ResetPlayer()
        {
            _flightLevel = FlightLevel.ROOFS;
        }
    }
}