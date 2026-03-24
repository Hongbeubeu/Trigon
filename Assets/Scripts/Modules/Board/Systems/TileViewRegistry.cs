using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class TileViewRegistry
{
    private readonly float _clearTileDelay;
    private readonly float _destroyAnimDuration;
    private readonly Color _boardColor;
    private readonly float _placedTileScale;
    private readonly BaseTile _placedTilePrefab;

    private readonly Dictionary<GridCoord, BoardTile> _boardTileViews = new();
    private readonly Dictionary<GridCoord, BaseTile> _placedTileViews = new();
    private readonly Dictionary<int, CompositeTile> _spawnedTileViews = new();
    private readonly List<GridCoord> _currentPlaceholderCoords = new();

    public IReadOnlyDictionary<int, CompositeTile> SpawnedTiles => _spawnedTileViews;

    public TileViewRegistry(LogicConfig logicConfig, GameViewConfig viewConfig)
    {
        _clearTileDelay = logicConfig.ClearTileDelay;
        _destroyAnimDuration = viewConfig.DestroyAnimDuration;
        _boardColor = viewConfig.BoardColor;
        _placedTileScale = viewConfig.PlacedTileScale;
        _placedTilePrefab = viewConfig.PlacedTilePrefab;

        GameEvents.OnGameStateChanged += OnGameStateChanged;
    }

    public void Dispose()
    {
        GameEvents.OnGameStateChanged -= OnGameStateChanged;
    }

    public void RegisterBoardTileView(GridCoord coord, BoardTile view)
    {
        _boardTileViews[coord] = view;
    }

    public void SpawnPlacedTile(GridCoord coord, TypeTile type, Vector2 position, Color color, int sortingOrder, Transform parent)
    {
        var tile = LeanPool.Spawn(_placedTilePrefab, position, Quaternion.identity, parent);
        tile.transform.localScale = Vector3.one * _placedTileScale;
        tile.SetColor(color);
        tile.SetSortingOrder(sortingOrder);
        tile.type = type;
        tile.transform.rotation = type == TypeTile.Up ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
        _placedTileViews[coord] = tile;
    }

    public void RegisterSpawnedTile(int id, CompositeTile view)
    {
        _spawnedTileViews[id] = view;
    }

    public void RemoveSpawnedTile(int id)
    {
        _spawnedTileViews.Remove(id);
    }

    public void AnimateRemovePlacedTile(GridCoord coord)
    {
        if (_placedTileViews.TryGetValue(coord, out var tile))
        {
            tile.DespawnAnim(_destroyAnimDuration);
        }

        _placedTileViews.Remove(coord);
    }

    public IEnumerator AnimateClearLine(List<GridCoord> line, BoardLogic boardLogic)
    {
        foreach (var coord in line)
        {
            boardLogic.RemoveTile(coord);
            AnimateRemovePlacedTile(coord);
            yield return new WaitForSeconds(_clearTileDelay);
        }
    }

    public void DespawnAllPlacedTileViews()
    {
        foreach (var kvp in _placedTileViews)
        {
            if (kvp.Value != null)
                LeanPool.Despawn(kvp.Value);
        }

        _placedTileViews.Clear();
    }

    public void ClearSpawnedTiles()
    {
        _spawnedTileViews.Clear();
    }

    public void ClearPlacedTiles()
    {
        _placedTileViews.Clear();
    }

    public void ShowPlaceholder(List<GridCoord> coords, Color color)
    {
        ClearPlaceholder();

        foreach (var coord in coords)
        {
            if (_boardTileViews.TryGetValue(coord, out var boardTile))
            {
                boardTile.SpriteRenderer.color = color;
                _currentPlaceholderCoords.Add(coord);
            }
        }
    }

    public void ClearPlaceholder()
    {
        foreach (var coord in _currentPlaceholderCoords)
        {
            if (_boardTileViews.TryGetValue(coord, out var boardTile))
            {
                boardTile.SpriteRenderer.color = _boardColor;
            }
        }

        _currentPlaceholderCoords.Clear();
    }

    public void SyncWorldPositions(BoardData boardData)
    {
        foreach (var kvp in _boardTileViews)
        {
            var cell = boardData.GetTile(kvp.Key);
            if (cell != null)
            {
                cell.WorldPosition = TypeConversions.ToPosition2D(kvp.Value.transform.position);
            }
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Lost)
            SetAllPlacedTilesToLoseColor();
    }

    private void SetAllPlacedTilesToLoseColor()
    {
        foreach (var kvp in _placedTileViews)
        {
            kvp.Value.SetLoseColor();
        }
    }
}
