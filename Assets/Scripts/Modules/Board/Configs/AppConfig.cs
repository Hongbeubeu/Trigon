using UnityEngine;

[CreateAssetMenu(fileName = "AppConfig", menuName = "Trigon/App Config")]
public class AppConfig : ScriptableObject
{
    [Header("App & Persistence")]
    [SerializeField] private int _targetFrameRate = 120;
    [SerializeField] private string _maxScoreKey = "MaxScore";

    public int TargetFrameRate => _targetFrameRate;
    public string MaxScoreKey => _maxScoreKey;
}
