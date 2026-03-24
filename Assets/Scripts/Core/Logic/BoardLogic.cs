using System;
using System.Collections.Generic;

public class BoardLogic
{
    // todo Fix logic check loose
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

    public Position2D FindNearestAvailablePosition(Position2D position, TypeTile type)
    {
        foreach (var kvp in _data.Cells)
        {
            var cell = kvp.Value;
            if (cell.Type != type || cell.IsOccupied) continue;
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
        foreach (var kvp in _data.Cells)
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

    public bool CanFitShape(List<Position2D> offsets, List<TypeTile> types)
    {
        var rootType = types[0];
        foreach (var kvp in _data.Cells)
        {
            var cell = kvp.Value;
            if (cell.IsOccupied || rootType != cell.Type) continue;
            var anchorPosition = cell.WorldPosition;
            var canFit = true;
            for (var i = 0; i < offsets.Count; i++)
            {
                var candidatePos = FindNearestAvailablePosition(anchorPosition + offsets[i], types[i]);
                if (!IsInvalidPosition(candidatePos)) continue;
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
        int totalTiles = 0;
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
            var cell = _data.GetCell(coord);
            if (cell == null || !cell.IsOccupied) return;
        }
        results.Add(new List<GridCoord>(line));
    }
}