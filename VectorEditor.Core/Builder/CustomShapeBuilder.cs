using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class CustomShapeBuilder : IShapeBuilder
{
    private readonly List<Point> _points = [];
    private Color _contourColor = Colors.Black;
    private Color _contentColor = Colors.White;
    private double _width = 2.0;
    private double _opacity = 1.0;

    public IEnumerable<Point> GetPoints() => _points;


    public CustomShapeBuilder AddPoint(Point point)
    {
        _points.Add(point);
        return this;
    }

    public CustomShapeBuilder SetContourColor(Color contourColor)
    {
        _contourColor = contourColor;
        return this;
    }

    public CustomShapeBuilder SetContentColor(Color contourColor)
    {
        _contentColor = contourColor;
        return this;
    }

    public CustomShapeBuilder SetOpacity(double opacity)
    {
        _opacity = opacity;
        return this;
    }

    public CustomShapeBuilder SetWidth(double width)
    {
        _width = width;
        return this;
    }

    public void Reset()
    {
        _points.Clear();
        _contourColor = Colors.Black;
        _contentColor = Colors.White;
        _width = 2.0;
        _opacity = 1.0;
    }

    public IShape Build()
    {
        return _points.Count < 2
            ? throw new InvalidOperationException("Custom shape requires at least two points.")
            : new CustomShape(_points, _contentColor, _contourColor, _width, _opacity);
    }
}