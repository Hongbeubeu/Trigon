public class TileData
{
    public GridCoord Coord { get; }
    public Position2D WorldPosition { get; set; }
    public TypeTile Type { get; }
    public bool IsOccupied { get; set; }

    public TileData(GridCoord coord, Position2D worldPosition, TypeTile type)
    {
        Coord = coord;
        WorldPosition = worldPosition;
        Type = type;
    }
}
