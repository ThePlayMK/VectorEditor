namespace VectorEditor.Core.Structures;

public class Point(double x, double y)
{
    private double X { get;} = x;
    private double Y { get;} = y;
    
    public Point Move(double x, double y) => new(X + x, Y + y);
}