using System;

public enum GameState
{
    Playing,
    Paused,
    Lost
}

public static class GameEvents
{
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnMaxScoreLoaded;
    public static event Action<GameState> OnGameStateChanged;

    public static void RaiseScoreChanged(int score)
    {
        OnScoreChanged?.Invoke(score);
    }

    public static void RaiseMaxScoreLoaded(int maxScore)
    {
        OnMaxScoreLoaded?.Invoke(maxScore);
    }

    public static void RaiseGameStateChanged(GameState state)
    {
        OnGameStateChanged?.Invoke(state);
    }
}
