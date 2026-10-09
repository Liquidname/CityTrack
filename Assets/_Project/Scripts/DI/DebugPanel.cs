using _Project.Scripts.Movement;
using _Project.Scripts.Player;
using _Project.Scripts.StateMachine;
using TMPro;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.DI
{
    
#if UNITY_EDITOR
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text velocityTextY;
        [SerializeField] private TMP_Text velocityTextX;
        [SerializeField] private TMP_Text passedDistanceText;
        [SerializeField] private TMP_Text flightLayer;
        [SerializeField] private TMP_Text gameState;
        
        private MovementSystem _movement;
        private PlayerSystem _playerSystem;
        private GameStateMachine _sfm;
        
        [Inject]
        public void Construct(MovementSystem movement,  PlayerSystem playerSystem,  GameStateMachine sfm)
        {
            _movement = movement;
            _playerSystem = playerSystem;
            _sfm = sfm;
        }

        private void Update()
        {
            velocityTextX.text = "Velocity X: " + _movement.VelocityX.ToString("F1");
            velocityTextY.text = "Velocity Y: " + _movement.VelocityY.ToString("F1");
            passedDistanceText.text = "Passed Distance: " + _movement.PassedDistance.ToString("F1");
            flightLayer.SetText($"Flight Layer: {_playerSystem.FlightLevel}");
            gameState.SetText("GameState: " + _sfm.GetState());
        }
    }
#endif
}