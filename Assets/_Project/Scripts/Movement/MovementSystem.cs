using System;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Map;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Movement
{
    public enum FlightState
    {
        DIVE,
        GLIDE
    }

    public class MovementSystem : IGameTick, IGameStart, IDisposable
    {
        private readonly MovementView _movementView;
        private readonly PlayerView _playerView;
        private readonly MovementSettings _settings;
        private readonly MapGenerator _mapGenerator;
        private readonly PlayerSystem _playerSystem;

        private bool _readyToMove;
        private FlightState _flightState = FlightState.GLIDE;

        private float _diveEntrySpeed;

        private readonly Vector3 _initialPlayerPosition;
        private readonly Quaternion _initialPlayerRotation;

        private readonly float _startingVelocityX;
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
        
        public event Action Defeated;

        public MovementSystem(MovementView movementView, PlayerView playerView, MovementSettings settings, MapContextView mapContextView, MapGenerator mapGenerator, PlayerSystem playerSystem)
        {
            _movementView = movementView;
            _playerView = playerView;
            _settings = settings;
            _startingVelocityX = settings.HorizontalSpeed;
            _mapGenerator = mapGenerator;
            _playerSystem =  playerSystem;
            _initialPlayerPosition = playerView.transform.position;
            _initialPlayerRotation = playerView.PlayerModel.transform.rotation;
        }

        public void ResetMovement()
        {
            _readyToMove = false;
            VelocityX = 0f;
            VelocityY = 0f;
            _diveEntrySpeed = 0f;
            _flightState = FlightState.GLIDE;
            _playerView.transform.position = _initialPlayerPosition;
            _playerSystem.SetPlayerModelRotation(_initialPlayerRotation);
        }

        public void StartMoving()
        {
            _readyToMove = true;
            VelocityX = _startingVelocityX;
        }

        public void StopMoving()
        {
            VelocityX = 0f;
            _readyToMove = false;
        }
        
        public void Start()
        {
            _playerView.HitThePlatform += OnPlayerBounce;
            _playerView.HitTheObstacle += OnPlayerHitObstacle;
        }

        public void Dispose()
        {
            Debug.Log("Disposing MovementSystem, all event unsubscribed");
            _playerView.HitThePlatform -= OnPlayerBounce;
            _playerView.HitTheObstacle -= OnPlayerHitObstacle;
        }
        
        public void Tick(float deltaTime)
        {
            if (!_readyToMove) return;

            _flightState = Input.GetMouseButton(0) ? FlightState.DIVE : FlightState.GLIDE;

            ComputeVerticalVelocity(deltaTime);
            ComputeHorizontalVelocity(deltaTime);
            Move(deltaTime);
        }

        private void Move(float deltaTime)
        {
            // X - Map move
            foreach (var i in _mapGenerator.ActiveChunks)
            {
                i.transform.Translate(_movementView.Direction * (VelocityX * deltaTime));
            }
            
            // Y - Player move
            _playerView.transform.Translate(_playerView.Direction * (VelocityY * deltaTime));
            
            // Set Player Rotation
            SetPlayerRotation();
        }

        private void SetPlayerRotation()
        {
            float angle = Mathf.Atan2(VelocityY, VelocityX) * Mathf.Rad2Deg;
            _playerSystem.SetPlayerModelRotation(Quaternion.Euler(0, 0, angle-90*-1));
        }

        private void ComputeVerticalVelocity(float deltaTime)
        {
            if (_flightState == FlightState.DIVE)
            {
                VelocityY = Mathf.Clamp(
                    VelocityY - (_settings.Gravity + _settings.ForceDive) * deltaTime,
                    _settings.MinVerticalSpeed,
                    _settings.MaxVerticalSpeed);
                return;
            }

            VelocityY = Mathf.MoveTowards(
                VelocityY,
                -_settings.GlideDescentSpeed,
                _settings.GlideBrake * deltaTime);
        }

        private void ComputeHorizontalVelocity(float deltaTime)
        {
            float speedChange;
            float minSpeed;

            if (_flightState == FlightState.DIVE)
            {
                _diveEntrySpeed = Mathf.Max(_diveEntrySpeed, VelocityX);
                minSpeed = Mathf.Max(
                    _settings.MinHorizontalSpeed,
                    _diveEntrySpeed * _settings.DiveSpeedLossLimit);
                speedChange = -_settings.DiveBrake;
            }
            else
            {
                _diveEntrySpeed = 0f;
                minSpeed = _settings.MinHorizontalSpeed;

                float lift = VelocityY < 0f ? -VelocityY * _settings.GlideLiftCoefficient : 0f;
                speedChange = _settings.GlideAcceleration - VelocityX * _settings.Drag + lift;
            }

            VelocityX = Mathf.Clamp(
                VelocityX + speedChange * deltaTime,
                minSpeed,
                _settings.MaxHorizontalSpeed);
        }

        private void OnPlayerBounce(float platformBoostMultiplier)
        {
            if (VelocityX < _settings.MinBounceSpeed)
            {
                Defeated?.Invoke();
                return;
            }

            AddBounceForce(_settings.BounceYBoostMultiplier, platformBoostMultiplier);
        }
        
        private void OnPlayerHitObstacle(Vector2 normal)
        {
            if (VelocityX < _settings.MinBounceSpeed)
            {
                Defeated?.Invoke();
                return;
            }
            
            CalculateObstacleBounce(normal);
        }

        private void AddBounceForce(float multiplier, float platFormMultiplier)
        {
            float boost = VelocityX / 4; // Magic Number, AI please add it to the MovementSetting when you saw it and remove the comment
            VelocityY += boost * platFormMultiplier * multiplier;
        }
        
        private void CalculateObstacleBounce(Vector2 normal)
        {
            // normal.y = 1, удар сверху вниз, направлен вверх
            // normal.x = 1, удар слева направо, направлен влево
            // normal.x = -1 там понятно
            // 45 слева направа - -0.707, 0.707
            if (normal.y > 0.7f)
            {
                Debug.Log("Player hit obstacle on top");
                AddBounceForce(_settings.BounceYBoostMultiplier/4, 1);
                VelocityX *= _settings.ReduceXonObstacleHit;
            }
            else if (normal.y < -0.7f)
            {
                Debug.Log("Player hit obstacle ceiling");
                if (VelocityY > 0)
                {
                    VelocityY = 0;
                }
                AddBounceForce(-_settings.BounceYBoostMultiplier / 4, 1);
                VelocityX *= _settings.ReduceXonObstacleHit;
            }
            else if (Mathf.Abs(normal.x) > 0.7f)
            {
                VelocityX *= _settings.ReduceXonObstacleHit * -1;
            }
        }
    }
}
