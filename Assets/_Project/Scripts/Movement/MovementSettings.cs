using System;
using UnityEngine;

namespace _Project.Scripts.Movement
{
    [Serializable]
    public class MovementSettings
    {
        [Header("Скорости, м/с")]
        [SerializeField] private float horizontalSpeed = 18f;
        [SerializeField] private float minHorizontalSpeed = 4f;
        [SerializeField] private float maxHorizontalSpeed = 35f;
        [SerializeField] private float minVerticalSpeed = -35f;
        [SerializeField] private float maxVerticalSpeed = 25f;

        [Header("Ускорения, м/с²")]
        [SerializeField] private float gravity = 20f;
        [SerializeField] private float forceDive = 15f;

        [Header("Планирование")]
        [Tooltip("Темп снижения при расправленных крыльях, м/с (по модулю)")]
        [SerializeField] private float glideDescentSpeed = 1f;
        [Tooltip("Как быстро крылья гасят набранную скорость снижения, м/с²")]
        [SerializeField] private float glideBrake = 8f;

        [Header("Потеря и разгон скорости по углу")]
        [Tooltip("Угол пикирования в градусах, при котором скорость ещё не теряется (доку: 10-20)")]
        [SerializeField] private float safeDiveAngle = 15f;
        [Tooltip("Доля скорости, теряемая за секунду при пикировании строго вниз (для Glide)")]
        [SerializeField] private float diveSpeedLossRate = 0.5f;
        [Tooltip("Потеря горизонтальной скорости в пикировании, м/с²")]
        [SerializeField] private float diveBrake = 5f;
        [Tooltip("Доля (0-1) от скорости входа в пикирование, ниже которой пикирование её не снижает. 0.6 = до 60%")]
        [SerializeField] private float diveSpeedLossLimit = 0.6f;
        [Tooltip("Разгон вперёд при расправленных крыльях, м/с²")]
        [SerializeField] private float glideAcceleration = 5f;
        [Tooltip("Потеря горизонтальной скорости за секунду (доля от текущей)")]
        [SerializeField] private float drag = 0.15f;
        [Tooltip("Перевод скорости снижения в разгон вперёд при расправленных крыльях, м/с² на каждый м/с снижения")]
        [SerializeField] private float glideLiftCoefficient = 0.35f;
        
        [Header("Bouncing")]
        [Tooltip("Bounce Y Boost multiplier")]
        [SerializeField] private float bounceYBoostMultiplier = 1.4f;
        [SerializeField] private float reduceXonObstacleHit = 0.7f;
        [Tooltip("Минимальная скорость X для отскока от платформы, ниже которой засчитывается поражение")]
        [SerializeField] private float minBounceSpeed = 4f;

        [Header("Constants")] [SerializeField] private float instantLooseY;
        

        public float HorizontalSpeed => horizontalSpeed;
        public float MinHorizontalSpeed => minHorizontalSpeed;
        public float MaxHorizontalSpeed => maxHorizontalSpeed;
        public float MinVerticalSpeed => minVerticalSpeed;
        public float MaxVerticalSpeed => maxVerticalSpeed;
        public float Gravity => gravity;
        public float ForceDive => forceDive;
        public float GlideDescentSpeed => glideDescentSpeed;
        public float GlideBrake => glideBrake;
        public float SafeDiveAngle => safeDiveAngle;
        public float DiveSpeedLossRate => diveSpeedLossRate;
        public float DiveBrake => diveBrake;
        public float DiveSpeedLossLimit => diveSpeedLossLimit;
        public float GlideAcceleration => glideAcceleration;
        public float Drag => drag;
        public float GlideLiftCoefficient => glideLiftCoefficient;
        public float BounceYBoostMultiplier => bounceYBoostMultiplier;
        public float ReduceXonObstacleHit => reduceXonObstacleHit;
        public float MinBounceSpeed => minBounceSpeed;
        public float InstantLooseY => instantLooseY;
    }
}
