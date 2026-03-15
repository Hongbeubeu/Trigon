using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileViewRegistry
{
    private const float CLEAR_TILE_DELAY = 0.01f;

    private readonly Dictionary<Vector3Int, BoardTile> _boardTileViews = new();
    private readonly Dictionary<Vector3Int, BaseTile> _placedTileViews = new();
    private readonly Dictionary<int, CompositeTile> _spawnedTileViews = new();

    public IReadOnlyDictionary<int, CompositeTile> SpawnedTiles => _spawnedTileViews;

    public void RegisterBoardTileView(Vector3Int coord, BoardTile view)
    {
        _boardTileViews[coord] = view;
    }

    public void RegisterPlacedTileView(Vector3Int coord, BaseTile view, Transform parent)
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

    public void AnimateRemovePlacedTile(Vector3Int coord)
    {
        if (_placedTileViews.TryGetValue(coord, out var tile))
        {
            tile.DestroyAnim();
        }

        _placedTileViews.Remove(coord);
    }

    public IEnumerator AnimateClearLine(List<Vector3Int> line, BoardLogic boardLogic)
    {
        foreach (var coord in line)
        {
            boardLogic.RemoveTile(coord);
            AnimateRemovePlacedTile(coord);
            yield return new WaitForSeconds(CLEAR_TILE_DELAY);
        }
    }

    public void SetAllPlacedTilesToLoseColor()
    {
        foreach (var kvp in _placedTileViews)
        {
            kvp.Value.SetLoseColor();
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
                cell.WorldPosition = (Vector2)kvp.Value.transform.position;
            }
        }
    }
}
