using UnityEngine;

namespace _Project.Scripts
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.up;
        
        public Vector3 Direction => direction;
    }
}