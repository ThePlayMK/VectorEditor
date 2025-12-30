using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Rectangle : IShape
{
    private Point StartPoint { get; set; }
    private Point HelperPoint1 { get; set; }
    private Point OppositePoint { get; set; }
    private Point HelperPoint2 { get; set; }
    private string ContentColor { get; set; }
    private string ContourColor { get; set; }
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
}