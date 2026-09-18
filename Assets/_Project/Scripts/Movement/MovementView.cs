using System;
using UnityEngine;

namespace _Project.Scripts.Movement
{
    public class MovementView : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;

        public Vector3 Direction => direction;
        public event Action DestroyTriggered;
        public event Action SpawnTriggered;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Movement view triggered");
            if (other.CompareTag("DestroyTrigger"))
            {
                DestroyTriggered?.Invoke();
            }
            else if (other.CompareTag("SpawnTrigger"))
            {
                SpawnTriggered?.Invoke();
            }
    }
    }
}
