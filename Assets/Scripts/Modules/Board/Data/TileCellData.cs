using UnityEngine;

public class TileCellData
{
    public Vector3Int Coord { get; }
    public Vector2 WorldPosition { get; set; }
    public TypeTile Type { get; }
    public bool IsOccupied { get; set; }

    public TileCellData(Vector3Int coord, Vector2 worldPosition, TypeTile type)
    {
        Coord = coord;
        WorldPosition = worldPosition;
        Type = type;
    }
}
