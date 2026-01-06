namespace VectorEditor.Core.Structures;

public class Point(double x, double y)
{
    public double X { get; private set; } = x; // Zmiana na public

    public double Y { get; private set; } = y;

    /*public void SetPoint(double x, double y)
    {
        X = x;
        Y = y;
    }*/
    
    public Point Move(double x, double y) => new(X + x, Y + y);
    
    public override string ToString() => $"({X}, {Y})";
}