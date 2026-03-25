public class PausedState : IState
{
    private readonly GameContext _context;

    public PausedState(GameContext context)
    {
        _context = context;
    }

    public void Enter()
    {
        _context.Session.State = GameState.Paused;
        GameEvents.RaiseGameStateChanged(GameState.Paused);
    }

    public void Update() { }

    public void Exit() { }
}
