namespace VectorEditor.Core.Structures;

public class Point(double x, double y)
{
    public double X { get; set; } = x; // Zmiana na public

    public double Y { get; set; } = y;

    /*public void SetPoint(double x, double y)
    {
        X = x;
        Y = y;
    }*/
    
    public Point Move(double x, double y) => new(X + x, Y + y);
    
    public override string ToString() => $"({X}, {Y})";
}