using System.Collections.Generic;
using System.Linq;

public class BoardData
{
    private readonly Dictionary<GridCoord, TileData> _tiles = new();
    private readonly Dictionary<int, List<GridCoord>> _linesByX = new();
    private readonly Dictionary<int, List<GridCoord>> _linesByY = new();
    private readonly Dictionary<int, List<GridCoord>> _linesByZ = new();

    public IReadOnlyDictionary<GridCoord, TileData> Tiles => _tiles;
    public IReadOnlyDictionary<int, List<GridCoord>> LinesByX => _linesByX;
    public IReadOnlyDictionary<int, List<GridCoord>> LinesByY => _linesByY;
    public IReadOnlyDictionary<int, List<GridCoord>> LinesByZ => _linesByZ;

    public void RegisterTiles(TileData tile)
    {
        _tiles[tile.Coord] = tile;
    }

    public TileData GetTile(GridCoord coord)
    {
        return _tiles.GetValueOrDefault(coord);
    }

    public void BuildAxisMappings()
    {
        _linesByX.Clear();
        _linesByY.Clear();
        _linesByZ.Clear();

        foreach (var coord in _tiles.Select(kvp => kvp.Key))
        {
            AddToAxis(_linesByX, coord.x, coord);
            AddToAxis(_linesByY, coord.y, coord);
            AddToAxis(_linesByZ, coord.z, coord);
        }
    }

    public void ResetOccupancy()
    {
        foreach (var kvp in _tiles)
        {
            kvp.Value.IsOccupied = false;
        }
    }

    public void SetOccupied(GridCoord coord, bool occupied)
    {
        if (_tiles.TryGetValue(coord, out var cell))
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
