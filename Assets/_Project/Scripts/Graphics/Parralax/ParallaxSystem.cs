using _Project.Scripts.Core;
using _Project.Scripts.Map;
using _Project.Scripts.Movement;

namespace _Project.Scripts.Graphics.Parralax
{
    public class ParallaxSystem : IGameTick
    {
        private readonly MovementSystem _movementSystem;
        
        private readonly ParallaxView _parallaxView;

        public ParallaxSystem(MovementSystem movementSystem, ParallaxView parallaxView)
        {
            _movementSystem = movementSystem;
            _parallaxView = parallaxView;
        }
        
        public void Tick(float deltaTime)
        {
            _parallaxView.Tick(_movementSystem.VelocityX, deltaTime);
        }
    }
}