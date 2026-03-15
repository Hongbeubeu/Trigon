using UnityEngine;

public class ScoreService
{
    private const string MAX_SCORE_KEY = "MaxScore";

    private readonly GameSessionData _session;

    public ScoreService(GameSessionData session)
    {
        _session = session;
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
            PlayerPrefs.SetInt(MAX_SCORE_KEY, _session.Score);
            GameEvents.RaiseMaxScoreLoaded(_session.Score);
        }
    }

    private static int GetMaxScore()
    {
        return PlayerPrefs.GetInt(MAX_SCORE_KEY, 0);
    }
}
