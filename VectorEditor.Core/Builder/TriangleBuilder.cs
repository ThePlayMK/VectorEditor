using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class TriangleBuilder : IShapeBuilder
{
    private Point _firstPoint = new(0, 0);
    private Point _secondPoint = new(0, 0);
    private Point _thirdPoint = new(0, 0);
    private Color _contourColor = Colors.Black;
    private Color _contentColor = Colors.White;
    private double _width = 2.0;
    private double _opacity = 1.0;

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

    public TriangleBuilder SetContourColor(Color contourColor)
    {
        _contourColor = contourColor;
        return this;
    }

    public TriangleBuilder SetContentColor(Color contourColor)
    {
        _contentColor = contourColor;
        return this;
    }

    public TriangleBuilder SetOpacity(double opacity)
    {
        _opacity = opacity;
        return this;
    }

    public TriangleBuilder SetWidth(double width)
    {
        _width = width;
        return this;
    }

    public IShape Build()
    {
        if (_firstPoint == null || _thirdPoint == null || _secondPoint == null)
            throw new InvalidOperationException("Triangle requires 3 points.");

        return new Triangle(_firstPoint, _secondPoint, _thirdPoint, _contentColor, _contourColor, _width, _opacity);
    }
}