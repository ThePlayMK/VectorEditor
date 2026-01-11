using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class CircleBuilder : IShapeBuilder
{
    private Point _centerPoint = new(0, 0);
    private Color _contourColor = Colors.Black;
    private Color _contentColor = Colors.White;
    private double _width = 2.0;
    private double _opacity = 1.0;
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

    public CircleBuilder SetContourColor(Color contourColor)
    {
        _contourColor = contourColor;
        return this;
    }

    public CircleBuilder SetContentColor(Color contourColor)
    {
        _contentColor = contourColor;
        return this;
    }

    public CircleBuilder SetOpacity(double opacity)
    {
        _opacity = opacity;
        return this;
    }

    public CircleBuilder SetWidth(double width)
    {
        _width = width;
        return this;
    }

    public IShape Build()
    {
        if (_centerPoint == null || _radius == 0)
            throw new InvalidOperationException("Circle requires center point and radius.");

        return new Circle(_centerPoint, _radius, _contentColor, _contourColor, _width, _opacity);
    }
}