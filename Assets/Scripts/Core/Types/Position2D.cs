using System;

public readonly struct Position2D : IEquatable<Position2D>
{
    public readonly float x;
    public readonly float y;

    public static readonly Position2D ZERO = new(0f, 0f);

    public Position2D(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static Position2D operator +(Position2D a, Position2D b) => new(a.x + b.x, a.y + b.y);
    public static Position2D operator -(Position2D a, Position2D b) => new(a.x - b.x, a.y - b.y);

    public bool Equals(Position2D other)
    {
        return MathF.Abs(x - other.x) < 1e-6f && MathF.Abs(y - other.y) < 1e-6f;
    }

    public override bool Equals(object obj)
    {
        return obj is Position2D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static bool operator ==(Position2D a, Position2D b) => a.Equals(b);
    public static bool operator !=(Position2D a, Position2D b) => !a.Equals(b);

    public override string ToString() => $"({x:F2}, {y:F2})";
}
