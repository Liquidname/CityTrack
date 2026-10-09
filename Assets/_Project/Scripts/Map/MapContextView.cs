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
        [SerializeField] private PlatformView[] platformBuildingPrefabs;
        [SerializeField] private PlatformView[] platformRoofsPrefabs;
        
        [SerializeField] private Transform platformBuildingRoot;
        [SerializeField] private Transform platformRoofsRoot;
        
        [Header("VFX")]
        [SerializeField] private GameObject fastWind;
        [SerializeField] private Transform fastWindStartPos;
        [SerializeField] private Transform fastWindSustainedPos;
        [SerializeField] private float fastWindSmooth;
        
        [Header("SFX")]
        [SerializeField] private AudioClip secondMapSectionEnterSFX;
        
        public Transform SpawnPosition => spawnPosition;
        public ChunkView[] СhunkPrefabs => chunkPrefabs;
        public Transform PoolRoot => poolRoot;
        public PlatformView[] PlatformBuildingPrefabs => platformBuildingPrefabs;
        public PlatformView[] PlatformRoofsPrefabs => platformRoofsPrefabs;
        public Transform PlatformBuildingRoot => platformBuildingRoot;
        public Transform PlatformRoofsRoot => platformRoofsRoot;
        public GameObject FastWind => fastWind;
        public Transform FastWindStartPos => fastWindStartPos;
        public Transform FastWindSustainedPos => fastWindSustainedPos;
        public float FastWindSmooth => fastWindSmooth;
        public AudioClip SecondMapSectionEnterSFX => secondMapSectionEnterSFX;
    }
}