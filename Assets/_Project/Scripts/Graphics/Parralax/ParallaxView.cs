using UnityEngine;

namespace _Project.Scripts.Graphics.Parralax
{
    public class ParallaxView : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera targetCamera;
        [SerializeField] private ParallaxLayerData[] layers;

        [System.Serializable]
        private struct ParallaxLayerData
        {
            public Transform[] parts;
            [Range(0f, 1.5f)]
            public float speedMultiplier;
        }

        private float[] _widths;
        private float[] _leftEdges;

        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = UnityEngine.Camera.main;

            _widths    = new float[layers.Length];
            _leftEdges = new float[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                var sr = layers[i].parts[0].GetComponent<SpriteRenderer>();
                _widths[i] = sr.bounds.size.x;

                float z        = layers[i].parts[0].position.z;
                float distance = Mathf.Abs(z - targetCamera.transform.position.z);
                float halfW    = distance
                                 * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad)
                                 * targetCamera.aspect;
                _leftEdges[i] = targetCamera.transform.position.x - halfW;
            }
        }

        public void Tick(float velocityX, float deltaTime)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                float move     = velocityX * layers[i].speedMultiplier * deltaTime;
                float width    = _widths[i];
                float leftEdge = _leftEdges[i];
                var   parts    = layers[i].parts;

                for (int j = 0; j < parts.Length; j++)
                {
                    Vector3 pos = parts[j].position;
                    pos.x -= move;
                    parts[j].position = pos;

                    if (pos.x + width * 0.5f <= leftEdge)
                    {
                        float maxX = parts[0].position.x;
                        for (int k = 1; k < parts.Length; k++)
                            if (parts[k].position.x > maxX) maxX = parts[k].position.x;

                        pos.x = maxX + width;
                        parts[j].position = pos;
                    }
                }
            }
        }
    }
}