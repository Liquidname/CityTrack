using UnityEngine;

namespace _Project.Scripts.Map
{
    public enum PlatformType
    {
        REGULAR,
        BUILDING_SAVER
    }
    public class PlatformView : MonoBehaviour
    {
        [SerializeField] private PlatformType _platformType;
        [SerializeField] private float platformBoostMultiplier;
        
        public float PlatformBoostMultiplier => platformBoostMultiplier;
        public PlatformType PlatformType => _platformType;
    }
}