using UnityEngine;

[CreateAssetMenu(fileName = "GameViewConfig", menuName = "Trigon/Game View Config")]
public class GameViewConfig : ScriptableObject
{
    [Header("Board")]
    [SerializeField] private float _tileWidth = 1f;
    [SerializeField] private float _boardScale = 1f;
    [SerializeField] private Color _boardColor;
    [SerializeField] private BoardTile _boardTilePrefab;

    [Header("Tile Visuals")]
    [SerializeField] private int _topSortingOrder = 5;
    [SerializeField] private int _defaultSortingOrder = 2;
    [SerializeField] private float _pickupScale = 0.5f;
    [SerializeField] private float _spawnScale = 0.45f;
    [SerializeField] private float _dragYOffset = 2f;
    [SerializeField] [Range(10f, 100f)] private float _dragSpeed = 30f;
    [SerializeField] private float _destroyAnimDuration = 0.25f;
    [SerializeField] [Range(0f, 1f)] private float _placeholderAlpha = 0.35f;
    [SerializeField] private Color _disabledColor = new(176f / 255f, 176f / 255f, 176f / 255f, 1f);

    [Header("Placed Tiles")]
    [SerializeField] private float _placedTileScale = 1f;
    [SerializeField] private BaseTile _placedTilePrefab;

    [Header("Spawning")]
    [SerializeField] private CompositeTile[] _tilePrefabs;
    [SerializeField] private ColorPalette[] _colorPalettes;

    public float TileWidth => _tileWidth;
    public float BoardScale => _boardScale;
    public Color BoardColor => _boardColor;
    public BoardTile BoardTilePrefab => _boardTilePrefab;
    public int TopSortingOrder => _topSortingOrder;
    public int DefaultSortingOrder => _defaultSortingOrder;
    public float PickupScale => _pickupScale;
    public float SpawnScale => _spawnScale;
    public float DragYOffset => _dragYOffset;
    public float DragSpeed => _dragSpeed;
    public float DestroyAnimDuration => _destroyAnimDuration;
    public float PlaceholderAlpha => _placeholderAlpha;
    public Color DisabledColor => _disabledColor;
    public float PlacedTileScale => _placedTileScale;
    public BaseTile PlacedTilePrefab => _placedTilePrefab;
    public CompositeTile[] TilePrefabs => _tilePrefabs;
    public ColorPalette[] ColorPalettes => _colorPalettes;
}
