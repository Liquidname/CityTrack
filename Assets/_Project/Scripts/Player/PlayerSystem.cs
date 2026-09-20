using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerSystem
    {
        private PlayerView _playerView;
        
        public PlayerSystem(PlayerView playerView)
        {
            _playerView = playerView;
        }

        public void SetPlayerModelRotation(Quaternion targetRotation)
        {
            // Can be made smooth by Lerp if needed
            _playerView.PlayerModel.transform.rotation = targetRotation;
        }
    }
}