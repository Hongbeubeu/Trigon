using System.Collections.Generic;
using UnityEngine;

public class BoardState
{
    private const float SNAP_THRESHOLD = 0.25f;
    private const float EXACT_THRESHOLD = 0.01f;
    private static readonly Vector2 INVALID_POSITION = new(-100f, 0f);

    private readonly Dictionary<Vector3Int, BoardTile> _boardTiles = new();
    private readonly Dictionary<Vector2, Vector3Int> _positionToCoord = new();
    private readonly Dictionary<Vector3Int, BaseTile> _placedTiles = new();

    public IReadOnlyDictionary<Vector3Int, BoardTile> BoardTiles => _boardTiles;

    public void RegisterTile(Vector3Int coord, BoardTile tile)
    {
        _boardTiles[coord] = tile;
    }

    public void BuildPositionMapping()
    {
        _positionToCoord.Clear();
        foreach (var kvp in _boardTiles)
        {
            _positionToCoord[(Vector2)kvp.Value.transform.position] = kvp.Key;
        }
    }

    public Vector2 FindNearestAvailablePosition(Vector2 position, TypeTile type)
    {
        foreach (var kvp in _boardTiles)
        {
            var boardTile = kvp.Value;
            if (boardTile.type != type || boardTile.isContainsTile) continue;

            Vector2 tilePos = boardTile.transform.position;
            if (Mathf.Abs(tilePos.x - position.x) < SNAP_THRESHOLD &&
                Mathf.Abs(tilePos.y - position.y) < SNAP_THRESHOLD)
            {
                return tilePos;
            }
        }

        return INVALID_POSITION;
    }

    public bool IsInvalidPosition(Vector2 position)
    {
        return Mathf.Approximately(position.x, INVALID_POSITION.x) &&
               Mathf.Approximately(position.y, INVALID_POSITION.y);
    }

    public Vector3Int GetCoordAtPosition(Vector2 position)
    {
        foreach (var kvp in _positionToCoord)
        {
            if (Mathf.Abs(kvp.Key.x - position.x) < EXACT_THRESHOLD &&
                Mathf.Abs(kvp.Key.y - position.y) < EXACT_THRESHOLD)
            {
                return kvp.Value;
            }
        }

        return new Vector3Int(-100, 0, 0);
    }

    public Vector3Int PlaceTile(BaseTile tile, Transform parent)
    {
        Vector2 position = tile.transform.position;
        var coord = GetCoordAtPosition(position);
        _placedTiles[coord] = tile;
        _boardTiles[coord].isContainsTile = true;
        tile.transform.SetParent(parent);
        return coord;
    }

    public void RemoveTileAt(Vector3Int coord)
    {
        if (_boardTiles.TryGetValue(coord, out var boardTile))
        {
            boardTile.isContainsTile = false;
        }

        if (_placedTiles.TryGetValue(coord, out var baseTile))
        {
            baseTile.DestroyAnim();
        }

        _placedTiles.Remove(coord);
    }

    public void Reset()
    {
        _placedTiles.Clear();
        foreach (var kvp in _boardTiles)
        {
            kvp.Value.isContainsTile = false;
        }
    }

    public void DestroyAllPlacedTiles()
    {
        foreach (var kvp in _placedTiles)
        {
            if (kvp.Value != null)
                kvp.Value.Destroy();
        }

        _placedTiles.Clear();
    }

    public void SetAllTilesToLoseColor()
    {
        foreach (var kvp in _placedTiles)
        {
            kvp.Value.SetLoseColor();
        }
    }

    public bool CanFitCompositeTile(CompositeTile composite)
    {
        var rootType = composite.BaseTiles[0].type;

        foreach (var kvp in _boardTiles)
        {
            var boardTile = kvp.Value;
            if (boardTile.isContainsTile || rootType != boardTile.type) continue;

            Vector2 anchorPosition = boardTile.transform.position;
            bool fits = true;

            for (int i = 0; i < composite.TileOffsets.Count; i++)
            {
                var candidatePos = FindNearestAvailablePosition(
                    anchorPosition + composite.TileOffsets[i],
                    composite.BaseTiles[i].type);

                if (IsInvalidPosition(candidatePos))
                {
                    fits = false;
                    break;
                }
            }

            if (fits) return true;
        }

        return false;
    }
}
