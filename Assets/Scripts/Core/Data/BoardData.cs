using System.Collections.Generic;

public class BoardData
{
    private readonly Dictionary<GridCoord, TileCellData> _cells = new();
    private readonly Dictionary<int, List<GridCoord>> _linesByX = new();
    private readonly Dictionary<int, List<GridCoord>> _linesByY = new();
    private readonly Dictionary<int, List<GridCoord>> _linesByZ = new();

    public IReadOnlyDictionary<GridCoord, TileCellData> Cells => _cells;
    public IReadOnlyDictionary<int, List<GridCoord>> LinesByX => _linesByX;
    public IReadOnlyDictionary<int, List<GridCoord>> LinesByY => _linesByY;
    public IReadOnlyDictionary<int, List<GridCoord>> LinesByZ => _linesByZ;

    public void RegisterCell(TileCellData cell)
    {
        _cells[cell.Coord] = cell;
    }

    public TileCellData GetCell(GridCoord coord)
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

    public void SetOccupied(GridCoord coord, bool occupied)
    {
        if (_cells.TryGetValue(coord, out var cell))
            cell.IsOccupied = occupied;
    }

    private static void AddToAxis(Dictionary<int, List<GridCoord>> dict, int key, GridCoord coord)
    {
        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<GridCoord>();
            dict[key] = list;
        }

        list.Add(coord);
    }
}
