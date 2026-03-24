public interface IScoreService
{
    void Reset();
    void AddScore(int points);
    void SaveMaxScoreIfNeeded();
}
