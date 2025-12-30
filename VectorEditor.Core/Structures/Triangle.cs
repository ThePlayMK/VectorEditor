using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Triangle(Point firstPoint, Point secondPoint, Point thirdPoint, string contentColor, string contourColor, int width) : IShape
{
    private Point FirstPoint { get; set; } = firstPoint;
    private Point SecondPoint { get; set; } = secondPoint;
    private Point ThirdPoint { get; set; } = thirdPoint;
    private string ContentColor { get; set; } = contentColor;
    private string ContourColor { get; set; } = contourColor;
    private int Width { get; set; } = width;
    public string Name => "Triangle";
    
    public override string ToString() => 
        $"Rectangle from ({FirstPoint}), ({SecondPoint}), ({ThirdPoint}); Color: {ContentColor} and {ContourColor}, Width: {Width}px";
    
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}