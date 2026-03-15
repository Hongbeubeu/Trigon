using System;
using System.Collections.Generic;

public class StateMachine<TKey> where TKey : Enum
{
    private readonly Dictionary<TKey, IState> _states = new();
    private IState _currentState;

    public TKey CurrentKey { get; private set; }

    public void RegisterState(TKey key, IState state)
    {
        _states[key] = state;
    }

    public void ChangeState(TKey key)
    {
        _currentState?.Exit();
        CurrentKey = key;
        _currentState = _states[key];
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState?.Update();
    }
}
