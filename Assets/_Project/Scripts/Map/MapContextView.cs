using UnityEngine;

namespace _Project.Scripts.Map
{
    public class MapContextView : MonoBehaviour
    {
        [SerializeField] private Transform spawnPosition;
        
        [Header("Chunks")]
        [SerializeField] private ChunkView[] chunkPrefabs;
        [SerializeField] private Transform poolRoot;
        
        [Header("Platforms")]
        [SerializeField] private PlatformView[] platformPrefabs;
        [SerializeField] private Transform platformRoot;
        
        public Transform SpawnPosition => spawnPosition;
        public ChunkView[] СhunkPrefabs => chunkPrefabs;
        public Transform PoolRoot => poolRoot;
        public PlatformView[] PlatformPrefabs => platformPrefabs;
        public Transform PlatformRoot => platformRoot;
        
    }
}