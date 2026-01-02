using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class TriangleBuilder(string contourColor, string contentColor, int width) : IShapeBuilder 
{
    private Point _firstPoint = new(0, 0);
    private Point _secondPoint = new(0, 0);
    private Point _thirdPoint = new(0, 0);

    public TriangleBuilder SetStart(Point start)
    {
        _firstPoint = start;
        return this;
    }
    
    public TriangleBuilder SetSecond(Point second)
    {
        _secondPoint = second;
        return this;
    }
    
    public TriangleBuilder SetEnd(Point end)
    {
        _thirdPoint = end;
        return this;
    }
    
    public IShape Build()
    {
        if (_firstPoint == null || _thirdPoint == null || _secondPoint == null)
            throw new InvalidOperationException("Triangle requires 3 points.");

        return new Triangle(_firstPoint, _secondPoint, _thirdPoint, contentColor, contourColor, width);
    }
}