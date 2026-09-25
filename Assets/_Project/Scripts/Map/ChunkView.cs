using UnityEngine;

namespace _Project.Scripts.Map
{
    public class ChunkView : MonoBehaviour
    {
        [SerializeField] private float length = 55f;
        [SerializeField] private float skyFloorYHeight;
        [SerializeField] private float roofsFloorYHeight;
        [SerializeField] private float buildingFloorYHeight;
        [SerializeField] private Transform platforms;
        public float Length => length;
        public Transform Platforms => platforms;
        public float SkyFloorYHeight => skyFloorYHeight;
        public float RoofsFloorYHeight => roofsFloorYHeight;
        public float BuildingFloorYHeight => buildingFloorYHeight;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (length <= 0 && transform.childCount > 0)
                length = transform.GetChild(0).localScale.x;
        }
#endif
    }
}