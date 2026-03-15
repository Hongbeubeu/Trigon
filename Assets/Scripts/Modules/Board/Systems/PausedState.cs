using UnityEngine;

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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _context.StateMachine.ChangeState(GameState.Playing);
        }
    }

    public void Exit() { }
}
