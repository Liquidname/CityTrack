using _Project.Scripts.StateMachine;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Scripts.Core
{
    public class GameEntryPoint : IStartable, ITickable
    {
        private Game _game;

        public GameEntryPoint(Game game)
        {
            _game = game;
        }
        
        public void Start()
        {
        }

        public void Tick()
        {
            _game.Tick(Time.deltaTime);
        }
    }
}