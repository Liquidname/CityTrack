using System;
using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.Player;
using TMPro;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.Level1
{
    
#if UNITY_EDITOR
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _velocityYText;
        [SerializeField] private TMP_Text _velocityXText;
        [SerializeField] private TMP_Text _flightLayer;
        
        private MovementSystem _movement;
        private PlayerSystem _playerSystem;
        
        [Inject]
        public void Construct(MovementSystem movement,  PlayerSystem playerSystem)
        {
            _movement = movement;
            _playerSystem = playerSystem;
        }

        private void Update()
        {
            _velocityXText.text = "Velocity X: " + _movement.VelocityX.ToString("F1");
            _velocityYText.text = "Velocity Y: " + _movement.VelocityY.ToString("F1");
            _flightLayer.SetText($"Flight Layer: {_playerSystem.FlightLevel}");
        }
    }
#endif
}