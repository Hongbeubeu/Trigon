using System;
using System.Collections.Generic;

/// <summary>
/// Provides mathematics and collision detection for hexagon tile placement.
/// Operates exclusively on GridCoords without Unity physics bindings.
/// </summary>
public class BoardLogic : IBoardLogic
{
    private static readonly Position2D InvalidPosition = new(float.MaxValue, float.MaxValue);
    private readonly BoardData _data;
    private readonly float _snapThreshold;
    private readonly float _exactThreshold;

    public BoardLogic(BoardData data, float snapThreshold, float exactThreshold)
    {
        _data = data;
        _snapThreshold = snapThreshold;
        _exactThreshold = exactThreshold;
    }

    public Position2D FindNearestAvailablePosition(Position2D position, TileType tileType)
    {
        foreach (var kvp in _data.Tiles)
        {
            var cell = kvp.Value;
            if (cell.TileType != tileType || cell.IsOccupied) continue;
            if (MathF.Abs(cell.WorldPosition.x - position.x) < _snapThreshold &&
                MathF.Abs(cell.WorldPosition.y - position.y) < _snapThreshold)
            {
                return cell.WorldPosition;
            }
        }
        return InvalidPosition;
    }

    public bool IsInvalidPosition(Position2D position)
    {
        return MathF.Abs(position.x - InvalidPosition.x) < 1e-4f &&
               MathF.Abs(position.y - InvalidPosition.y) < 1e-4f;
    }

    public GridCoord GetCoordAtPosition(Position2D position)
    {
        foreach (var kvp in _data.Tiles)
        {
            var cell = kvp.Value;
            if (MathF.Abs(cell.WorldPosition.x - position.x) < _exactThreshold &&
                MathF.Abs(cell.WorldPosition.y - position.y) < _exactThreshold)
            {
                return cell.Coord;
            }
        }
        return new GridCoord(-100, 0, 0);
    }

    public GridCoord PlaceTile(Position2D tileWorldPosition)
    {
        var coord = GetCoordAtPosition(tileWorldPosition);
        _data.SetOccupied(coord, true); 
        return coord;
    }

    public void RemoveTile(GridCoord coord)
    {
        _data.SetOccupied(coord, false);
    }

    public bool CanFitShape(List<GridCoord> gridOffsets, TileType rootTileType)
    {
        foreach (var kvp in _data.Tiles)
        {
            var tile = kvp.Value;
            if (tile.IsOccupied || tile.TileType != rootTileType) continue;

            var canFit = true;
            foreach (var offset in gridOffsets)
            {
                var targetCoord = tile.Coord + offset;
                var targetCell = _data.GetTile(targetCoord);
                if (targetCell is { IsOccupied: false }) continue;
                canFit = false;
                break;
            }

            if (canFit) return true;
        }
        return false;
    }

    public List<List<GridCoord>> FindCompletedLines(List<GridCoord> recentCoords)
    {
        var result = new List<List<GridCoord>>();
        var checkedX = new HashSet<int>();
        var checkedY = new HashSet<int>();
        var checkedZ = new HashSet<int>();
        foreach (var coord in recentCoords)
        {
            TryCollectLine(_data.LinesByX, coord.x, checkedX, result);
            TryCollectLine(_data.LinesByY, coord.y, checkedY, result);
            TryCollectLine(_data.LinesByZ, coord.z, checkedZ, result);
        }
        return result;
    }

    public static int CalculateLineScore(List<List<GridCoord>> lines)
    {
        var totalTiles = 0;
        foreach (var line in lines)
        {
            totalTiles += line.Count;
        }
        return totalTiles * lines.Count;
    }

    private void TryCollectLine(IReadOnlyDictionary<int, List<GridCoord>> axisLines, int axisValue, HashSet<int> alreadyChecked, List<List<GridCoord>> results)
    {
        if (!alreadyChecked.Add(axisValue)) return;
        if (!axisLines.TryGetValue(axisValue, out var line)) return;
        foreach (var coord in line)
        {
            var cell = _data.GetTile(coord);
            if (cell == null || !cell.IsOccupied) return;
        }
        results.Add(new List<GridCoord>(line));
    }
}