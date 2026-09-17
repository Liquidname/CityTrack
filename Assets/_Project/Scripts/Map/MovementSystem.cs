using _Project.Scripts.Core;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Scripts.Map
{
    public enum FlightState {
        DIVE,
        GLIDE
    }
    public class MovementSystem : IGameTick
    {
        private readonly MovementView _movementView;
        private readonly PlayerView _playerView;
        private readonly MovementSettings _settings;

        private bool _readyToMove;
        public float _velocityY { get;  private set; }
        public float _velocityX { get; private set; } = 10f;
        public float boost { get;  private set; }
        
        private FlightState _flightState; 

        public MovementSystem(MovementView movementView, PlayerView playerView, MovementSettings settings)
        {
            _movementView = movementView;
            _playerView = playerView;
            _settings = settings;
        }

        public void StartMoving()
        {
            _readyToMove = true;
        }

        public void StopMoving()
        {
            _readyToMove = false;
        }

        public void Tick(float deltaTime)
        {
            if(_movementView == null) return;

            if (_readyToMove)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _flightState = FlightState.DIVE;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    _flightState = FlightState.GLIDE;
                }
                
                ComputeVerticalVelocity();
                ComputeHorizontalVelocity();
                Move(deltaTime);
            }
        }

        private void Move(float deltaTime)
        {
            _movementView.transform.Translate(
                (_movementView.Direction * _settings.HorizontalSpeed) * _settings.HorizontalSpeed * deltaTime  // X new = X + Vx
            );
            _playerView.transform.Translate(
                (_playerView.Direction * _velocityY) * _settings.VerticalSpeed * deltaTime  // Y new = Y + Vy
            );
        }

        private void ComputeVerticalVelocity()
        {
            if(_flightState == FlightState.DIVE)
                _velocityY += _settings.Gravity + _settings.ForceDive;
            else if(_flightState == FlightState.GLIDE)
                _velocityY += (_settings.Gravity * _settings.GlideGravityReduceCoefficient) - (boost * _settings.LiftForce);
            
            _velocityY = Mathf.Clamp(_velocityY, _settings.MinVerticalSpeed, _settings.MaxVerticalSpeed);
        }
        
        private void ComputeHorizontalVelocity()
        {
            if (_flightState == FlightState.DIVE)
                _velocityX -= _velocityX * _settings.DiveXLossCoefficient;
            else if (_flightState == FlightState.GLIDE)
            {
                if (_velocityY > 0)
                {
                    boost = _velocityY * _settings.YToXConversionCoefficient;
                } else if (_velocityY <= 0)
                {
                    boost = 0;
                }
                
                _velocityX = (_velocityX * boost) * _settings.Drag;
            }

            _velocityX = Mathf.Clamp(_velocityX, _settings.MinHorizontalSpeed, _settings.MaxHorizontalSpeed);
            
        }
    }
}
