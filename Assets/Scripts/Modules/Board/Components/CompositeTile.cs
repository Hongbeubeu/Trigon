using System;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Dictates the interaction mode the composite tile is currently undergoing.
/// </summary>
public enum TileInteractionState
{
    Idle,
    Dragging
}

/// <summary>
/// Represents a cohesive, user-draggable shape composed of multiple BaseTiles.
/// Responsible for receiving player inputs and coordinating with BoardLogic to snap to grid positions.
/// </summary>
public class CompositeTile : MonoBehaviour
{
    #region Inspector Fields
    [Header("Structure")]
    [SerializeField] private List<BaseTile> _baseTiles = new();
    [SerializeField] private List<Vector3Int> _occupiedCoords = new();
    [SerializeField] private BaseTile _tilePrefab;
    #endregion

    #region Properties
    /// <summary>Unique identifier for this spawned composite tile instance.</summary>
    public int Id { get; private set; }
    
    /// <summary>The primitive hexagonal tiles that make up this composite shape.</summary>
    public IReadOnlyList<BaseTile> BaseTiles => _baseTiles;
    
    /// <summary>The relative grid coordinate offsets from the root tile (index 0).</summary>
    public List<GridCoord> GridOffsets { get; } = new();
    #endregion

    #region Private State
    private TileInteractionState _interactionState = TileInteractionState.Idle;
    private bool _canPlace = true;
    private Vector2 _homePosition;
    private Vector2 _homeScale;
    private Color _activeColor;
    private Vector2 _targetDragPosition;
    
    // Dependencies & Components
    private Camera _camera;
    private IDataService _dataService;
    private IBoardLogic _boardLogic;
    private ITileViewRegistry _viewRegistry;
    private GameManager _gameManager;
    
    // Config Caches
    private int _topSortingOrder;
    private int _defaultSortingOrder;
    private float _pickupScale;
    private float _dragYOffset;
    private float _dragSpeed;
    private float _placeholderAlpha;
    private Color _disabledColor;
    private float _tileWidth;
    
    // Constants
    private const float PICKUP_ANIM_DURATION = 0.15f;
    private const float RESET_ANIM_DURATION = 0.2f;
    #endregion

