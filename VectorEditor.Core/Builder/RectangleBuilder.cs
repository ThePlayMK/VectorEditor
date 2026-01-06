using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Builder;

public class RectangleBuilder : IShapeBuilder 
{
    private Point _startPoint = new(0, 0);
    private Point _endPoint = new(0, 0);
    private Color _contourColor = Colors.Black;
    private Color _contentColor  = Colors.White;
    private double _width = 2.0;
    private double _opacity = 1.0;

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
    
    public RectangleBuilder SetContourColor(Color contourColor)
    {
        _contourColor = contourColor;
        return this;
    }

    public RectangleBuilder SetContentColor(Color contourColor)
    {
        _contentColor = contourColor;
        return this;
    }

    public RectangleBuilder SetOpacity(double opacity)
    {
        _opacity = opacity;
        return this;
    }

    public RectangleBuilder SetWidth(double width)
    {
        _width = width;
        return this;
    }
    
    public IShape Build()
    {
        if (_startPoint == null || _endPoint == null)
            throw new InvalidOperationException("Rectangle requires start and end point.");

        return new Rectangle(_startPoint, _endPoint, _contentColor, _contourColor, _width, _opacity);
    }
}