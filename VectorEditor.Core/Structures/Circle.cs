using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Circle(Point centerPoint, double radius, string contentColor, string contourColor, int width) : IShape
{
    private Point CenterPoint { get; set; } = centerPoint;
    private double Radius { get; set; } = radius;
    private string ContentColor { get; set; } = contentColor;
    private string ContourColor { get; set; } = contourColor;
    private int Width { get; set; } = width;
    public string Name => "Circle";
    
    public override string ToString() => 
        $"Circle Center: {CenterPoint}, Radius: {Radius}, Color: {ContentColor} and {ContourColor}, Width: {Width}px";
    
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}