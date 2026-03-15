using System.Collections.Generic;
using UnityEngine;

public class CompositeTile : MonoBehaviour
{
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

    private int _topSortingOrder;
    private int _defaultSortingOrder;
    private float _pickupScale;
    private float _dragYOffset;
    private Color _disabledColor;

    public int Id { get; set; }
    public List<BaseTile> BaseTiles => baseTiles;
    public List<Position2D> TileOffsets { get; } = new();

    private void Awake()
    {
        _homePosition = transform.position;
        _cachedCamera = Camera.main;

        _dataService = ServiceLocator.Get<DataService>();
        _boardLogic = ServiceLocator.Get<BoardLogic>();
        _viewRegistry = ServiceLocator.Get<TileViewRegistry>();
        _gameManager = ServiceLocator.Get<GameManager>();

        var viewConfig = ServiceLocator.Get<ConfigService>().GameView;
        _topSortingOrder = viewConfig.TopSortingOrder;
        _defaultSortingOrder = viewConfig.DefaultSortingOrder;
        _pickupScale = viewConfig.PickupScale;
        _dragYOffset = viewConfig.DragYOffset;
        _disabledColor = viewConfig.DisabledColor;
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
        SetAllTileColors(canPlace ? _activeColor : _disabledColor);
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
        transform.localScale = new Vector2(_pickupScale, _pickupScale);
        SetSortingOrder(_topSortingOrder);
    }

    private void OnMouseDrag()
    {
        if (!CanInteract()) return;
        if (_cachedCamera == null) return;

        Vector2 worldPos = _cachedCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.y += _dragYOffset;
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
        var snappedPositions = new List<Position2D>();
        var anchorPosition = TypeConversions.ToPosition2D(baseTiles[0].transform.position);

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

        var placedCoords = new List<GridCoord>();
        for (int i = 0; i < baseTiles.Count; i++)
        {
            baseTiles[i].transform.position = TypeConversions.ToVector2(snappedPositions[i]);
            var coord = _boardLogic.PlaceTile(snappedPositions[i]);
            _viewRegistry.RegisterPlacedTileView(coord, baseTiles[i], _gameManager.TilesOnBoardZone);
            placedCoords.Add(coord);
        }

        SetSortingOrder(_defaultSortingOrder);
        _gameManager.OnTilePlacedOnBoard(this, placedCoords);
        Destroy(gameObject);
    }

    private void ResetToHome()
    {
        transform.position = _homePosition;
        transform.localScale = _homeScale;
        SetSortingOrder(_defaultSortingOrder);
    }

    private void ComputeTileOffsets()
    {
        TileOffsets.Clear();
        Vector2 rootPoint = baseTiles[0].transform.position;
        TileOffsets.Add(Position2D.Zero);

        for (int i = 1; i < baseTiles.Count; i++)
        {
            Vector2 tilePoint = baseTiles[i].transform.position;
            Vector2 delta = tilePoint - rootPoint;
            TileOffsets.Add(new Position2D(delta.x, delta.y));
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
