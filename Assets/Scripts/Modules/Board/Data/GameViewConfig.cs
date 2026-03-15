using UnityEngine;

[CreateAssetMenu(fileName = "GameViewConfig", menuName = "Trigon/Game View Config")]
public class GameViewConfig : ScriptableObject
{
    [Header("Board")]
    [SerializeField] private float tileWidth = 0.576f;
    [SerializeField] private float boardStartY = 8f;
    [SerializeField] private float boardScale = 0.5f;
    [SerializeField] private Color boardColor;
    [SerializeField] private BoardTile upTilePrefab;
    [SerializeField] private BoardTile downTilePrefab;

    [Header("Tile Visuals")]
    [SerializeField] private int topSortingOrder = 5;
    [SerializeField] private int defaultSortingOrder = 2;
    [SerializeField] private float pickupScale = 0.5f;
    [SerializeField] private float spawnScale = 0.45f;
    [SerializeField] private float dragYOffset = 1f;
    [SerializeField] private float destroyAnimDuration = 0.25f;
    [SerializeField] private Color disabledColor = new(176f / 255f, 176f / 255f, 176f / 255f, 1f);

    [Header("Spawning")]
    [SerializeField] private CompositeTile[] tilePrefabs;
    [SerializeField] private ColorPalette[] colorPalettes;

    public float TileWidth => tileWidth;
    public float BoardStartY => boardStartY;
    public float BoardScale => boardScale;
    public Color BoardColor => boardColor;
    public BoardTile UpTilePrefab => upTilePrefab;
    public BoardTile DownTilePrefab => downTilePrefab;

    public int TopSortingOrder => topSortingOrder;
    public int DefaultSortingOrder => defaultSortingOrder;
    public float PickupScale => pickupScale;
    public float SpawnScale => spawnScale;
    public float DragYOffset => dragYOffset;
    public float DestroyAnimDuration => destroyAnimDuration;
    public Color DisabledColor => disabledColor;

    public CompositeTile[] TilePrefabs => tilePrefabs;
    public ColorPalette[] ColorPalettes => colorPalettes;
}
