using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class RectangleBuilder(string contourColor, string contentColor, int width) : IShapeBuilder 
{
    private Point _startPoint = new(0, 0);
    private Point _endPoint = new(0, 0);

    public RectangleBuilder SetStart(Point start)
    {
        _startPoint = start;
        return this;
    }
    
    public RectangleBuilder SetEnd(Point end)
    {
        _endPoint = end;
        return this;
    }
    
    public IShape Build()
    {
        if (_startPoint == null || _endPoint == null)
            throw new InvalidOperationException("Rectangle requires start and end point.");

        return new Rectangle(_startPoint, _endPoint, contentColor, contourColor, width);
    }
}