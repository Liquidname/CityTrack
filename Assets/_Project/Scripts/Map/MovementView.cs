using UnityEngine;

namespace _Project.Scripts.Map
{
    public class MovementView : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private Vector3 direction;

        public float Speed => speed;
        public Vector3 Direction => direction;
    }
}
