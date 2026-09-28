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
        
        public MapLayer CurrentLayer { get; private set; }
        
        public float PlatformBoostMultiplier => platformBoostMultiplier;
        public PlatformType PlatformType => _platformType;

        public void SetLayer(MapLayer layer)
        {
            CurrentLayer = layer;
        }
    }
}