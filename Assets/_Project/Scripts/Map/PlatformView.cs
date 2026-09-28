using _Project.Scripts.Player;
using Unity.AppUI.Core;
using UnityEngine;

namespace _Project.Scripts.Map
{
    public class PlatformView : MonoBehaviour
    {
        [SerializeField] private PlatformConfig _config;
        public PlatformConfig Config => _config;
        public float PlatformBoostMultiplier => _config.baseBoostMultiplier;
        public MapLayer CurrentLayer { get; private set; }
        
        public void SetLayer(MapLayer layer)
        {
            CurrentLayer = layer;
        }
    }
}