using System;
using UnityEngine;

namespace _Project.Scripts.Camera
{
    [Serializable]
    public class CameraSettings
    {
        [Range(0.5f, 0.95f), Tooltip("Нормализованная координата Y верхней границы мертвой зоны (0..1)")]
        public float TopThreshold = 0.75f;

        [Range(0.05f, 0.5f), Tooltip("Нормализованная координата Y нижней границы мертвой зоны (0..1)")]
        public float BottomThreshold = 0.25f;

        [Tooltip("Время сглаживания при подъеме камеры (меньше = резче)")]
        public float SmoothTimeUp = 0.12f;

        [Tooltip("Время сглаживания при спуске камеры")]
        public float SmoothTimeDown = 0.20f;

        [Tooltip("Минимальная Y камеры (уровень стартовой позиции)")]
        public float MinY = 5.95f;

        [Tooltip("Максимальная Y камеры")]
        public float MaxY = 200f;
    }
}
