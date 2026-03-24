using UnityEngine;

public static class HexGridExtensions
{
    public static readonly float SQRT3 = Mathf.Sqrt(3f);
    public static float GetTileHeight(float tileWidth = 1f) => tileWidth * SQRT3 / 2f;
    public static bool IsUpTile(this GridCoord coord) => coord.x - coord.y - coord.z == 0;

    // Converts a GridCoord to a world position Vector2
    public static Vector2 ToVector2(this GridCoord coord, out bool isUpTile, float tileWidth = 1f)
    {
        isUpTile = coord.IsUpTile();
        var column = isUpTile ? 2 * coord.z : 2 * coord.z + 1;
        var h = tileWidth * SQRT3 / 2f;
        var upYOffset = tileWidth * SQRT3 / 6f;
        var downYOffset = tileWidth * SQRT3 / 3f;
        var posX = (-coord.x + column) * tileWidth / 2f;
        var posY = -(coord.x * h) + (isUpTile ? upYOffset : downYOffset);
        return new Vector2(posX, posY);
    }

    // Converts a world position Vector2 to a GridCoord
    public static GridCoord ToGridCoord(this Vector2 position, float tileWidth)
    {
        var h = tileWidth * SQRT3 / 2f;
        var rowFloat = (-position.y) / h + (2f / 3f);
        var x = Mathf.FloorToInt(rowFloat);
        var colFloat = (2f * position.x) / tileWidth + x;
        var column = Mathf.RoundToInt(colFloat);
        var isUpTile = column % 2 == 0;
        int y, z;
        if (isUpTile)
        {
            z = column / 2;
            y = x - z;
        }
        else
        {
            z = (column - 1) / 2;
            y = x - z - 1;
        }
        return new GridCoord(x, y, z);
    }
}