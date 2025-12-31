using VectorEditor.Core.Strategy;
using VectorEditor.Core.Structures;
namespace VectorEditor.Core.Composite;

public interface ICanvas
{
    Layer? ParentLayer { get; set; }
    bool IsBlocked { get; set; }
    bool IsVisible { get; set; }
    void Move(int dx, int dy);
    void Scale(ScaleHandle handle, Point newPos); 
    void ScaleTransform(Point pivot, double sx, double sy);
    double GetMinX();
    double GetMaxX();
    double GetMinY();
    double GetMaxY();
    void ConsoleDisplay(int depth = 0);
    bool IsWithinBounds(Point startPoint, Point oppositePoint);
    

}