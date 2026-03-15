public class GameContext
{
    public GameSessionData Session { get; }
    public ScoreService ScoreService { get; }
    public TileViewRegistry ViewRegistry { get; }
    public StateMachine<GameState> StateMachine { get; }

    public GameContext(
        GameSessionData session,
        ScoreService scoreService,
        TileViewRegistry viewRegistry,
        StateMachine<GameState> stateMachine)
    {
        Session = session;
        ScoreService = scoreService;
        ViewRegistry = viewRegistry;
        StateMachine = stateMachine;
    }
}
