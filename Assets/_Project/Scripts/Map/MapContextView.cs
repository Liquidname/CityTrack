using UnityEngine;

namespace _Project.Scripts.Map
{
    public class MapContextView : MonoBehaviour
    {
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private ChunkView[] chunkPrefabs;
        [SerializeField] private Transform poolRoot;
        [SerializeField] private Transform chunkParent;
        
        public Transform SpawnPosition => spawnPosition;
        public ChunkView[] СhunkPrefabs => chunkPrefabs;
        public Transform PoolRoot => poolRoot;
        public Transform ChunkParent => chunkParent;
    }
}