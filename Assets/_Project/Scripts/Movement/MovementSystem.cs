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
        private readonly MovementStats _movementStats;

        private bool _readyToMove;
        public FlightState FlightState { get; private set; } = FlightState.GLIDE;

        private float _diveEntrySpeed;

        private readonly Vector3 _initialPlayerPosition;
        private readonly Quaternion _initialPlayerRotation;

        private readonly float _startingVelocityX;
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
        
        public event Action Defeated;
        
        private bool struggleInObstacle;

        public MovementSystem(MovementView movementView, PlayerView playerView, MovementSettings settings, MapContextView mapContextView, 
            MapGenerator mapGenerator, PlayerSystem playerSystem, MovementStats movementStats)
        {
            _movementView = movementView;
            _playerView = playerView;
            _settings = settings;
            _startingVelocityX = settings.LaunchSpeed;
            _mapGenerator = mapGenerator;
            _playerSystem =  playerSystem;
            _initialPlayerPosition = playerView.transform.position;
            _initialPlayerRotation = playerView.PlayerModel.transform.rotation;
            _movementStats = movementStats;
        }

        private float EffectiveMaxHorizontalSpeed => _settings.MaxHorizontalSpeed + _movementStats.MaxSpeedBonus;
        private float EffectiveReduceXonObstacleHit => Mathf.Lerp(_settings.ReduceXonObstacleHit, _settings.MaxReduceXonObstacleHit, _movementStats.ArmorProgress);

        private float GetEffectivePlatformMultiplier(PlatformConfig platformConfig)
        {
            // Сюда бы юнит тест написать
            if (platformConfig == null) return 1f;
            return platformConfig.baseBoostMultiplier + _movementStats.GetPlatformBounceBonus(platformConfig);
        }

        public void ResetMovement()
        {
            _readyToMove = false;
            VelocityX = 0f;
            VelocityY = 0f;
            _diveEntrySpeed = 0f;
            FlightState = FlightState.GLIDE;
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

            FlightState = Input.GetMouseButton(0) ? FlightState.DIVE : FlightState.GLIDE;

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
            Physics.SyncTransforms();

            // Y - Player move
            float deltaY = VelocityY * deltaTime;

            // Защита от проваливания сквозь Obstacle при падении вниз:
            if (deltaY < 0f && CheckObstacleRoof(Mathf.Abs(deltaY), out float roofContactY))
            {
                _playerView.transform.position = new Vector3(_playerView.transform.position.x, roofContactY, _playerView.transform.position.z);
            }
            else
            {
                _playerView.transform.Translate(_playerView.Direction * deltaY);
            }

            if (_playerView.transform.position.y < _settings.InstantLooseY)
            {
                Loose();
            }

            // Set Player Rotation
            SetPlayerRotation();
        }

        private bool CheckObstacleRoof(float distance, out float roofContactY)
        {
            roofContactY = 0f;
            Vector3 origin = _playerView.transform.position;
            float radius = 0.5f;

            RaycastHit[] hits = Physics.SphereCastAll(origin, radius * 0.8f, Vector3.down, distance + 0.1f, ~0, QueryTriggerInteraction.Collide);
            foreach (var hit in hits)
            {
                if (hit.collider.transform.root == _playerView.transform.root) continue;

                // Проверяем крышу препятствия ТОЛЬКО если игрок находится выше её поверхности (падает сверху)
                if (hit.collider.CompareTag("Obstacle") && hit.normal.y > 0.5f)
                {
                    if (origin.y >= hit.collider.bounds.max.y - 0.2f)
                    {
                        roofContactY = hit.point.y + radius;
                        return true;
                    }
                }
            }
            return false;
        }

        private void Loose()
        {
            Defeated?.Invoke();
        }

        private void SetPlayerRotation()
        {
            float angle = Mathf.Atan2(VelocityY, VelocityX) * Mathf.Rad2Deg;
            _playerSystem.SetPlayerModelRotation(Quaternion.Euler(0, 0, angle - 90 * -1));
        }

        private void ComputeVerticalVelocity(float deltaTime)
        {
            if (FlightState == FlightState.DIVE)
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

            if (FlightState == FlightState.DIVE)
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

            // Если игрок отскочил от стены и летит назад (VelocityX < 0):
            // Позволяем ему двигаться назад и плавно восстанавливать скорость вперед
            if (VelocityX < 0f)
            {
                VelocityX += speedChange * deltaTime;
                return;
            }
            
            VelocityX = Mathf.Clamp(
                VelocityX + speedChange * deltaTime,
                minSpeed,
                EffectiveMaxHorizontalSpeed);
        }

        private void OnPlayerBounce(PlatformView platform)
        {
            if (VelocityX < _settings.MinBounceSpeed)
            {
                Loose();
                return;
            }

            AddBounceForce(_settings.BounceYBoostMultiplier, platform);
        }
        
        private void OnPlayerHitObstacle(Vector2 normal)
        {
            if (VelocityX < _settings.MinBounceSpeed)
            {
                Loose();
                return;
            }
            
            CalculateObstacleBounce(normal);
        }

        private void AddBounceForce(float multiplier, PlatformView platform)
        {
            float boost = VelocityX / _settings.BounceVelocityXDivider;
            VelocityY += boost * GetEffectivePlatformMultiplier(platform.Config) * multiplier;
        }
        
        private void CalculateObstacleBounce(Vector2 normal)
        {
            if (normal.y > 0.7f)
            {
                Debug.Log("Player hit obstacle on top");
                // Оригинальный слабый толчок от крыши (без лишней подъемной силы):
                float boost = VelocityX / _settings.BounceVelocityXDivider;
                float roofBounce = boost * (_settings.BounceYBoostMultiplier / 4f);
                VelocityY = roofBounce;
                VelocityX *= EffectiveReduceXonObstacleHit;
                _diveEntrySpeed = VelocityX;

                if (VelocityX < _settings.MinBounceSpeed)
                {
                    Loose();
                }
            }
            else if (normal.y < -0.7f)
            {
                Debug.Log("Player hit obstacle ceiling");
                if (VelocityY > 0f)
                {
                    VelocityY = 0f;
                }
                float boost = VelocityX / _settings.BounceVelocityXDivider;
                VelocityY -= boost * (_settings.BounceYBoostMultiplier / 4f);
                VelocityX *= EffectiveReduceXonObstacleHit;
                _diveEntrySpeed = VelocityX;

                if (VelocityX < _settings.MinBounceSpeed)
                {
                    Loose();
                }
            }
            else if (Mathf.Abs(normal.x) > 0.7f)
            {
                Debug.Log("Player hit obstacle wall");
                // Разворачиваем игрока и отталкиваем назад:
                VelocityX *= EffectiveReduceXonObstacleHit * -1f;
                _diveEntrySpeed = 0f;
            }
        }
    }
}
