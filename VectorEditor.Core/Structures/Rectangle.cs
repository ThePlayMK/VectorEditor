using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Structures;

public class Rectangle : IShape
{
    // Pola prywatne - brak bezpośredniego dostępu z zewnątrz
    private Color _contentColor;
    private Color _contourColor;
    private int _width;
    private Point _startPoint;
    private Point _oppositePoint;
    private Point? _helperPoint1;
    private Point? _helperPoint2;
    private double _transparency = 0;

    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;
    public string Name => "Rectangle";

    public Rectangle(Point startPoint, Point oppositePoint, Color contentColor, Color contourColor, int width)
    {
        _startPoint = startPoint;
        _oppositePoint = oppositePoint;
        _contentColor = contentColor;
        _contourColor = contourColor;
        _width = width;
        UpdateHelperPoints();
    }

    // --- GETTERY (Publiczne) ---
    public Color GetContentColor() => _contentColor;
    public Color GetContourColor() => _contourColor;
    public int GetWidth() => _width;
    public Point GetStartPoint() => _startPoint;
    public Point GetOppositePoint() => _oppositePoint;
    public double GetTransparency() => _transparency;
    public IEnumerable<Point> GetPoints() => new List<Point> {_startPoint, _oppositePoint};

    // --- SETERY (Publiczne) ---
    public void SetPoints(List<Point> points)
    {
        if (IsBlocked) return;
        
        if (points.Count < 2)
        {
            return;
        }
        _startPoint = points[0];
        _oppositePoint = points[1];
        UpdateHelperPoints();
    }
    
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
    
    public void SetTransparency(double transparency)
    {
        _transparency = transparency;
    }

    // --- SKALOWANIE ---
    public void Scale(ScaleHandle handle, Point newPos)
    {
        if (IsBlocked) return;

        // Pobieramy aktualne granice (Bounding Box) samego prostokąta
        var left = GetMinX();
        var right = GetMaxX();
        var top = GetMinY();
        var bottom = GetMaxY();

        // 1. Wyznaczamy punkt Pivot (punkt stały) dla samego prostokąta
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

        // 2. Obliczamy współczynniki skalowania
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

        // 3. Aplikujemy transformację
        ScaleTransform(pivot, sx, sy);
    }

    public void ScaleTransform(Point pivot, double sx, double sy)
    {
        if (IsBlocked) return;

        // Każdy punkt prostokąta transformujemy względem Pivotu
        _startPoint = TransformPoint(_startPoint, pivot, sx, sy);
        _oppositePoint = TransformPoint(_oppositePoint, pivot, sx, sy);

        UpdateHelperPoints();
    }

    private static Point TransformPoint(Point p, Point pivot, double sx, double sy)
    {
        return new Point(
            pivot.X + (p.X - pivot.X) * sx,
            pivot.Y + (p.Y - pivot.Y) * sy
        );
    }

    // --- METODY GRANIC ---
    public double GetMinX() => Math.Min(_startPoint.X, _oppositePoint.X);
    public double GetMaxX() => Math.Max(_startPoint.X, _oppositePoint.X);
    public double GetMinY() => Math.Min(_startPoint.Y, _oppositePoint.Y);
    public double GetMaxY() => Math.Max(_startPoint.Y, _oppositePoint.Y);

    // --- GEOMETRIA (Również pilnuje blokady) ---
    public void Move(int dx, int dy)
    {
        if (IsBlocked) return;
        _startPoint = new Point(_startPoint.X + dx, _startPoint.Y + dy);
        _oppositePoint = new Point(_oppositePoint.X + dx, _oppositePoint.Y + dy);
        // Aktualizujemy punkty pomocnicze, jeśli są używane w renderowaniu
        _helperPoint1 = new Point(_startPoint.X, _oppositePoint.Y);
        _helperPoint2 = new Point(_oppositePoint.X, _startPoint.Y);
    }
    
    private void UpdateHelperPoints()
    {
        _helperPoint1 = new Point(_startPoint.X, _oppositePoint.Y);
        _helperPoint2 = new Point(_oppositePoint.X, _startPoint.Y);
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        // 1. Wyznaczamy krawędzie prostokąta obiektu
        var rectLeft = Math.Min(_startPoint.X, _oppositePoint.X);
        var rectRight = Math.Max(_startPoint.X, _oppositePoint.X);
        var rectTop = Math.Min(_startPoint.Y, _oppositePoint.Y);
        var rectBottom = Math.Max(_startPoint.Y, _oppositePoint.Y);

        // 2. Wyznaczamy krawędzie prostokąta zaznaczenia
        var selLeft = Math.Min(startPoint.X, oppositePoint.X);
        var selRight = Math.Max(startPoint.X, oppositePoint.X);
        var selTop = Math.Min(startPoint.Y, oppositePoint.Y);
        var selBottom = Math.Max(startPoint.Y, oppositePoint.Y);

        // 3. Test nachodzenia (AABB)
        var overlapX = rectLeft <= selRight &&
                       rectRight >= selLeft;
        var overlapY = rectTop <= selBottom &&
                       rectBottom >= selTop;

        return overlapX && overlapY;
    }
    
    // --- KOPIOWANIE
    public ICanvas Clone() => new Rectangle(_startPoint, _oppositePoint, _contentColor, _contourColor, _width)
    {
        IsBlocked = IsBlocked,
        IsVisible =  IsVisible,
        _transparency =  _transparency
    };

    public override string ToString() =>
        $"Rectangle from ({_startPoint}), ({_helperPoint1}), ({_oppositePoint}), ({_helperPoint2}) Color: {_contentColor} and {_contourColor}";

    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}