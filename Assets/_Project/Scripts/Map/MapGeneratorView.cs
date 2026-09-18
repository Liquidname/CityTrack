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
        [SerializeField]
        private GameObject[] platforms;
        
        public float MinSpace => minSpace;
        public float MaxSpace => maxSpace;
        public float MinHeight => minHeight;
        public float MaxHeight => maxHeight;
        public GameObject[] Platforms => platforms;
    }
}