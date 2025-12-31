using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Structures;

public class Line(Point startPoint, Point endPoint, string contourColor, int width) : IShape
{
    private Point _startPoint = startPoint;
    private Point _endPoint = endPoint;
    private string _contourColor = contourColor;
    private int _width = width;

    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;
    public string Name => "Line";
    
    // --- GETTERY ---
    // Linia nie ma ContentColor, ale zwracamy ContourColor, by nie psuć generycznych operacji
    public string GetContentColor() => _contourColor;
    public string GetContourColor() => _contourColor;
    public int GetWidth() => _width;
    public Point GetStartPoint() => _startPoint;
    public Point GetEndPoint() => _endPoint;
    
    // --- SETTERY (Z LOGIKĄ BLOKADY) ---
    public void SetContentColor(string color)
    {
        if (IsBlocked) return;
        // Zgodnie z logiką: zmiana contentu linii zmienia jej kontur
        _contourColor = color;
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
            _startPoint = new Point(_startPoint.X + dx, _startPoint.Y + dy);
            _endPoint = new Point(_endPoint.X + dx, _endPoint.Y + dy);
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
               LineIntersectsLine(p1, p2, new Point(tl.X, br.Y), tl);   // Lewo
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
    
    public override string ToString() => 
        $"Line from {_startPoint} to {_endPoint}, ContourColor: {_contourColor}, Width: {_width}px";

    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
    
}