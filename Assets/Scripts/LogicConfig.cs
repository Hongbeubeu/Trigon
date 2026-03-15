using UnityEngine;

[CreateAssetMenu(fileName = "LogicConfig", menuName = "Trigon/Logic Config")]
public class LogicConfig : ScriptableObject
{
    [Header("Board Rules")]
    [SerializeField] private int boardRowCount = 12;
    [SerializeField] private int boardMinAxisValue = 4;
    [SerializeField] private float snapThreshold = 0.25f;
    [SerializeField] private float exactMatchThreshold = 0.01f;

    [Header("Gameplay")]
    [SerializeField] private int tilesPerSpawn = 3;
    [SerializeField] private int targetFrameRate = 120;
    [SerializeField] private float lineClearSettleDelay = 0.26f;
    [SerializeField] private float clearTileDelay = 0.01f;

    [Header("Persistence")]
    [SerializeField] private string maxScoreKey = "MaxScore";

    public int BoardRowCount => boardRowCount;
    public int BoardMinAxisValue => boardMinAxisValue;
    public float SnapThreshold => snapThreshold;
    public float ExactMatchThreshold => exactMatchThreshold;
    public int TilesPerSpawn => tilesPerSpawn;
    public int TargetFrameRate => targetFrameRate;
    public float LineClearSettleDelay => lineClearSettleDelay;
    public float ClearTileDelay => clearTileDelay;
    public string MaxScoreKey => maxScoreKey;
}
