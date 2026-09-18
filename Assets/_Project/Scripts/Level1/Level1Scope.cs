using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.StateMachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.Level1
{
    public class Level1Scope : LifetimeScope
    {
        [SerializeField] private MovementView mapView;
        [SerializeField] private PlayerView player;
        [SerializeField] private MovementSettings movementSettings = new MovementSettings();
        [SerializeField] private MapContextView  mapContextView;
        [SerializeField] private MapGeneratorView mapGeneratorView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(player);
            builder.RegisterComponent(mapView);
            builder.RegisterComponent(mapContextView);
            builder.RegisterComponent(mapGeneratorView);
            
            builder.Register<ObjectPool>(Lifetime.Singleton);
            builder.Register<MapGenerator>(Lifetime.Singleton);

            builder.Register<MovementSystem>(Lifetime.Singleton)
                .WithParameter(movementSettings);
        
            builder.Register<GameStateMachine>(Lifetime.Singleton);
        
            builder.Register<PrepareState>(Lifetime.Transient);
            builder.Register<RunState>(Lifetime.Transient);
            builder.Register<DefeatState>(Lifetime.Transient);
        
            builder.RegisterEntryPoint<Level1EntryPoint>();
        
        }
    }
}
