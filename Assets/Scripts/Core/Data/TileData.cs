public class TileData
{
    public GridCoord Coord { get; }
    public Position2D WorldPosition { get; set; }
    public TileType TileType { get; }
    public bool IsOccupied { get; set; }

    public TileData(GridCoord coord, Position2D worldPosition, TileType tileType)
    {
        Coord = coord;
        WorldPosition = worldPosition;
        TileType = tileType;
    }
}
