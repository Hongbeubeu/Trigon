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
    private float _placeholderAlpha;

    // Smooth drag
    private Vector2 _targetDragPosition;
    private bool _isDragging;
    private float _dragSpeed;

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
        _dragSpeed = viewConfig.DragSpeed;
        _placeholderAlpha = viewConfig.PlaceholderAlpha;
        _disabledColor = viewConfig.DisabledColor;
    }

    public void Initialize(int id, Vector2 scale, Color color)
    {
        Id = id;
        _homeScale = scale;
        _activeColor = color;
        _canPlace = true;
        _isDragging = false;
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
        }
    }

    private void OnMouseDrag()
    {
        if (!CanInteract() || !_isDragging) return;
        if (_cachedCamera == null) return;

        Vector2 worldPos = _cachedCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.y += _dragYOffset;
        _targetDragPosition = worldPos;

        float t = 1f - Mathf.Exp(-_dragSpeed * Time.deltaTime);
        Vector2 smoothed = Vector2.Lerp(transform.position, _targetDragPosition, t);
        transform.position = smoothed;

        UpdatePlaceholder();
    }

    private void OnMouseUp()
    {
        if (!CanInteract() || !_isDragging) return;
        _isDragging = false;
        _viewRegistry.ClearPlaceholder();
        transform.position = _targetDragPosition;
        TryPlaceTiles();
    }

    private void OnDisable()
    {
        _viewRegistry.ClearPlaceholder();
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
            var worldPos = TypeConversions.ToVector2(snappedPositions[i]);
            var coord = _boardLogic.PlaceTile(snappedPositions[i]);
            _viewRegistry.SpawnPlacedTile(coord, baseTiles[i].type, worldPos, _activeColor,
                _defaultSortingOrder, _gameManager.TilesOnBoardZone);
            placedCoords.Add(coord);
        }

        transform.DOKill();
        _gameManager.OnTilePlacedOnBoard(this, placedCoords);
        LeanPool.Despawn(gameObject);
    }

    private void ResetToHome()
    {
        _viewRegistry.ClearPlaceholder();
        transform.DOKill();
        transform.DOMove(_homePosition, ResetAnimDuration).SetEase(Ease.OutCubic);
        transform.DOScale(_homeScale, ResetAnimDuration).SetEase(Ease.OutCubic);
        SetSortingOrder(_defaultSortingOrder);
    }

    private void UpdatePlaceholder()
    {
        var snappedPositions = TryGetSnappedPositions();
        if (snappedPositions == null)
        {
            _viewRegistry.ClearPlaceholder();
            return;
        }

        var coords = new List<GridCoord>();
        foreach (var pos in snappedPositions)
        {
            coords.Add(_boardLogic.GetCoordAtPosition(pos));
        }

        var previewColor = _activeColor;
        previewColor.a = _placeholderAlpha;
        _viewRegistry.ShowPlaceholder(coords, previewColor);
    }

    private List<Position2D> TryGetSnappedPositions()
    {
        var snappedPositions = new List<Position2D>();
        var anchorPosition = TypeConversions.ToPosition2D(baseTiles[0].transform.position);

        for (int i = 0; i < TileOffsets.Count; i++)
        {
            var candidatePos = anchorPosition + TileOffsets[i];
            var snappedPos = _boardLogic.FindNearestAvailablePosition(candidatePos, baseTiles[i].type);

            if (_boardLogic.IsInvalidPosition(snappedPos))
                return null;

            snappedPositions.Add(snappedPos);

            if (i == 0)
                anchorPosition = snappedPos;
        }

        return snappedPositions;
    }

    private void ComputeTileOffsets()
    {
        TileOffsets.Clear();
        Vector2 rootPoint = baseTiles[0].transform.position;
        TileOffsets.Add(Position2D.ZERO);

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
