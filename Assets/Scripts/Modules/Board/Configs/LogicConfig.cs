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

    public int BoardRowCount => _boardRowCount;
    public int CutOffLines => _cutOffLines;
    public float SnapThreshold => _snapThreshold;
    public float ExactMatchThreshold => _exactMatchThreshold;
    public int TilesPerSpawn => _tilesPerSpawn;
}
