public class LostState : IState
{
    private readonly GameContext _context;

    public LostState(GameContext context)
    {
        _context = context;
    }

    public void Enter()
    {
        _context.ScoreService.SaveMaxScoreIfNeeded();
        _context.Session.State = GameState.Lost;
        GameEvents.RaiseGameStateChanged(GameState.Lost);
    }

    public void Update() { }

    public void Exit() { }
}
