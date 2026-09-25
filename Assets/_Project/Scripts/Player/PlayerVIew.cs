using System;
using _Project.Scripts.Map;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.up;
        [SerializeField] private GameObject playerModel;
        
        public event Action<PlatformView> HitThePlatform; //float is platform boost
        public event Action<Vector2> HitTheObstacle;
        public event Action BreakTheWindow;
        
        public Vector3 Direction => direction;
        public GameObject PlayerModel => playerModel;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Platform"))
            {
                Debug.Log("Player hit the platform");
                HitThePlatform?.Invoke(other.GetComponent<PlatformView>());
            } 
            else if (other.gameObject.CompareTag("Obstacle"))
            {
                Vector3 cp = other.ClosestPoint(transform.position);
                Vector2 normal;
                if (Mathf.Abs(cp.y - other.bounds.max.y) < 0.1f)
                {
                    normal = Vector2.up; // Крыша сверху
                }
                else if (Mathf.Abs(cp.y - other.bounds.min.y) < 0.1f)
                {
                    normal = Vector2.down; // Потолок снизу
                }
                else
                {
                    float signX = transform.position.x < other.bounds.center.x ? -1f : 1f;
                    normal = new Vector2(signX, 0f); // Боковая стена
                }
                
                HitTheObstacle?.Invoke(normal);
            }
            else if (other.gameObject.CompareTag("Window"))
            {
                BreakTheWindow?.Invoke();
            }
        }
    }
}