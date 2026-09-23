using UnityEngine;

namespace _Project.Scripts.Map
{
    public class PlatformView : MonoBehaviour
    {
        [SerializeField] private float platformBoostMultiplier;
        
        public float PlatformBoostMultiplier => platformBoostMultiplier;
    }
}