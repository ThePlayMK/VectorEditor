using Avalonia.Controls;
using VectorEditor.Core.Strategy;
using VectorEditor.Core.Structures;
namespace VectorEditor.Core.Composite;

public interface ICanvas
{
    string Name { get; }
    Layer? ParentLayer { get; set; }
    bool IsBlocked { get; set; }
    bool IsVisible { get; set; }
    void Move(double dx, double dy);
    void Scale(ScaleHandle? handle, Point newPos); 
    void ScaleTransform(Point pivot, double sx, double sy);
    IEnumerable<Point> GetPoints();
    void SetPoints(List<Point> points);
    double GetMinX();
    double GetMaxX();
    double GetMinY();
    double GetMaxY();
    void ConsoleDisplay(int depth = 0);
    void Render(Canvas canvas);
    bool IsWithinBounds(Point startPoint, Point oppositePoint);
    ICanvas Clone();
}