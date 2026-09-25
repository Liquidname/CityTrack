using _Project.Scripts.Player;
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
        [SerializeField] private MapLayer _spawnLayer;
        
        public float PlatformBoostMultiplier => platformBoostMultiplier;
        public PlatformType PlatformType => _platformType;
        public MapLayer SpawnLayer => _spawnLayer;
    }
}