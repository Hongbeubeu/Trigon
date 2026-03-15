using System.Collections.Generic;
using UnityEngine;

public class CompositeTile : MonoBehaviour
{
    private const int TOP_SORTING_ORDER = 5;
    private const int DEFAULT_SORTING_ORDER = 2;
    private const float PICKUP_SCALE = 0.5f;
    private const float DRAG_Y_OFFSET = 1f;
    private static readonly Color DISABLED_COLOR = new(176f / 255f, 176f / 255f, 176f / 255f, 1f);

    [SerializeField] private List<BaseTile> baseTiles = new();

    private Vector2 _homePosition;
    private Vector2 _homeScale;
    private Color _activeColor;
    private bool _canPlace = true;
    private Camera _cachedCamera;
    private DataService _dataService;
    private BoardLogic _boardLogic;
    private TileViewRegistry _viewRegistry;
    private GameManager _gameManager;

    public int Id { get; set; }
    public List<BaseTile> BaseTiles => baseTiles;
    public List<Vector2> TileOffsets { get; } = new();

    private void Awake()
    {
        _homePosition = transform.position;
        _cachedCamera = Camera.main;
        _dataService = ServiceLocator.Get<DataService>();
        _boardLogic = ServiceLocator.Get<BoardLogic>();
        _viewRegistry = ServiceLocator.Get<TileViewRegistry>();
        _gameManager = ServiceLocator.Get<GameManager>();
    }

    public void Initialize(int id, Vector2 scale, Color color)
    {
        Id = id;
        _homeScale = scale;
        _activeColor = color;
        transform.localScale = scale;
        SetAllTileColors(color);
        ComputeTileOffsets();
    }

    public void SetPlaceable(bool canPlace)
    {
        if (_canPlace == canPlace) return;
        _canPlace = canPlace;
        SetAllTileColors(canPlace ? _activeColor : DISABLED_COLOR);
    }

    public void DestroyTile()
    {
        foreach (var tile in baseTiles)
        {
            if (tile != null)
                tile.Destroy();
        }
    }

    private void OnMouseDown()
    {
        if (!CanInteract()) return;
        transform.localScale = new Vector2(PICKUP_SCALE, PICKUP_SCALE);
        SetSortingOrder(TOP_SORTING_ORDER);
    }

    private void OnMouseDrag()
    {
        if (!CanInteract()) return;
        if (_cachedCamera == null) return;

        Vector2 worldPos = _cachedCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.y += DRAG_Y_OFFSET;
        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        if (!CanInteract()) return;
        TryPlaceTiles();
    }

    private bool CanInteract()
    {
        return _canPlace && _dataService.Session.State == GameState.Playing;
    }

    private void TryPlaceTiles()
    {
        var snappedPositions = new List<Vector2>();
        Vector2 anchorPosition = baseTiles[0].transform.position;

        for (int i = 0; i < TileOffsets.Count; i++)
        {
            var candidatePos = anchorPosition + TileOffsets[i];
            var snappedPos = _boardLogic.FindNearestAvailablePosition(candidatePos, baseTiles[i].type);

            if (_boardLogic.IsInvalidPosition(snappedPos))
            {
                ResetToHome();
                return;
            }

            snappedPositions.Add(snappedPos);

            if (i == 0)
                anchorPosition = snappedPos;
        }

        var placedCoords = new List<Vector3Int>();
        for (int i = 0; i < baseTiles.Count; i++)
        {
            baseTiles[i].transform.position = snappedPositions[i];
            var coord = _boardLogic.PlaceTile(snappedPositions[i]);
            _viewRegistry.RegisterPlacedTileView(coord, baseTiles[i], _gameManager.TilesOnBoardZone);
            placedCoords.Add(coord);
        }

        SetSortingOrder(DEFAULT_SORTING_ORDER);
        _gameManager.OnTilePlacedOnBoard(this, placedCoords);
        Destroy(gameObject);
    }

    private void ResetToHome()
    {
        transform.position = _homePosition;
        transform.localScale = _homeScale;
        SetSortingOrder(DEFAULT_SORTING_ORDER);
    }

    private void ComputeTileOffsets()
    {
        TileOffsets.Clear();
        Vector2 rootPoint = baseTiles[0].transform.position;
        TileOffsets.Add(Vector2.zero);

        for (int i = 1; i < baseTiles.Count; i++)
        {
            Vector2 tilePoint = baseTiles[i].transform.position;
            TileOffsets.Add(tilePoint - rootPoint);
        }
    }

    private void SetAllTileColors(Color color)
    {
        foreach (var tile in baseTiles)
        {
            tile.SetColor(color);
        }
    }

    private void SetSortingOrder(int order)
    {
        foreach (var tile in baseTiles)
        {
            tile.SetSortingOrder(order);
        }
    }
}
