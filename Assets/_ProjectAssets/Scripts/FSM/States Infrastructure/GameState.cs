namespace _ProjectAssets.Scripts
{
    public abstract class GameState
    {
        public virtual void Enter() {}
        public virtual void Execute() {}
        public virtual void Exit() {}
        public virtual void SetNextState() {}
    }
}