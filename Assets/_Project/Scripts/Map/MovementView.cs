using UnityEngine;

namespace _Project.Scripts.Map
{
    public class MovementView : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;

        public Vector3 Direction => direction;
    }
}
