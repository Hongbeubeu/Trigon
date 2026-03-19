using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
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

    // Smooth drag
    private Vector2 _targetDragPosition;
    private Vector2 _dragVelocity;
    private bool _isDragging;

    private const float DragSmoothTime = 0.04f;
    private const float PickupAnimDuration = 0.15f;
    private const float ResetAnimDuration = 0.2f;

    public int Id { get; set; }
    public List<BaseTile> BaseTiles => baseTiles;
    public List<Position2D> TileOffsets { get; } = new();

    private void Awake()
    {
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
        _canPlace = true;
        _isDragging = false;
        _dragVelocity = Vector2.zero;
        _homePosition = transform.position;
        transform.localScale = scale;
        SetAllTileColors(color);
        ComputeTileOffsets();
        SetSortingOrder(_defaultSortingOrder);
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

        _isDragging = true;
        transform.DOKill();
        transform.DOScale(new Vector2(_pickupScale, _pickupScale), PickupAnimDuration)
            .SetEase(Ease.OutBack);
        SetSortingOrder(_topSortingOrder);

        if (_cachedCamera != null)
        {
            Vector2 worldPos = _cachedCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPos.y += _dragYOffset;
            _targetDragPosition = worldPos;
            _dragVelocity = Vector2.zero;
        }
    }

    private void OnMouseDrag()
    {
        if (!CanInteract() || !_isDragging) return;
        if (_cachedCamera == null) return;

        Vector2 worldPos = _cachedCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.y += _dragYOffset;
        _targetDragPosition = worldPos;

        Vector2 smoothed = Vector2.SmoothDamp(
            transform.position, _targetDragPosition, ref _dragVelocity, DragSmoothTime);
        transform.position = smoothed;
    }

    private void OnMouseUp()
    {
        if (!CanInteract() || !_isDragging) return;
        _isDragging = false;
        transform.position = _targetDragPosition;
        TryPlaceTiles();
    }

    private void OnDisable()
    {
        transform.DOKill();
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

        transform.DOKill();
        SetSortingOrder(_defaultSortingOrder);
        _gameManager.OnTilePlacedOnBoard(this, placedCoords);
        LeanPool.Detach(gameObject, true);
        Destroy(gameObject);
    }

    private void ResetToHome()
    {
        transform.DOKill();
        transform.DOMove(_homePosition, ResetAnimDuration).SetEase(Ease.OutCubic);
        transform.DOScale(_homeScale, ResetAnimDuration).SetEase(Ease.OutCubic);
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
