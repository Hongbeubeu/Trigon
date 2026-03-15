using UnityEngine;

public class ScoreService
{
    private readonly GameSessionData _session;
    private readonly string _maxScoreKey;

    public ScoreService(GameSessionData session, LogicConfig config)
    {
        _session = session;
        _maxScoreKey = config.MaxScoreKey;
    }

    public void Reset()
    {
        _session.Score = 0;
        GameEvents.RaiseScoreChanged(0);
        GameEvents.RaiseMaxScoreLoaded(GetMaxScore());
    }

    public void AddScore(int points)
    {
        _session.Score += points;
        GameEvents.RaiseScoreChanged(_session.Score);
    }

    public void SaveMaxScoreIfNeeded()
    {
        if (_session.Score > GetMaxScore())
        {
            PlayerPrefs.SetInt(_maxScoreKey, _session.Score);
            GameEvents.RaiseMaxScoreLoaded(_session.Score);
        }
    }

    private int GetMaxScore()
    {
        return PlayerPrefs.GetInt(_maxScoreKey, 0);
    }
}
