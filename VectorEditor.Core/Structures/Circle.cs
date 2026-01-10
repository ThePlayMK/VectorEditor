using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Structures;

public class Circle(Point centerPoint, double radius, Color contentColor, Color contourColor, double width, double opacity = 1.0) : IShape
{
    private Point _centerPoint = centerPoint;
    private double _radiusX = radius;
    private double _radiusY = radius;
    private Color _contentColor = contentColor;
    private Color _contourColor = contourColor;
    private double _width = width;
    private double _opacity = opacity;

    public string Name => Math.Abs(_radiusX - _radiusY) < 0.001 ? "Circle" : "Ellipse";
    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;
    
    // --- GETTERY ---
    public Color GetContentColor() => _contentColor;
    public Color GetContourColor() => _contourColor;
    public double GetWidth() => _width;
    public Point GetCenterPoint() => _centerPoint;
    public double GetOpacity() => _opacity;
    public double GetRadiusX() => _radiusX;
    public double GetRadiusY() => _radiusY;
    public IEnumerable<Point> GetPoints() => 
    [
        _centerPoint, 
        new Point(_centerPoint.X + _radiusX, _centerPoint.Y + _radiusY)
    ];
    public double GetMinX() => _centerPoint.X - _radiusX;
    public double GetMaxX() => _centerPoint.X + _radiusX;
    public double GetMinY() => _centerPoint.Y - _radiusY;
    public double GetMaxY() => _centerPoint.Y + _radiusY;
    
    // --- SETERY (Z LOGIKĄ BLOKADY) ---
    public void SetContentColor(Color  color)
    {
        if (IsBlocked) return;
        _contentColor = color;
    }

    public void SetContourColor(Color  color)
    {
        if (IsBlocked) return;
        _contourColor = color;
    }

    public void SetWidth(int width)
    {
        if (IsBlocked) return;
        _width = width;
    }

    public void SetRadius(double radius)
    {
        if (IsBlocked) return;
        _radiusX = radius;
        _radiusY = radius;
    }

    public void SetRadius(double radiusX, double radiusY)
    {
        if (IsBlocked) return;
        _radiusX = radiusX;
        _radiusY = radiusY;
    }
    
    public void SetPoints(List<Point> points)
    {
        if (IsBlocked || points.Count < 2)
        {
            return;
        }
        
        _centerPoint = points[0];
        _radiusX = Math.Abs(points[1].X - points[0].X);
        _radiusY = Math.Abs(points[1].Y - points[0].Y);
    }

    public void SetOpacity(double transparency)
    {
        _opacity = transparency;
    }
    
    // --- GEOMETRIA ---
    public void Move(double dx, double dy)
    {
        if (IsBlocked) return;
        _centerPoint = new Point(_centerPoint.X + dx, _centerPoint.Y + dy);
    }

    public void Scale(ScaleHandle? handle, Point newPos)
    {
        if (IsBlocked) return;

        var left = GetMinX();
        var right = GetMaxX();
        var top = GetMinY();
        var bottom = GetMaxY();

        // 1. Wyznaczamy Pivot (punkt, który się nie rusza)
        var pivot = handle switch
        {
            ScaleHandle.TopLeft => new Point(right, bottom),
            ScaleHandle.Top => new Point(left, bottom),
            ScaleHandle.TopRight => new Point(left, bottom),
            ScaleHandle.Right => new Point(left, top),
            ScaleHandle.BottomRight => new Point(left, top),
            ScaleHandle.Bottom => new Point(left, top),
            ScaleHandle.BottomLeft => new Point(right, top),
            ScaleHandle.Left => new Point(right, top),
            _ => new Point(left, top)
        };

        // 2. Obliczamy stare i nowe wymiary Bounding Boxa
        var oldW = Math.Max(1, right - left);
        var oldH = Math.Max(1, bottom - top);
        var newW = oldW;
        var newH = oldH;

        switch (handle)
        {
            case ScaleHandle.TopLeft:
                newW = right - newPos.X;
                newH = bottom - newPos.Y;
                break;
            case ScaleHandle.Top: 
                newH = bottom - newPos.Y; 
                break;
            case ScaleHandle.TopRight:
                newW = newPos.X - left;
                newH = bottom - newPos.Y;
                break;
            case ScaleHandle.Right: 
                newW = newPos.X - left; 
                break;
            case ScaleHandle.BottomRight:
                newW = newPos.X - left;
                newH = newPos.Y - top;
                break;
            case ScaleHandle.Bottom:
                newH = newPos.Y - top; 
                break;
            case ScaleHandle.BottomLeft:
                newW = right - newPos.X;
                newH = newPos.Y - top;
                break;
            case ScaleHandle.Left: 
                newW = right - newPos.X; 
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(handle), handle, null);
        }

