using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Line(Point startPoint, Point endPoint, string color, int width) : IShape
{
    private Point StartPoint { get; set; } = startPoint;
    private Point EndPoint { get; set; } = endPoint;
    private string Color { get; set; } = color;
    private int Width { get; set; } = width;
    public string Name => "Line";
    
    public override string ToString() => $"Line from {StartPoint} to {EndPoint}";
    
}