using UnityEngine;

namespace _Project.Scripts.Map
{
    public class MapGeneratorView : MonoBehaviour
    {
        [Header("Расстояние между зданиями")]
        [SerializeField] private float minSpace = 15f;
        [SerializeField] private float maxSpace = 35f;
        
        [Header("Высота зданий")]
        [SerializeField] private float minHeight = -2.5f;
        [SerializeField] private float maxHeight = 2.5f;

        [Header("Prefabs")]
        [SerializeField] private Transform chunkParent;
        
        [Header("Platforms Parameters")]
        [SerializeField] private int minPlatformsTop = 0;
        [SerializeField] private int maxPlatformsTop = 3;
        [SerializeField] private int minPlatformsBottom = 0, maxPlatformsBottom = 3;
        
        
        public float MinSpace => minSpace;
        public float MaxSpace => maxSpace;
        public float MinHeight => minHeight;
        public float MaxHeight => maxHeight;
        public Transform ChunkParent => chunkParent;
        public int MinPlatformsTop => minPlatformsTop;
        public int MaxPlatformsTop => maxPlatformsTop;
        public int MinPlatformsBottom => minPlatformsBottom;
        public int MaxPlatformsBottom => maxPlatformsBottom;
        
    }
}