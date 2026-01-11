namespace VectorEditor.Core.Structures;

public class Point(double x, double y)
{
    public double X { get; } = x; // Zmiana na public

    public double Y { get; } = y;

    public override string ToString() => $"({X}, {Y})";
}