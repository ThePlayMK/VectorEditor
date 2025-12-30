using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Rectangle : IShape
{
    private Point StartPoint { get; set; }
    private Point HelperPoint1 { get; set; }
    private Point OppositePoint { get; set; }
    private Point HelperPoint2 { get; set; }
    public string ContentColor { get; set; }
    public string ContourColor { get; set; }
    private int Width { get; set; }
    public string Name => "Rectangle";

    public Rectangle(Point startPoint, Point oppositePoint, string contentColor, string contourColor, int width)
    {
        StartPoint = startPoint;
        OppositePoint = oppositePoint;
        ContentColor = contentColor;
        ContourColor = contourColor;
        Width = width;
        HelperPoint1 = new Point(StartPoint.X, OppositePoint.Y);
        HelperPoint2 = new Point(OppositePoint.X, StartPoint.Y);
    }
    
    public override string ToString() => 
        $"Rectangle from ({StartPoint}), ({HelperPoint1}), ({OppositePoint}), ({HelperPoint2})";

    
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        // Sprawdzamy, czy prostokąty NIE są rozłączne.
        // Dwa prostokąty nachodzą na siebie, jeśli:
        // (Lewa krawędź A < Prawa krawędź B) ORAZ (Prawa krawędź A > Lewa krawędź B)
        // ORAZ to samo dla osi Y.

        var overlapX = StartPoint.X <= oppositePoint.X && OppositePoint.X >= startPoint.X;
        var overlapY = StartPoint.Y <= oppositePoint.Y && OppositePoint.Y >= startPoint.Y;

        return overlapX && overlapY;
    }
}