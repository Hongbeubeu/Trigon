using UnityEngine;

public static class TypeConversions
{
    public static GridCoord ToGridCoord(Vector3Int v) => new(v.x, v.y, v.z);
    public static Vector3Int ToVector3Int(GridCoord g) => new(g.x, g.y, g.z);
    public static Position2D ToPosition2D(Vector2 v) => new(v.x, v.y);
    public static Vector2 ToVector2(Position2D p) => new(p.x, p.y);
}
