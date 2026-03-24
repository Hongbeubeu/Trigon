/// <summary>
/// A lightweight pure-data context payload injected into each GameState
/// to allow them to interrogate current scores and request transitions.
/// </summary>
public class GameContext
{
    public GameSessionData Session { get; }
    public IScoreService ScoreService { get; }
    public StateMachine<GameState> StateMachine { get; }

    public GameContext(GameSessionData session, IScoreService scoreService, StateMachine<GameState> stateMachine)
    {
        Session = session;
        ScoreService = scoreService;
        StateMachine = stateMachine;
    }
}
