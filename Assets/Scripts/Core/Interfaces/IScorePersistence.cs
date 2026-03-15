public interface IScorePersistence
{
    int LoadMaxScore();
    void SaveMaxScore(int score);
}
