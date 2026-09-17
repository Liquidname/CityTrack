using System;
using UnityEngine;

namespace _Project.Scripts.Map
{
    [Serializable]
    public class MovementSettings
    {
        [SerializeField] private float horizontalSpeed = 1f;
        [SerializeField] private float maxHorizontalSpeed = 4f;
        [SerializeField] private float minHorizontalSpeed = 0.1f;
        [SerializeField] private float verticalSpeed = 1f, maxVerticalSpeed = 4f, minVerticalSpeed = -4f;
        [SerializeField] private float gravity = 0.15f;
        [SerializeField] private float glideGravityReduceCoefficient = 0.25f;
        [SerializeField] private float forceDive = 1.7f;
        [SerializeField] private float liftForce = 0.2f;
        [SerializeField] private float diveXLossCoefficient = 0.015f;
        [SerializeField] private float yToXConversionCoefficient = 0.1f;
        [SerializeField] private float drag = 0.995f;
        

        public float HorizontalSpeed => horizontalSpeed;
        public float VerticalSpeed => verticalSpeed;
        public float Gravity => gravity;
        public float GlideGravityReduceCoefficient => glideGravityReduceCoefficient;
        public float ForceDive => forceDive;
        public float LiftForce => liftForce;
        public float DiveXLossCoefficient => diveXLossCoefficient;
        public float YToXConversionCoefficient => yToXConversionCoefficient;
        public float Drag => drag;
        public float MinHorizontalSpeed => minHorizontalSpeed;
        public float MaxHorizontalSpeed => maxHorizontalSpeed;
        public float MinVerticalSpeed => minVerticalSpeed;
        public float MaxVerticalSpeed => maxVerticalSpeed;
    }
}
