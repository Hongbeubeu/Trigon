public class GameContext
{
    public GameSessionData Session { get; }
    public ScoreService ScoreService { get; }
    public StateMachine<GameState> StateMachine { get; }

    public GameContext(
        GameSessionData session,
        ScoreService scoreService,
        StateMachine<GameState> stateMachine)
    {
        Session = session;
        ScoreService = scoreService;
        StateMachine = stateMachine;
    }
}