        var sx = newW / oldW;
        var sy = newH / oldH;

        // 3. Wykonujemy transformację
        ScaleTransform(pivot, sx, sy);
    }

    public void ScaleTransform(Point pivot, double sx, double sy)
    {
        if (IsBlocked) return;

        // 1. Skalujemy środek względem pivotu
        var newCenterX = pivot.X + (_centerPoint.X - pivot.X) * sx;
        var newCenterY = pivot.Y + (_centerPoint.Y - pivot.Y) * sy;
        _centerPoint = new Point(newCenterX, newCenterY);

        // 2. Skalujemy promienie (zawsze wartości dodatnie)
        _radiusX *= Math.Abs(sx);
        _radiusY *= Math.Abs(sy);
    }

    public void Render(Canvas canvas)
    {
        if (!IsVisible)
        {
            return;
        }
        
        var ui = new Avalonia.Controls.Shapes.Ellipse
        {
            Width = _radiusX * 2,
            Height = _radiusY * 2,
            Stroke = new SolidColorBrush(_contourColor, _opacity),
            Fill = new SolidColorBrush(_contentColor,_opacity),
            StrokeThickness = _width
        };

        Canvas.SetLeft(ui, _centerPoint.X - _radiusX);
        Canvas.SetTop(ui, _centerPoint.Y - _radiusY);
        canvas.Children.Add(ui);
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        var h1 = new Point(Math.Min(startPoint.X, oppositePoint.X), Math.Min(startPoint.Y, oppositePoint.Y));
        var h2 = new Point(Math.Max(startPoint.X, oppositePoint.X), Math.Max(startPoint.Y, oppositePoint.Y));
        startPoint = h1;
        oppositePoint = h2;
        // 1. Znajdź punkt na prostokącie (lub w jego środku), który jest najbliżej środka koła
        // Clampujemy współrzędne środka koła do zakresu prostokąta [tl, br]
        var closestX = Math.Clamp(_centerPoint.X, startPoint.X, oppositePoint.X);
        var closestY = Math.Clamp(_centerPoint.Y, startPoint.Y, oppositePoint.Y);

        // 2. Oblicz odległość (euklidesową) od środka koła do tego najbliższego punktu
        var dx = (_centerPoint.X - closestX) / _radiusX;
        var dy = (_centerPoint.Y - closestY) / _radiusY;

        // 3. Sprawdź, czy kwadrat odległości jest mniejszy lub równy kwadratowi promienia
        // (Unikamy wyciągania pierwiastka dla wydajności)
        return dx * dx + dy * dy <= 1;
    }
    
    public ICanvas Clone() => new Circle(_centerPoint, _radiusX, _contentColor, _contourColor, _width)
    {
        _radiusY =  _radiusY,
        IsBlocked = IsBlocked,
        IsVisible =  IsVisible,
        _opacity =  _opacity
    };
    
    public override string ToString() => 
        $"Circle Center: {_centerPoint}, Radius: {_radiusX} x {_radiusY}, Color: {_contentColor} and {_contourColor}, Width: {_width}px";
    
    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}