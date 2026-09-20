using _Project.Scripts.Core;
using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Camera
{
    public class CameraSystem : IGameTick
    {
        private readonly CameraView _cameraView;
        private readonly PlayerView _playerView;
        private readonly CameraSettings _settings;

        private readonly float _initialY;

        // Пороги в мировом пространстве относительно центра камеры — кэшируются в конструкторе,
        // т.к. камера по Z не двигается и halfHeight постоянна.
        private readonly float _topMargin;
        private readonly float _bottomMargin;

        private float _smoothVelocityY;

        public CameraSystem(CameraView cameraView, PlayerView playerView, CameraSettings settings)
        {
            _cameraView = cameraView;
            _playerView = playerView;
            _settings = settings;

            _initialY = cameraView.Position.y;

            // Полувысота видимой области на плоскости Z игрока
            float distance = Mathf.Abs(cameraView.Position.z - playerView.transform.position.z);
            float halfHeight = distance * Mathf.Tan(cameraView.Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            // Смещения границ от центра камеры: положительное — выше центра, отрицательное — ниже
            _topMargin = halfHeight * (2f * settings.TopThreshold - 1f);
            _bottomMargin = halfHeight * (2f * settings.BottomThreshold - 1f);
        }

        public void ResetCamera()
        {
            _smoothVelocityY = 0f;
            _cameraView.ResetPositionY(_initialY);
        }

        public void Tick(float deltaTime)
        {
            float currentY = _cameraView.Position.y;
            float playerY = _playerView.transform.position.y;

            // Допустимый коридор высоты камеры, удерживающий игрока в границах экрана
            float minAllowedCamY = playerY - _topMargin;
            float maxAllowedCamY = playerY - _bottomMargin;

            // Камера стремится к _initialY, но удерживает игрока внутри коридора видимости
            float targetY = Mathf.Clamp(_initialY, minAllowedCamY, maxAllowedCamY);
            targetY = Mathf.Clamp(targetY, _settings.MinY, _settings.MaxY);

            // Разное время сглаживания при подъеме и спуске
            float smoothTime = targetY > currentY ? _settings.SmoothTimeUp : _settings.SmoothTimeDown;

            float newY = Mathf.SmoothDamp(currentY, targetY, ref _smoothVelocityY, smoothTime, Mathf.Infinity, deltaTime);
            _cameraView.SetPositionY(newY);
        }
    }
}
