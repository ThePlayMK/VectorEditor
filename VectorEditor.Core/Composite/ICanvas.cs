using VectorEditor.Core.Structures;
namespace VectorEditor.Core.Composite;

public interface ICanvas
{
    Layer? ParentLayer { get; set; }
    bool IsBlocked { get; set; }
    bool IsVisible { get; set; }
    void Move(int dx, int dy);
    void ConsoleDisplay(int depth = 0);
    bool IsWithinBounds(Point startPoint, Point oppositePoint);

}