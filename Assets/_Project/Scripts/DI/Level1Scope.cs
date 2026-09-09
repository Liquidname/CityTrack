using _Project.Scripts.Map;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class Level1Scope : LifetimeScope
{
    [SerializeField] private MovementView mapView; 

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(mapView);
        builder.RegisterEntryPoint<MovementSystem>();
    }
}
