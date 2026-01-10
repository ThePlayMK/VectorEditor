using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Structures;

public class Triangle(
    Point firstPoint,
    Point secondPoint,
    Point thirdPoint,
    Color contentColor,
    Color contourColor,
    double width,
    double opacity = 1.0) : IShape
{
    private Point _firstPoint = firstPoint;
    private Point _secondPoint = secondPoint;
    private Point _thirdPoint = thirdPoint;
    private Color _contentColor = contentColor;
    private Color _contourColor = contourColor;
    private double _width = width;
    private double _opacity = opacity;

    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;
    public string Name => "Triangle";

    // --- GETTERY ---
    public Color GetContentColor() => _contentColor;
    public Color GetContourColor() => _contourColor;
    public double GetWidth() => _width;
    public Point GetFirstPoint() => _firstPoint;
    public Point GetSecondPoint() => _secondPoint;
    public Point GetThirdPoint() => _thirdPoint;
    public double GetOpacity() => _opacity;
    public IEnumerable<Point> GetPoints() => new List<Point> {_firstPoint, _secondPoint, _thirdPoint};
    
    public double GetMinX() => Math.Min(_firstPoint.X, Math.Min(_secondPoint.X, _thirdPoint.X));
    public double GetMaxX() => Math.Max(_firstPoint.X, Math.Max(_secondPoint.X, _thirdPoint.X));
    public double GetMinY() => Math.Min(_firstPoint.Y, Math.Min(_secondPoint.Y, _thirdPoint.Y));
    public double GetMaxY() => Math.Max(_firstPoint.Y, Math.Max(_secondPoint.Y, _thirdPoint.Y));

    // --- SETTERY (Z LOGIKĄ BLOKADY) ---
    public void SetContentColor(Color color)
    {
        if (IsBlocked) return;
        _contentColor = color;
    }

    public void SetContourColor(Color color)
    {
        if (IsBlocked) return;
        _contourColor = color;
    }

    public void SetWidth(int width)
    {
        if (IsBlocked) return;
        _width = width;
    }

    public void SetPoints(List<Point> points)
    {
        if (IsBlocked) return;
        
        if (points.Count < 3)
        {
            return;
        }
        _firstPoint = points[0];
        _secondPoint = points[1];
        _thirdPoint = points[2];
    }
    
    public void SetOpacity(double transparency)
    {
        _opacity = transparency;
    }
    
    // --- GEOMETRIA ---
    public void Move(double dx, double dy)
    {
        if (IsBlocked) return;
        _firstPoint = new Point(_firstPoint.X + dx, _firstPoint.Y + dy);
        _secondPoint = new Point(_secondPoint.X + dx, _secondPoint.Y + dy);
        _thirdPoint = new Point(_thirdPoint.X + dx, _thirdPoint.Y + dy);
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

        _firstPoint = TransformPoint(_firstPoint, pivot, sx, sy);
        _secondPoint = TransformPoint(_secondPoint, pivot, sx, sy);
        _thirdPoint = TransformPoint(_thirdPoint, pivot, sx, sy);
    }

    private static Point TransformPoint(Point p, Point pivot, double sx, double sy)
    {
        return new Point(
            pivot.X + (p.X - pivot.X) * sx,
            pivot.Y + (p.Y - pivot.Y) * sy
        );
    }

    public void Render(Canvas canvas)
    {
        var ui = new Avalonia.Controls.Shapes.Polygon
        {
            Points =
            {
                new Avalonia.Point(_firstPoint.X, _firstPoint.Y),
                new Avalonia.Point(_secondPoint.X, _secondPoint.Y),
                new Avalonia.Point(_thirdPoint.X, _thirdPoint.Y)
            },
            Stroke = new SolidColorBrush(_contourColor, _opacity),
            Fill = new SolidColorBrush(_contentColor,_opacity),
            StrokeThickness = _width
        };

        canvas.Children.Add(ui);
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        var h1 = new Point(Math.Min(startPoint.X, oppositePoint.X), Math.Min(startPoint.Y, oppositePoint.Y));
        var h2 = new Point(Math.Max(startPoint.X, oppositePoint.X), Math.Max(startPoint.Y, oppositePoint.Y));
        startPoint = h1;
        oppositePoint = h2;
        List<Point> vertices = [_firstPoint, _secondPoint, _thirdPoint];
        // 1. Czy jakikolwiek wierzchołek trójkąta jest wewnątrz zaznaczenia?
        if (vertices.Any(p =>
                p.X >= startPoint.X && p.X <= oppositePoint.X && p.Y >= startPoint.Y && p.Y <= oppositePoint.Y))
            return true;

        // 2. Czy którakolwiek krawędź trójkąta przecina krawędzie prostokąta?
        for (var i = 0; i < 3; i++)
        {
            if (LineIntersectsRect(vertices[i], vertices[(i + 1) % 3], startPoint, oppositePoint))
                return true;
        }

        // 3. Czy środek zaznaczenia jest wewnątrz trójkąta?
        // (Obsługa przypadku gdy trójkąt jest ogromny i zawiera w sobie całe zaznaczenie)
        var center = new Point((startPoint.X + oppositePoint.X) / 2, (startPoint.Y + oppositePoint.Y) / 2);
        return IsPointInTriangle(center, _firstPoint, _secondPoint, _thirdPoint);
    }

    private static bool LineIntersectsRect(Point s, Point e, Point tl, Point br)
    {
        // Sprawdzenie przecięcia z 4 krawędziami prostokąta
        return LineIntersectsLine(s, e, tl, new Point(br.X, tl.Y)) || // Góra
               LineIntersectsLine(s, e, new Point(br.X, tl.Y), br) || // Prawo
               LineIntersectsLine(s, e, br, new Point(tl.X, br.Y)) || // Dół
               LineIntersectsLine(s, e, new Point(tl.X, br.Y), tl); // Lewo
    }

    private static bool LineIntersectsLine(Point a1, Point a2, Point b1, Point b2)
    {
        var d = (a2.X - a1.X) * (b2.Y - b1.Y) - (a2.Y - a1.Y) * (b2.X - b1.X);
        if (d == 0) return false;
        var u = ((b1.X - a1.X) * (b2.Y - b1.Y) - (b1.Y - a1.Y) * (b2.X - b1.X)) / d;
        var v = ((b1.X - a1.X) * (a2.Y - a1.Y) - (b1.Y - a1.Y) * (a2.X - a1.X)) / d;
        return u is >= 0 and <= 1 &&
               v is >= 0 and <= 1;
    }

    private static bool IsPointInTriangle(Point p, Point a, Point b, Point c)
    {
        // Algorytm oparty na współrzędnych barycentrycznych
        var denominator = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);
        var wa = ((b.Y - c.Y) * (p.X - c.X) + (c.X - b.X) * (p.Y - c.Y)) / denominator;
        var wb = ((c.Y - a.Y) * (p.X - c.X) + (a.X - c.X) * (p.Y - c.Y)) / denominator;
        var wc = 1 - wa - wb;
        return wa is >= 0 and <= 1 &&
               wb is >= 0 and <= 1 &&
               wc is >= 0 and <= 1;
    }
    
    // --- KOPIOWANIE
    public ICanvas Clone() => new Triangle(_firstPoint,_secondPoint, _thirdPoint, _contentColor, _contourColor, _width)
    {
        IsBlocked = IsBlocked,
        IsVisible =  IsVisible,
        _opacity =  _opacity
    };

    public override string ToString() =>
        $"Triangle from ({_firstPoint}), ({_secondPoint}), ({_thirdPoint}); Color: {_contentColor} and {_contourColor}, Width: {_width}px";

    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}