using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Structures;

public class Triangle(Point firstPoint, Point secondPoint, Point thirdPoint, string contentColor, string contourColor, int width) : IShape
{
    private Point _firstPoint = firstPoint;
    private Point _secondPoint = secondPoint;
    private Point _thirdPoint = thirdPoint;
    private string _contentColor = contentColor;
    private string _contourColor = contourColor;
    private int _width = width;

    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;
    public string Name => "Triangle";
    
    // --- GETTERY ---
    public string GetContentColor() => _contentColor;
    public string GetContourColor() => _contourColor;
    public int GetWidth() => _width;
    public Point GetFirstPoint() => _firstPoint;
    public Point GetSecondPoint() => _secondPoint;
    public Point GetThirdPoint() => _thirdPoint;
    
    // --- SETTERY (Z LOGIKĄ BLOKADY) ---
    public void SetContentColor(string color)
    {
        if (IsBlocked) return;
        _contentColor = color;
    }

    public void SetContourColor(string color)
    {
        if (IsBlocked) return;
        _contourColor = color;
    }

    public void SetWidth(int width)
    {
        if (IsBlocked) return;
        _width = width;
    }
    
    // --- GEOMETRIA ---
    
    public void Move(int dx, int dy)
    {
        if (IsBlocked) return;
        _firstPoint = new Point(_firstPoint.X + dx, _firstPoint.Y + dy);
        _secondPoint = new Point(_secondPoint.X + dx, _secondPoint.Y + dy);
        _thirdPoint = new Point(_thirdPoint.X + dx, _thirdPoint.Y + dy);
    }
    
    public void Scale(ScaleHandle handle, Point newPos)
        {
            throw new NotImplementedException();
        }

    public void ScaleTransform(Point pivot, double sx, double sy)
    {
        throw new NotImplementedException();
    }

    public double GetMinX()
    {
        throw new NotImplementedException();
    }

    public double GetMaxX()
    {
        throw new NotImplementedException();
    }

    public double GetMinY()
    {
        throw new NotImplementedException();
    }

    public double GetMaxY()
    {
        throw new NotImplementedException();
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
        {
            var h1 = new Point(Math.Min(startPoint.X, oppositePoint.X), Math.Min(startPoint.Y, oppositePoint.Y));
            var h2 = new Point(Math.Max(startPoint.X, oppositePoint.X), Math.Max(startPoint.Y, oppositePoint.Y));
            startPoint = h1;
            oppositePoint = h2;
            List<Point> vertices = [_firstPoint, _secondPoint, _thirdPoint];
            // 1. Czy jakikolwiek wierzchołek trójkąta jest wewnątrz zaznaczenia?
            if (vertices.Any(p => p.X >= startPoint.X && p.X <= oppositePoint.X && p.Y >= startPoint.Y && p.Y <= oppositePoint.Y))
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
               LineIntersectsLine(s, e, new Point(tl.X, br.Y), tl);   // Lewo
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
    
    public override string ToString() => 
        $"Rectangle from ({_firstPoint}), ({_secondPoint}), ({_thirdPoint}); Color: {_contentColor} and {_contourColor}, Width: {_width}px";
    
    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
    
}