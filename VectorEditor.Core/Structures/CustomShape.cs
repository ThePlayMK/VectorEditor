using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class CustomShape(List<Point> points, string contentColor, string contourColor, int width) : IShape
{
    private List<Point> _points = points;
    private string _contentColor = contentColor;
    private string _contourColor = contourColor;
    private int _width = width;
    public string Name => "Custom";

    public override string ToString()
    {
        var pointsStr = string.Join(",\n", _points.Select(p => p.ToString()));
        return $"Custom shape with points: [{pointsStr}], Content: {_contentColor}, Contour: {_contourColor}, Width: {_width}px";
    }
    
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}