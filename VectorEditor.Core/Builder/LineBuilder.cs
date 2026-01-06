using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class LineBuilder : IShapeBuilder
{
    private Point _start = new(0, 0);
    private Point _end = new(0, 0);
    private Color _contourColor = Colors.Black;
    //private Color _contentColor = Colors.White;
    private double _width = 2.0;
    private double _opacity = 1.0;

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
    
    public LineBuilder SetContourColor(Color contourColor)
    {
        _contourColor = contourColor;
        return this;
    }

    public LineBuilder SetContentColor(Color contourColor)
    {
        return SetContourColor(contourColor);
    }

    public LineBuilder SetOpacity(double opacity)
    {
        _opacity = opacity;
        return this;
    }

    public LineBuilder SetWidth(double width)
    {
        _width = width;
        return this;
    }
    
    public IShape Build()
    {
        if (_start == null || _end == null)
            throw new InvalidOperationException("Line requires start and end points.");

        return new Line(_start, _end, _contourColor, _width, _opacity);
    }
}