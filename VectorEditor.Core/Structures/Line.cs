using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Line(Point startPoint, Point endPoint, string contourColor, int width) : IShape
{
    private Point StartPoint { get; set; } = startPoint;
    private Point EndPoint { get; set; } = endPoint;
    public string ContourColor { get; set; } = contourColor;
    public string ContentColor { get; set; } = string.Empty;
    private int Width { get; set; } = width;
    public string Name => "Line";
    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; } 
    public bool IsVisible { get; set; } = true;
    
    public override string ToString() => 
        $"Line from {StartPoint} to {EndPoint}, ContourColor: {ContourColor}, Width: {Width}px";
    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
    
    public void Move(int dx, int dy)
    {
        if (IsBlocked) return;
        StartPoint = new Point(StartPoint.X + dx, StartPoint.Y + dy);
        EndPoint = new Point(EndPoint.X + dx, EndPoint.Y + dy);
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        // NORMALIZACJA ZAZNACZENIA
        var tl = new Point(Math.Min(startPoint.X, oppositePoint.X), Math.Min(startPoint.Y, oppositePoint.Y));
        var br = new Point(Math.Max(startPoint.X, oppositePoint.X), Math.Max(startPoint.Y, oppositePoint.Y));
        
        // 1. Sprawdź czy którykolwiek z końców linii jest wewnątrz prostokąta
        if (IsPointInRect(StartPoint, tl, br) || IsPointInRect(EndPoint, tl, br))
            return true;

        // 2. Sprawdź czy linia przecina którąkolwiek z krawędzi prostokąta
        return LineIntersectsRect(StartPoint, EndPoint, tl, br);
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
}