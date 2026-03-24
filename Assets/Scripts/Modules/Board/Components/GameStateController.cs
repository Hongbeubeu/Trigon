/// <summary>
/// A pure C# non-Unity logic controller that manages strictly defined GameStates
/// and maps external UI pause/resume requests into StateMachine transitions.
/// </summary>
public class GameStateController
{
    private readonly StateMachine<GameState> _stateMachine;
    
    public GameState CurrentState => _stateMachine?.CurrentKey ?? GameState.Playing;

    public GameStateController(IDataService dataService, IScoreService scoreService)
    {
        _stateMachine = new StateMachine<GameState>();
        var context = new GameContext(dataService.Session, scoreService, _stateMachine);

        _stateMachine.RegisterState(GameState.Playing, new PlayingState(context));
        _stateMachine.RegisterState(GameState.Paused, new PausedState(context));
        _stateMachine.RegisterState(GameState.Lost, new LostState(context));
        
        GameEvents.OnPauseRequested += OnPauseRequested;
        GameEvents.OnResumeRequested += OnResumeRequested;
    }

    public void Update()
    {
        _stateMachine.Update();
    }

    public void ChangeState(GameState state)
    {
        _stateMachine.ChangeState(state);
    }

    private void OnPauseRequested()
    {
        if (CurrentState == GameState.Playing)
            ChangeState(GameState.Paused);
    }

    private void OnResumeRequested()
    {
        ChangeState(GameState.Playing);
    }

    public void Dispose()
    {
        GameEvents.OnPauseRequested -= OnPauseRequested;
        GameEvents.OnResumeRequested -= OnResumeRequested;
    }
}
