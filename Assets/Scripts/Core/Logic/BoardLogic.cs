using System;
using System.Collections.Generic;

public class BoardLogic
{
    private static readonly Position2D INVALID_POSITION = new(-100f, 0f);

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

            if (MathF.Abs(cell.WorldPosition.X - position.X) < _snapThreshold &&
                MathF.Abs(cell.WorldPosition.Y - position.Y) < _snapThreshold)
            {
                return cell.WorldPosition;
            }
        }

        return INVALID_POSITION;
    }

    public bool IsInvalidPosition(Position2D position)
    {
        return MathF.Abs(position.X - INVALID_POSITION.X) < 1e-4f &&
               MathF.Abs(position.Y - INVALID_POSITION.Y) < 1e-4f;
    }

    public GridCoord GetCoordAtPosition(Position2D position)
    {
        foreach (var kvp in _data.Cells)
        {
            var cell = kvp.Value;
            if (MathF.Abs(cell.WorldPosition.X - position.X) < _exactThreshold &&
                MathF.Abs(cell.WorldPosition.Y - position.Y) < _exactThreshold)
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
            bool fits = true;

            for (int i = 0; i < offsets.Count; i++)
            {
                var candidatePos = FindNearestAvailablePosition(
                    anchorPosition + offsets[i], types[i]);

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

    public List<List<GridCoord>> FindCompletedLines(List<GridCoord> recentCoords)
    {
        var result = new List<List<GridCoord>>();
        var checkedX = new HashSet<int>();
        var checkedY = new HashSet<int>();
        var checkedZ = new HashSet<int>();

        foreach (var coord in recentCoords)
        {
            TryCollectLine(_data.LinesByX, coord.X, checkedX, result);
            TryCollectLine(_data.LinesByY, coord.Y, checkedY, result);
            TryCollectLine(_data.LinesByZ, coord.Z, checkedZ, result);
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

    private void TryCollectLine(
        IReadOnlyDictionary<int, List<GridCoord>> axisLines,
        int axisValue,
        HashSet<int> alreadyChecked,
        List<List<GridCoord>> results)
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
