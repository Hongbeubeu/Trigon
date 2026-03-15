using UnityEngine;

public class PlayingState : IState
{
    private readonly GameContext _context;

    public PlayingState(GameContext context)
    {
        _context = context;
    }

    public void Enter()
    {
        _context.Session.State = GameState.Playing;
        GameEvents.RaiseGameStateChanged(GameState.Playing);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _context.StateMachine.ChangeState(GameState.Paused);
        }
    }

    public void Exit() { }
}
