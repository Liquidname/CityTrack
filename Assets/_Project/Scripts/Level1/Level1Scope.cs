using _Project.Scripts.Map;
using _Project.Scripts.Movement;
using _Project.Scripts.ObjectPools;
using _Project.Scripts.Player;
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
            
            builder.Register<ChunkPool>(Lifetime.Singleton);
            builder.Register<PlatformPool>(Lifetime.Singleton);
            builder.Register<MapGenerator>(Lifetime.Singleton);
            
            builder.Register<PlayerSystem>(Lifetime.Singleton);
            
            builder.Register<MovementSystem>(Lifetime.Singleton)
                .WithParameter(movementSettings);
        
            builder.Register<GameStateMachine>(Lifetime.Singleton);
        
            builder.Register<PrepareState>(Lifetime.Singleton);
            builder.Register<RunState>(Lifetime.Singleton);
            builder.Register<DefeatState>(Lifetime.Singleton);
        
            builder.RegisterEntryPoint<Level1EntryPoint>();
        
        }
    }
}
