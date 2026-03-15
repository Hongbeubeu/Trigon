using UnityEngine;

public class PlayerPrefsScorePersistence : IScorePersistence
{
    private readonly string _key;

    public PlayerPrefsScorePersistence(string key)
    {
        _key = key;
    }

    public int LoadMaxScore()
    {
        return PlayerPrefs.GetInt(_key, 0);
    }

    public void SaveMaxScore(int score)
    {
        PlayerPrefs.SetInt(_key, score);
    }
}
