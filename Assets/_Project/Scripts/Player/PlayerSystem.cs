using _Project.Scripts.Core;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerSystem : IGameStart
    {
        private PlayerView _playerView;
        
        private MapLayer _flightLevel;
        
        public MapLayer FlightLevel => _flightLevel;
        
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
            if (_flightLevel == MapLayer.ROOFS)
            {
                _flightLevel = MapLayer.BUILDING;
            } else if (_flightLevel == MapLayer.BUILDING)
            {
                _flightLevel = MapLayer.ROOFS;
            }
        }

        public void ResetPlayer()
        {
            _flightLevel = MapLayer.ROOFS;
        }
    }
}