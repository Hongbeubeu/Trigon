using System.Collections.Generic;
using UnityEngine;

public class BoardData
{
    private readonly Dictionary<Vector3Int, TileCellData> _cells = new();
    private readonly Dictionary<int, List<Vector3Int>> _linesByX = new();
    private readonly Dictionary<int, List<Vector3Int>> _linesByY = new();
    private readonly Dictionary<int, List<Vector3Int>> _linesByZ = new();

    public IReadOnlyDictionary<Vector3Int, TileCellData> Cells => _cells;
    public IReadOnlyDictionary<int, List<Vector3Int>> LinesByX => _linesByX;
    public IReadOnlyDictionary<int, List<Vector3Int>> LinesByY => _linesByY;
    public IReadOnlyDictionary<int, List<Vector3Int>> LinesByZ => _linesByZ;

    public void RegisterCell(TileCellData cell)
    {
        _cells[cell.Coord] = cell;
    }

    public TileCellData GetCell(Vector3Int coord)
    {
        return _cells.TryGetValue(coord, out var cell) ? cell : null;
    }

    public void BuildAxisMappings()
    {
        _linesByX.Clear();
        _linesByY.Clear();
        _linesByZ.Clear();

        foreach (var kvp in _cells)
        {
            var coord = kvp.Key;
            AddToAxis(_linesByX, coord.x, coord);
            AddToAxis(_linesByY, coord.y, coord);
            AddToAxis(_linesByZ, coord.z, coord);
        }
    }

    public void ResetOccupancy()
    {
        foreach (var kvp in _cells)
        {
            kvp.Value.IsOccupied = false;
        }
    }

    public void SetOccupied(Vector3Int coord, bool occupied)
    {
        if (_cells.TryGetValue(coord, out var cell))
            cell.IsOccupied = occupied;
    }

    private static void AddToAxis(Dictionary<int, List<Vector3Int>> dict, int key, Vector3Int coord)
    {
        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<Vector3Int>();
            dict[key] = list;
        }

        list.Add(coord);
    }
}
