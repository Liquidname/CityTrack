using UnityEngine;

namespace _Project.Scripts.Map
{
    public class ChunkView : MonoBehaviour
    {
        [SerializeField] private float length = 55f;
        public float Length => length;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (length <= 0 && transform.childCount > 0)
                length = transform.GetChild(0).localScale.x;
        }
#endif
    }
}