using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class CustomShapeBuilder(string contentColor, string contourColor, int width) : IShapeBuilder
{
    private readonly List<Point> _points = [];
    private bool _isClosed;

    public CustomShapeBuilder AddPoint(Point point)
    {
        if (_isClosed)
        {
            throw new InvalidOperationException("Cannot add points to a closed shape.");
        }

        _points.Add(point);
        
        if (_points.Count >= 3 && DoesNewSegmentCloseShape())
        {
            _isClosed = true;
        }

        return this;
    }

    public CustomShapeBuilder Close()
    {
        if (_points.Count < 3)
        {
            throw new InvalidOperationException("Cannot close a shape with fewer than 3 points.");
        }

        _isClosed = true;
        return this;
    }

    public IShape Build()
    {
        return _points.Count < 2 ? 
            throw new InvalidOperationException("Custom shape requires at least two points.") : 
            new CustomShape(_points, contentColor, contourColor, width);
    }


    private bool DoesNewSegmentCloseShape()
    {
        if (_points.Count < 3)
        {
            return false;
        }

        var newPoint = _points[^1];
        var lastPoint = _points[^2];
        
        for (var i = 0; i < _points.Count - 2; i++)
        {
            if (DoSegmentsIntersect(_points[i], _points[i + 1], lastPoint, newPoint))
            {
                return true;
            }
        }

        return false;
    }

    private static bool DoSegmentsIntersect(Point p1, Point p2, Point p3, Point p4)
    {
        var d1 = Direction(p3, p4, p1);
        var d2 = Direction(p3, p4, p2);
        var d3 = Direction(p1, p2, p3);
        var d4 = Direction(p1, p2, p4);

        return ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
               ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0));
    }

    private static double Direction(Point pi, Point pj, Point pk)
    {
        return (pk.X - pi.X) * (pj.Y - pi.Y) - (pj.X - pi.X) * (pk.Y - pi.Y);
    }
}