public class ScoreService
{
    private readonly GameSessionData _session;
    private readonly IScorePersistence _persistence;

    public ScoreService(GameSessionData session, IScorePersistence persistence)
    {
        _session = session;
        _persistence = persistence;
    }

    public void Reset()
    {
        _session.Score = 0;
        GameEvents.RaiseScoreChanged(0);
        GameEvents.RaiseMaxScoreLoaded(_persistence.LoadMaxScore());
    }

    public void AddScore(int points)
    {
        _session.Score += points;
        GameEvents.RaiseScoreChanged(_session.Score);
    }

    public void SaveMaxScoreIfNeeded()
    {
        if (_session.Score > _persistence.LoadMaxScore())
        {
            _persistence.SaveMaxScore(_session.Score);
            GameEvents.RaiseMaxScoreLoaded(_session.Score);
        }
    }
}
