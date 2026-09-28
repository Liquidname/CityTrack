using UnityEngine;

namespace _Project.Scripts.Map
{
    [CreateAssetMenu(fileName = "PlatformConfig", menuName = "Scriptable Objects/PlatformConfig")]
    public class PlatformConfig : ScriptableObject
    {
        public string displayName;
        public float baseBoostMultiplier;
        public bool isBuildingSaver = false;
    }
}