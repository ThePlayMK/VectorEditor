using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class LineBuilder(Color color, double width, double opacity = 1.0) : IShapeBuilder
{
    private Point _start = new(0, 0);
    private Point _end = new(0, 0);

    public LineBuilder SetStart(Point start)
    {
        _start = start;
        return this;
    }
    
    public LineBuilder SetEnd(Point end)
    {
        _end = end;
        return this;
    }
    
    public IShape Build()
    {
        if (_start == null || _end == null)
            throw new InvalidOperationException("Line requires start and end points.");

        return new Line(_start, _end, color, width, opacity);
    }
}