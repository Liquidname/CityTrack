namespace _Project.Scripts.StateMachine
{
    public interface IGameState
    {
        void Enter();
        void Tick();
        void Exit();
    }
}