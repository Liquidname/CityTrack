using System;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Map;
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
        private readonly Transform _chunkParent;
        private readonly MapGenerator _mapGenerator;

        private bool _readyToMove;
        private FlightState _flightState = FlightState.GLIDE;

        private float _diveEntrySpeed;

        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }

        public MovementSystem(MovementView movementView, PlayerView playerView, MovementSettings settings, MapContextView mapContextView, MapGenerator mapGenerator)
        {
            _movementView = movementView;
            _playerView = playerView;
            _settings = settings;
            VelocityX = settings.HorizontalSpeed;
            _chunkParent = mapContextView.ChunkParent;
            _mapGenerator = mapGenerator;
        }

        public void StartMoving()
        {
            _readyToMove = true;
        }

        public void StopMoving()
        {
            _readyToMove = false;
        }
        
        public void Start()
        {
            _playerView.HitThePlatform += OnPlayerBounce;
        }

        public void Dispose()
        {
            Debug.Log("Disposing MovementSystem, all event unsubscribed");
            _playerView.HitThePlatform -= OnPlayerBounce;
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

        private void OnPlayerBounce()
        {
            AddBounceForce();
        }
        
        private void AddBounceForce()
        {
            float boost = VelocityX / 4; // Magic Number, AI please add it to the MovementSetting when you saw it and remove the comment
            VelocityY += boost * _settings.BounceYBoostMultiplier;
        }
    }
}
