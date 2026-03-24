using UnityEngine;

[CreateAssetMenu(fileName = "GameViewConfig", menuName = "Trigon/Game View Config")]
public class GameViewConfig : ScriptableObject
{
    [Header("Board")]
    [SerializeField] private float _tileWidth = 1f;
    [SerializeField] private float _boardScale = 1f;
    [SerializeField] private Color boardColor;
    [SerializeField] private BoardTile _boardTilePrefab;

    [Header("Tile Visuals")]
    [SerializeField] private int topSortingOrder = 5;
    [SerializeField] private int defaultSortingOrder = 2;
    [SerializeField] private float pickupScale = 0.5f;
    [SerializeField] private float spawnScale = 0.45f;
    [SerializeField] private float dragYOffset = 1f;
    [SerializeField] [Range(10f, 100f)] private float dragSpeed = 30f;
    [SerializeField] private float destroyAnimDuration = 0.25f;
    [SerializeField] [Range(0f, 1f)] private float placeholderAlpha = 0.35f;
    [SerializeField] private Color disabledColor = new(176f / 255f, 176f / 255f, 176f / 255f, 1f);

    [Header("Placed Tiles")]
    [SerializeField] private float _placedTileScale = 1f;
    [SerializeField] private BaseTile _placedTilePrefab;

    [Header("Spawning")]
    [SerializeField] private CompositeTile[] tilePrefabs;
    [SerializeField] private ColorPalette[] colorPalettes;

    public float TileWidth => _tileWidth;
    public float BoardScale => _boardScale;
    public Color BoardColor => boardColor;
    public BoardTile BoardTilePrefab => _boardTilePrefab;

    public int TopSortingOrder => topSortingOrder;
    public int DefaultSortingOrder => defaultSortingOrder;
    public float PickupScale => pickupScale;
    public float SpawnScale => spawnScale;
    public float DragYOffset => dragYOffset;
    public float DragSpeed => dragSpeed;
    public float DestroyAnimDuration => destroyAnimDuration;
    public float PlaceholderAlpha => placeholderAlpha;
    public Color DisabledColor => disabledColor;

    public float PlacedTileScale => _placedTileScale;
    public BaseTile PlacedTilePrefab => _placedTilePrefab;

    public CompositeTile[] TilePrefabs => tilePrefabs;
    public ColorPalette[] ColorPalettes => colorPalettes;
}
