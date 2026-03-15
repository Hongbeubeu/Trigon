using UnityEngine;

public class ScoreService
{
    private const string MAX_SCORE_KEY = "MaxScore";
    private int _currentScore;

    public int CurrentScore
    {
        get => _currentScore;
        private set
        {
            _currentScore = value;
            GameEvents.RaiseScoreChanged(_currentScore);
        }
    }

    public void Reset()
    {
        CurrentScore = 0;
        GameEvents.RaiseMaxScoreLoaded(GetMaxScore());
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
    }

    public void SaveMaxScoreIfNeeded()
    {
        if (_currentScore > GetMaxScore())
        {
            PlayerPrefs.SetInt(MAX_SCORE_KEY, _currentScore);
            GameEvents.RaiseMaxScoreLoaded(_currentScore);
        }
    }

    private static int GetMaxScore()
    {
        return PlayerPrefs.GetInt(MAX_SCORE_KEY, 0);
    }
}
