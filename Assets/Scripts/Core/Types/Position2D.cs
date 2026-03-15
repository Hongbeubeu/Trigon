using System;

public readonly struct Position2D : IEquatable<Position2D>
{
    public readonly float X;
    public readonly float Y;

    public static readonly Position2D Zero = new(0f, 0f);

    public Position2D(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static Position2D operator +(Position2D a, Position2D b) => new(a.X + b.X, a.Y + b.Y);
    public static Position2D operator -(Position2D a, Position2D b) => new(a.X - b.X, a.Y - b.Y);

    public bool Equals(Position2D other)
    {
        return MathF.Abs(X - other.X) < 1e-6f && MathF.Abs(Y - other.Y) < 1e-6f;
    }

    public override bool Equals(object obj)
    {
        return obj is Position2D other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Position2D a, Position2D b) => a.Equals(b);
    public static bool operator !=(Position2D a, Position2D b) => !a.Equals(b);

    public override string ToString() => $"({X:F2}, {Y:F2})";
}
