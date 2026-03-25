using UnityEngine;

[CreateAssetMenu(fileName = "LogicConfig", menuName = "Trigon/Logic Config")]
public class LogicConfig : ScriptableObject
{
    [Header("Board Rules")]
    [SerializeField] private int _boardRowCount = 12;
    [SerializeField] private int _cutOffLines = 4;
    [SerializeField] private float _snapThreshold = 0.3f;
    [SerializeField] private float _exactMatchThreshold = 0.01f;

    [Header("Gameplay")]
    [SerializeField] private int _tilesPerSpawn = 3;
    [SerializeField] private int _targetFrameRate = 120;
    [SerializeField] private float _lineClearSettleDelay = 0.26f;
    [SerializeField] private float _clearTileDelay = 0.01f;

    [Header("Persistence")]
    [SerializeField] private string _maxScoreKey = "MaxScore";

    public int BoardRowCount => _boardRowCount;
    public int CutOffLines => _cutOffLines;
    public float SnapThreshold => _snapThreshold;
    public float ExactMatchThreshold => _exactMatchThreshold;
    public int TilesPerSpawn => _tilesPerSpawn;
    public int TargetFrameRate => _targetFrameRate;
    public float LineClearSettleDelay => _lineClearSettleDelay;
    public float ClearTileDelay => _clearTileDelay;
    public string MaxScoreKey => _maxScoreKey;
}
