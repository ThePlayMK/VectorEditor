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
    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; } 
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
        $"Rectangle from ({StartPoint}), ({HelperPoint1}), ({OppositePoint}), ({HelperPoint2}) Color: {ContentColor} and {ContourColor}";

    
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        // 1. Wyznaczamy krawędzie prostokąta obiektu
        var rectLeft = Math.Min(StartPoint.X, OppositePoint.X);
        var rectRight = Math.Max(StartPoint.X, OppositePoint.X);
        var rectTop = Math.Min(StartPoint.Y, OppositePoint.Y);
        var rectBottom = Math.Max(StartPoint.Y, OppositePoint.Y);

        // 2. Wyznaczamy krawędzie prostokąta zaznaczenia
        var selLeft = Math.Min(startPoint.X, oppositePoint.X);
        var selRight = Math.Max(startPoint.X, oppositePoint.X);
        var selTop = Math.Min(startPoint.Y, oppositePoint.Y);
        var selBottom = Math.Max(startPoint.Y, oppositePoint.Y);

        // 3. Test nachodzenia (AABB)
        var overlapX = rectLeft <= selRight && 
                       rectRight >= selLeft;
        var overlapY = rectTop <= selBottom && 
                       rectBottom >= selTop;

        return overlapX && overlapY;
    }
}