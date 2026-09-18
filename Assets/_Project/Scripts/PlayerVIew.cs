using System;
using _Project.Scripts.Map;
using UnityEngine;
using VContainer;

namespace _Project.Scripts
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.up;
        
        public event Action HitThePlatform;
        
        public Vector3 Direction => direction;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Triggered On Player");
            if (other.gameObject.CompareTag("Platform"))
            {
                Debug.Log("Player hit the platform");
                HitThePlatform?.Invoke();
            }
        }
    }
}