    #region Unity Lifecycle & Initialization
    private void Awake()
    {
        _camera = Camera.main;
        _dataService = ServiceLocator.Get<IDataService>();
        _boardLogic = ServiceLocator.Get<IBoardLogic>();
        _viewRegistry = ServiceLocator.Get<ITileViewRegistry>();
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

    /// <summary>
    /// Bootstraps the tile visual and logical defaults upon spawning.
    /// </summary>
    public void Initialize(int id, Vector2 scale, Color color)
    {
        Id = id;
        _homeScale = scale;
        _activeColor = color;
        _canPlace = true;
        _interactionState = TileInteractionState.Idle;
        _homePosition = transform.position;
        transform.localScale = scale;
        
        SetColor(color);
        CalculateStructuralOffsets();
        SetSortingOrder(_defaultSortingOrder);
    }

    private void OnDisable()
    {
        _viewRegistry?.ClearPlaceholder();
        transform.DOKill();
    }

    /// <summary>
    /// Forces the underlying base tiles to be dismantled completely.
    /// </summary>
    public void DestroyTile()
    {
        foreach (var tile in _baseTiles)
        {
            if (tile != null) tile.Destroy();
        }
    }
    #endregion

    #region Input Event Routing
    private void OnMouseDown() => BeginDrag();
    private void OnMouseDrag() => UpdateDrag();
    private void OnMouseUp() => EndDrag();
    #endregion

    #region Drag Interaction Logic
    /// <summary>Starts scaling animation and prepares for dragging across the screen.</summary>
    private void BeginDrag()
    {
        if (!CanInteract()) return;
        
        _interactionState = TileInteractionState.Dragging;
        
        transform.DOKill();
        transform.DOScale(new Vector2(_pickupScale, _pickupScale), PICKUP_ANIM_DURATION)
                 .SetEase(Ease.OutBack);
        
        SetSortingOrder(_topSortingOrder);
        UpdateTargetDragPosition();
    }

    /// <summary>Lerps the composite shape smoothly toward the mouse cursor.</summary>
    private void UpdateDrag()
    {
        if (!CanInteract() || _interactionState != TileInteractionState.Dragging) return;
        
        UpdateTargetDragPosition();
        
        var t = 1f - Mathf.Exp(-_dragSpeed * Time.deltaTime);
        transform.position = Vector2.Lerp(transform.position, _targetDragPosition, t);
        
        UpdatePlaceholderVisuals();
    }

    /// <summary>Evaluates placement mathematical viability and delegates execution if valid.</summary>
    private void EndDrag()
    {
        if (!CanInteract() || _interactionState != TileInteractionState.Dragging) return;
        
        _interactionState = TileInteractionState.Idle;
        _viewRegistry.ClearPlaceholder();
        transform.position = _targetDragPosition;
        
        AttemptPlacementOnBoard();
    }

    private bool CanInteract()
    {
        return _canPlace && _dataService.Session.State == GameState.Playing;
    }

    private void UpdateTargetDragPosition()
    {
        if (_camera == null) return;
        var worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.y += _dragYOffset;
        _targetDragPosition = worldPos;
    }
    #endregion

    #region Board Placement Mathematics
    /// <summary>
    /// Determines if the current drag position perfectly aligns with valid board grid coords.
    /// If successful, instructs BoardLogic to place tiles and Game Manager to route score.
    /// </summary>
    private void AttemptPlacementOnBoard()
    {
        var snappedPositions = CalculateValidPlacementPositions();
        
        if (snappedPositions == null)
        {
            AnimateReturnToOrigin();
            return;
        }
        
        var placedCoords = new List<GridCoord>();
        
        for (var i = 0; i < _baseTiles.Count; i++)
        {
            var worldPos = TypeConversions.ToVector2(snappedPositions[i]);
            var coord = _boardLogic.PlaceTile(snappedPositions[i]);
            
            _viewRegistry.SpawnPlacedTile(coord, _baseTiles[i].type, worldPos, _activeColor, _defaultSortingOrder, _gameManager.TilesOnBoardZone);
            placedCoords.Add(coord);
        }
        
        transform.DOKill();
        _gameManager.OnTilePlacedOnBoard(this, placedCoords);
        LeanPool.Despawn(gameObject);
    }

    /// <summary>
    /// Returns the exact physical world space points per BaseTile if the layout flawlessly fits the grid. Null otherwise.
    /// </summary>
    private List<Position2D> CalculateValidPlacementPositions()
    {
        if (GridOffsets.Count != _baseTiles.Count) return null;

        var snappedPositions = new List<Position2D>();
        var rootPos = TypeConversions.ToPosition2D(_baseTiles[0].transform.position);
        var rootSnappedPos = _boardLogic.FindNearestAvailablePosition(rootPos, _baseTiles[0].type);
        
        if (_boardLogic.IsInvalidPosition(rootSnappedPos)) return null;

        var rootCoord = _boardLogic.GetCoordAtPosition(rootSnappedPos);
        snappedPositions.Add(rootSnappedPos);

        for (var i = 1; i < _baseTiles.Count; i++)
        {
            var offset = GridOffsets[i];
            var targetCoord = new GridCoord(rootCoord.x + offset.x, rootCoord.y + offset.y, rootCoord.z + offset.z);
            var targetCell = _dataService.Board.GetTile(targetCoord);
            
            if (targetCell == null || targetCell.IsOccupied) return null;
            
            snappedPositions.Add(targetCell.WorldPosition);
        }

        return snappedPositions;
    }

    /// <summary>Pre-generates purely mathematical topological offsets to test fitting permutations dynamically.</summary>
    private void CalculateStructuralOffsets()
    {
        GridOffsets.Clear();
        GridOffsets.Add(new GridCoord(0, 0, 0));

        var rootOccupied = _occupiedCoords.Count > 0 ? TypeConversions.ToGridCoord(_occupiedCoords[0]) : new GridCoord(0,0,0);

        for (var i = 1; i < _baseTiles.Count; i++)
        {
            if (i < _occupiedCoords.Count)
            {
                var coord = TypeConversions.ToGridCoord(_occupiedCoords[i]);
                GridOffsets.Add(new GridCoord(coord.x - rootOccupied.x, coord.y - rootOccupied.y, coord.z - rootOccupied.z));
            }
        }
    }
    #endregion

    #region Visual States & Feedback
    /// <summary>Toggles physical appearance identifying if the tile is legally acceptable anywhere on the board.</summary>
    public void SetPlaceable(bool canPlace)
    {
        if (_canPlace == canPlace) return;
        _canPlace = canPlace;
        SetColor(canPlace ? _activeColor : _disabledColor);
    }

    /// <summary>Smoothly glides the shape back to the UI spawn rack.</summary>
    private void AnimateReturnToOrigin()
    {
        _viewRegistry.ClearPlaceholder();
        transform.DOKill();
        transform.DOMove(_homePosition, RESET_ANIM_DURATION).SetEase(Ease.OutCubic);
        transform.DOScale(_homeScale, RESET_ANIM_DURATION).SetEase(Ease.OutCubic);
        SetSortingOrder(_defaultSortingOrder);
    }

    /// <summary>Signals the TileViewRegistry to render a faint ghost-shadow under legally snapping boards.</summary>
    private void UpdatePlaceholderVisuals()
    {
        var snappedPositions = CalculateValidPlacementPositions();
        
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

    private void SetColor(Color color)
    {
        foreach (var tile in _baseTiles) tile.SetColor(color);
    }

    private void SetSortingOrder(int order)
    {
        foreach (var tile in _baseTiles) tile.SetSortingOrder(order);
    }
    #endregion

#if UNITY_EDITOR
    #region Editor Tools
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
    #endregion
#endif
}