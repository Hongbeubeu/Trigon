using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileViewRegistry
{
    private readonly float _clearTileDelay;
    private readonly float _destroyAnimDuration;

    private readonly Dictionary<GridCoord, BoardTile> _boardTileViews = new();
    private readonly Dictionary<GridCoord, BaseTile> _placedTileViews = new();
    private readonly Dictionary<int, CompositeTile> _spawnedTileViews = new();

    public IReadOnlyDictionary<int, CompositeTile> SpawnedTiles => _spawnedTileViews;

    public TileViewRegistry(LogicConfig logicConfig, GameViewConfig viewConfig)
    {
        _clearTileDelay = logicConfig.ClearTileDelay;
        _destroyAnimDuration = viewConfig.DestroyAnimDuration;

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

    public void RegisterPlacedTileView(GridCoord coord, BaseTile view, Transform parent)
    {
        _placedTileViews[coord] = view;
        view.transform.SetParent(parent);
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
            tile.DestroyAnim(_destroyAnimDuration);
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

    public void DestroyAllPlacedTileViews()
    {
        foreach (var kvp in _placedTileViews)
        {
            if (kvp.Value != null)
                kvp.Value.Destroy();
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

    public void SyncWorldPositions(BoardData boardData)
    {
        foreach (var kvp in _boardTileViews)
        {
            var cell = boardData.GetCell(kvp.Key);
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
