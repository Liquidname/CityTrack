using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.up;
        [SerializeField] private GameObject playerModel;
        
        public event Action HitThePlatform;
        public event Action<Vector2> HitTheObstacle;
        
        public Vector3 Direction => direction;
        public GameObject PlayerModel => playerModel;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Triggered On Player");
            if (other.gameObject.CompareTag("Platform"))
            {
                Debug.Log("Player hit the platform");
                HitThePlatform?.Invoke();
            }
            
            if (other.gameObject.CompareTag("Obstacle"))
            {
                Vector3 diff = transform.position - other.bounds.center;
                Vector3 extents = other.bounds.extents;
                // Сравниваем нормализованное смещение по осям X и Y
                Vector2 normal = Mathf.Abs(diff.x / extents.x) > Mathf.Abs(diff.y / extents.y)
                    ? new Vector2(Mathf.Sign(diff.x), 0f)  // Чистый боковой удар: (1, 0) или (-1, 0)
                    : new Vector2(0f, Mathf.Sign(diff.y)); // Чистый вертикальный удар: (0, 1) или (0, -1)
                
                HitTheObstacle?.Invoke(normal);
            }
        }

        
    }
}