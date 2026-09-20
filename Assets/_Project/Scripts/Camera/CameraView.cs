using UnityEngine;

namespace _Project.Scripts.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraView : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera targetCamera;

        public UnityEngine.Camera Camera => targetCamera;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = GetComponent<UnityEngine.Camera>();
        }

        public void SetPositionY(float y)
        {
            Vector3 pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        public void ResetPositionY(float y)
        {
            SetPositionY(y);
        }
    }
}
