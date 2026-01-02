namespace VectorEditor.Core.Structures;

public class Point(double x, double y)
{
    internal double X { get;} = x;
    internal double Y { get;} = y;
    
    public Point Move(double x, double y) => new(X + x, Y + y);
    
    public override string ToString() => $"({X}, {Y})";
}