using System;

public struct GridCoord : IEquatable<GridCoord>
{
    public int x;
    public int y;
    public int z;

    public GridCoord(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public bool Equals(GridCoord other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override bool Equals(object obj)
    {
        return obj is GridCoord other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }

    public static bool operator ==(GridCoord a, GridCoord b) => a.Equals(b);
    public static bool operator !=(GridCoord a, GridCoord b) => !a.Equals(b);

    public override string ToString() => $"({x}, {y}, {z})";
}
