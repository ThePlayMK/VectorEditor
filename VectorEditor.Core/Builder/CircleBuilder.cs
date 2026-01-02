using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class CircleBuilder(string contourColor, string contentColor, int width) : IShapeBuilder 
{
    private Point _centerPoint = new(0, 0);
    private double _radius;

    public CircleBuilder SetStart(Point start)
    {
        _centerPoint = start;
        return this;
    }
    
    public CircleBuilder SetRadius(Point end)
    {
        _radius = Math.Sqrt(Math.Pow(end.X - _centerPoint.X, 2) + Math.Pow(end.Y - _centerPoint.Y, 2));
        return this;
    }

    public CircleBuilder SetRadius(double radius)
    {
        _radius = radius;
        return this;
    }
    
    public IShape Build()
    {
        if (_centerPoint == null || _radius == 0)
            throw new InvalidOperationException("Circle requires center point and radius.");

        return new Circle(_centerPoint, _radius, contentColor, contourColor, width);
    }
}