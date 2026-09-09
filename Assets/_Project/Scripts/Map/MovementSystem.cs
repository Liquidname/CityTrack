using UnityEngine;
using VContainer.Unity;

namespace _Project.Scripts.Map
{
    public class MovementSystem : ITickable
    {
        private readonly MovementView _movementView;

        public MovementSystem(MovementView movementView)
        {
            _movementView = movementView;
        }

        public void Tick()
        {
            if(_movementView == null) return;
            
            _movementView.transform.Translate(
                (_movementView.Direction * _movementView.Speed) * Time.deltaTime
                );
            
        }
    }
}