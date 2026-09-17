using _Project.Scripts.Core;
using _Project.Scripts.Map;
using _Project.Scripts.StateMachine;
using VContainer;
using VContainer.Unity;

public class RootScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Game>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GameEntryPoint>();
    }
}
