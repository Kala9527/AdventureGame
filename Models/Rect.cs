namespace AdventureGame.Models;

public class GameRect : IEquatable<GameRect>
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public GameRect(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool Intersects(GameRect other)
    {
        return X < other.X + other.Width &&
               X + Width > other.X &&
               Y < other.Y + other.Height &&
               Y + Height > other.Y;
    }

    public double Right => X + Width;
    public double Bottom => Y + Height;
    public double CenterX => X + Width / 2;
    public double CenterY => Y + Height / 2;

    public System.Windows.Rect ToWpfRect() => new(X, Y, Width, Height);

    public bool Equals(GameRect? other)
    {
        if (other is null) return false;
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object? obj) => Equals(obj as GameRect);
    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
    public static bool operator ==(GameRect? left, GameRect? right) => EqualityComparer<GameRect>.Default.Equals(left, right);
    public static bool operator !=(GameRect? left, GameRect? right) => !(left == right);
}
