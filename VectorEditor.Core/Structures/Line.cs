using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Structures;

public class Line(Point startPoint, Point endPoint, Color contourColor, int width) : IShape
{
    private Point _startPoint = startPoint;
    private Point _endPoint = endPoint;
    private Color _contourColor = contourColor;
    private int _width = width;
    private double _transparency = 0;

    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;
    public string Name => "Line";

    // --- GETTERY ---
    // Linia nie ma ContentColor, ale zwracamy ContourColor, by nie psuć generycznych operacji
    public Color GetContentColor() => _contourColor;
    public Color GetContourColor() => _contourColor;
    public int GetWidth() => _width;
    public Point GetStartPoint() => _startPoint;
    public Point GetEndPoint() => _endPoint;
    public double GetTransparency() => _transparency;
    public IEnumerable<Point> GetPoints() => new List<Point> { _startPoint, _endPoint };
    public double GetMinX() => Math.Min(_startPoint.X, _endPoint.X);
    public double GetMaxX() => Math.Max(_startPoint.X, _endPoint.X);
    public double GetMinY() => Math.Min(_startPoint.Y, _endPoint.Y);
    public double GetMaxY() => Math.Max(_startPoint.Y, _endPoint.Y);

    // --- SETTERY (Z LOGIKĄ BLOKADY) ---
    public void SetContentColor(Color color)
    {
        if (IsBlocked) return;
        // Zgodnie z logiką: zmiana contentu linii zmienia jej kontur
        _contourColor = color;
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

        if (points.Count < 2)
        {
            return;
        }

        _startPoint = points[0];
        _endPoint = points[1];
    }
    
    public void SetTransparency(double transparency)
    {
        _transparency = transparency;
    }

    // --- GEOMETRIA ---
    public void Move(int dx, int dy)
    {
        if (IsBlocked) return;
        _startPoint = new Point(_startPoint.X + dx, _startPoint.Y + dy);
        _endPoint = new Point(_endPoint.X + dx, _endPoint.Y + dy);
    }

    public void Scale(ScaleHandle handle, Point newPos)
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

        // Transformujemy oba końce linii względem Pivotu
        _startPoint = new Point(
            pivot.X + (_startPoint.X - pivot.X) * sx,
            pivot.Y + (_startPoint.Y - pivot.Y) * sy
        );

        _endPoint = new Point(
            pivot.X + (_endPoint.X - pivot.X) * sx,
            pivot.Y + (_endPoint.Y - pivot.Y) * sy
        );
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        // NORMALIZACJA ZAZNACZENIA
        var tl = new Point(Math.Min(startPoint.X, oppositePoint.X), Math.Min(startPoint.Y, oppositePoint.Y));
        var br = new Point(Math.Max(startPoint.X, oppositePoint.X), Math.Max(startPoint.Y, oppositePoint.Y));

        // 1. Sprawdź czy którykolwiek z końców linii jest wewnątrz prostokąta
        if (IsPointInRect(_startPoint, tl, br) || IsPointInRect(_endPoint, tl, br))
            return true;

        // 2. Sprawdź czy linia przecina którąkolwiek z krawędzi prostokąta
        return LineIntersectsRect(_startPoint, _endPoint, tl, br);
    }

    private static bool IsPointInRect(Point p, Point tl, Point br) =>
        p.X >= tl.X && p.X <= br.X && p.Y >= tl.Y && p.Y <= br.Y;

    private static bool LineIntersectsRect(Point p1, Point p2, Point tl, Point br)
    {
        // Sprawdzamy przecięcie z 4 krawędziami prostokąta zaznaczenia
        return LineIntersectsLine(p1, p2, tl, new Point(br.X, tl.Y)) || // Góra
               LineIntersectsLine(p1, p2, new Point(br.X, tl.Y), br) || // Prawo
               LineIntersectsLine(p1, p2, br, new Point(tl.X, br.Y)) || // Dół
               LineIntersectsLine(p1, p2, new Point(tl.X, br.Y), tl); // Lewo
    }

    private static bool LineIntersectsLine(Point a1, Point a2, Point b1, Point b2)
    {
        // Algorytm CCW (Counter-Clockwise) dla przecięć odcinków
        var d = (a2.X - a1.X) * (b2.Y - b1.Y) - (a2.Y - a1.Y) * (b2.X - b1.X);
        if (d == 0) return false; // Równoległe

        var u = ((b1.X - a1.X) * (b2.Y - b1.Y) - (b1.Y - a1.Y) * (b2.X - b1.X)) / d;
        var v = ((b1.X - a1.X) * (a2.Y - a1.Y) - (b1.Y - a1.Y) * (a2.X - a1.X)) / d;

        return u is >= 0 and <= 1 &&
               v is >= 0 and <= 1;
    }
    
    // --- KOPIOWANIE
    public ICanvas Clone() => new Line(_startPoint, _endPoint, _contourColor, _width)
    {
        IsBlocked = IsBlocked,
        IsVisible =  IsVisible,
        _transparency =  _transparency
    };

    public override string ToString() =>
        $"Line from {_startPoint} to {_endPoint}, ContourColor: {_contourColor}, Width: {_width}px";

    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}