using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using UnityEditor;
using UnityEngine;

public class CompositeTile : MonoBehaviour
{
    [SerializeField] private List<BaseTile> _baseTiles = new();
    [SerializeField] private List<Vector3Int> _occupiedCoords = new();
    [SerializeField] private BaseTile _tilePrefab;
    public int Id { get; private set; }
    public List<BaseTile> BaseTiles => _baseTiles;
    public List<Position2D> TileOffsets { get; } = new();
    private Vector2 _homePosition;
    private Vector2 _homeScale;
    private Color _activeColor;
    private bool _canPlace = true;
    private Camera _camera;
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
    private Vector2 _targetDragPosition;
    private bool _isDragging;
    private float _dragSpeed;
    private float _tileWidth;
    private const float PICKUP_ANIM_DURATION = 0.15f;
    private const float RESET_ANIM_DURATION = 0.2f;

    private void Awake()
    {
        _camera = Camera.main;
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
        SetColor(color);
        ComputeTileOffsets();
        SetSortingOrder(_defaultSortingOrder);
    }

#if UNITY_EDITOR
    [ContextMenu("Generate Pattern")]
    private void GeneratePattern()
    {
        CleanTiles();
        foreach (var vector3Int in _occupiedCoords)
        {
            var tile = PrefabUtility.InstantiatePrefab(_tilePrefab, transform) as BaseTile;
            if (tile == null) throw new NullReferenceException($"{nameof(CompositeTile)} Null {nameof(_tilePrefab)}");
            _baseTiles.Add(tile);
            
            var gridCoord = TypeConversions.ToGridCoord(vector3Int);
            tile.transform.localPosition = gridCoord.ToVector2(out var isUpTile);
            tile.transform.rotation = isUpTile ? Quaternion.identity : Quaternion.Euler(0, 0, 180f);
            tile.type = isUpTile ? TypeTile.Up : TypeTile.Down;
        }
        FitShapeToCenter();
    }

    private void FitShapeToCenter()
    {
        var tileHeight = HexGridExtensions.GetTileHeight();
        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(float.MinValue, float.MinValue);
        foreach (var tile in _baseTiles)
        {
            var position = tile.transform.localPosition;
            var isUpTile = tile.type == TypeTile.Up;
            var upOffset = isUpTile ? tileHeight * 2 / 3f : tileHeight / 3f;
            var downOffset = isUpTile ? tileHeight / 3f : tileHeight * 2 / 3f;
            if (position.x - _tileWidth / 2f < min.x) min.x = position.x - _tileWidth / 2f;
            if (position.y - downOffset < min.y) min.y = position.y - downOffset;
            if (position.x + _tileWidth / 2f > max.x) max.x = position.x + _tileWidth / 2f;
            if (position.y + upOffset > max.y) max.y = position.y + upOffset;
        }
        var center = new Vector3((min.x + max.x) / 2f, (min.y + max.y) / 2f, 0f);
        foreach (var tile in _baseTiles)
        {
            var newPosition = tile.transform.position - center;
            tile.transform.localPosition = newPosition;
        }
    }

    [ContextMenu("Clear Tiles")]
    private void CleanTiles()
    {
        if (_baseTiles.Count <= 0) return;
        foreach (var tile in _baseTiles)
        {
            DestroyImmediate(tile.gameObject);
        }
        _baseTiles.Clear();
    }
#endif

    public void SetPlaceable(bool canPlace)
    {
        if (_canPlace == canPlace) return;
        _canPlace = canPlace;
        SetColor(canPlace ? _activeColor : _disabledColor);
    }

    public void DestroyTile()
    {
        foreach (var tile in _baseTiles)
        {
            if (tile != null) tile.Destroy();
        }
    }

    private void OnMouseDown()
    {
        if (!CanInteract()) return;
        _isDragging = true;
        transform.DOKill();
        transform.DOScale(new Vector2(_pickupScale, _pickupScale), PICKUP_ANIM_DURATION)
                 .SetEase(Ease.OutBack);
        SetSortingOrder(_topSortingOrder);
        if (_camera == null) return;
        Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.y += _dragYOffset;
        _targetDragPosition = worldPos;
    }

    private void OnMouseDrag()
    {
        if (!CanInteract() || !_isDragging) return;
        if (_camera == null) return;
        Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
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
        var anchorPosition = TypeConversions.ToPosition2D(_baseTiles[0].transform.position);
        for (int i = 0; i < TileOffsets.Count; i++)
        {
            var candidatePos = anchorPosition + TileOffsets[i];
            var snappedPos = _boardLogic.FindNearestAvailablePosition(candidatePos, _baseTiles[i].type);
            if (_boardLogic.IsInvalidPosition(snappedPos))
            {
                ResetToHome();
                return;
            }
            snappedPositions.Add(snappedPos);
            if (i == 0) anchorPosition = snappedPos;
        }
        var placedCoords = new List<GridCoord>();
        for (int i = 0; i < _baseTiles.Count; i++)
        {
            var worldPos = TypeConversions.ToVector2(snappedPositions[i]);
            var coord = _boardLogic.PlaceTile(snappedPositions[i]);
            _viewRegistry.SpawnPlacedTile(coord, _baseTiles[i].type, worldPos, _activeColor,
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
        transform.DOMove(_homePosition, RESET_ANIM_DURATION).SetEase(Ease.OutCubic);
        transform.DOScale(_homeScale, RESET_ANIM_DURATION).SetEase(Ease.OutCubic);
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
        var anchorPosition = TypeConversions.ToPosition2D(_baseTiles[0].transform.position);
        for (var i = 0; i < TileOffsets.Count; i++)
        {
            var candidatePos = anchorPosition + TileOffsets[i];
            var snappedPos = _boardLogic.FindNearestAvailablePosition(candidatePos, _baseTiles[i].type);
            if (_boardLogic.IsInvalidPosition(snappedPos)) return null;
            snappedPositions.Add(snappedPos);
            if (i == 0) anchorPosition = snappedPos;
        }
        return snappedPositions;
    }

    private void ComputeTileOffsets()
    {
        TileOffsets.Clear();
        Vector2 rootPoint = _baseTiles[0].transform.position;
        TileOffsets.Add(Position2D.ZERO);
        for (var i = 1; i < _baseTiles.Count; i++)
        {
            Vector2 tilePoint = _baseTiles[i].transform.position;
            var delta = tilePoint - rootPoint;
            TileOffsets.Add(new Position2D(delta.x, delta.y));
        }
    }

    private void SetColor(Color color)
    {
        foreach (var tile in _baseTiles)
        {
            tile.SetColor(color);
        }
    }

    private void SetSortingOrder(int order)
    {
        foreach (var tile in _baseTiles)
        {
            tile.SetSortingOrder(order);
        }
    }
}