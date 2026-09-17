using System;
using _Project.Scripts.Map;
using TMPro;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.Level1
{
    
#if UNITY_EDITOR
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _velocityYText;
        [SerializeField] private TMP_Text _velocityXText;
        
        private MovementSystem _movement;
        
        [Inject]
        public void Construct(MovementSystem movement)
        {
            _movement = movement;
        }

        private void Update()
        {
            _velocityXText.text = "Velocity X: " + _movement._velocityX.ToString();
            _velocityYText.text = "Velocity Y: " + _movement._velocityY.ToString();
        }
    }
#endif
}