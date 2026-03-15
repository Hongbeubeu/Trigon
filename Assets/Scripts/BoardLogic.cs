using System.Collections.Generic;
using UnityEngine;

public class BoardLogic
{
    private const float SNAP_THRESHOLD = 0.25f;
    private const float EXACT_THRESHOLD = 0.01f;
    private static readonly Vector2 INVALID_POSITION = new(-100f, 0f);

    private readonly BoardData _data;

    public BoardLogic(BoardData data)
    {
        _data = data;
    }

    public Vector2 FindNearestAvailablePosition(Vector2 position, TypeTile type)
    {
        foreach (var kvp in _data.Cells)
        {
            var cell = kvp.Value;
            if (cell.Type != type || cell.IsOccupied) continue;

            if (Mathf.Abs(cell.WorldPosition.x - position.x) < SNAP_THRESHOLD &&
                Mathf.Abs(cell.WorldPosition.y - position.y) < SNAP_THRESHOLD)
            {
                return cell.WorldPosition;
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
        foreach (var kvp in _data.Cells)
        {
            var cell = kvp.Value;
            if (Mathf.Abs(cell.WorldPosition.x - position.x) < EXACT_THRESHOLD &&
                Mathf.Abs(cell.WorldPosition.y - position.y) < EXACT_THRESHOLD)
            {
                return cell.Coord;
            }
        }

        return new Vector3Int(-100, 0, 0);
    }

    public Vector3Int PlaceTile(Vector2 tileWorldPosition)
    {
        var coord = GetCoordAtPosition(tileWorldPosition);
        _data.SetOccupied(coord, true);
        return coord;
    }

    public void RemoveTile(Vector3Int coord)
    {
        _data.SetOccupied(coord, false);
    }

    public bool CanFitShape(List<Vector2> offsets, List<TypeTile> types)
    {
        var rootType = types[0];

        foreach (var kvp in _data.Cells)
        {
            var cell = kvp.Value;
            if (cell.IsOccupied || rootType != cell.Type) continue;

            Vector2 anchorPosition = cell.WorldPosition;
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

    public List<List<Vector3Int>> FindCompletedLines(List<Vector3Int> recentCoords)
    {
        var result = new List<List<Vector3Int>>();
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

    public static int CalculateLineScore(List<List<Vector3Int>> lines)
    {
        int totalTiles = 0;
        foreach (var line in lines)
        {
            totalTiles += line.Count;
        }

        return totalTiles * lines.Count;
    }

    private void TryCollectLine(
        IReadOnlyDictionary<int, List<Vector3Int>> axisLines,
        int axisValue,
        HashSet<int> alreadyChecked,
        List<List<Vector3Int>> results)
    {
        if (!alreadyChecked.Add(axisValue)) return;
        if (!axisLines.TryGetValue(axisValue, out var line)) return;

        foreach (var coord in line)
        {
            var cell = _data.GetCell(coord);
            if (cell == null || !cell.IsOccupied) return;
        }

        results.Add(new List<Vector3Int>(line));
    }
}
