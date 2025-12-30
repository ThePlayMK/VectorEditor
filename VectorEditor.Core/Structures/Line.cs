using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Line(Point startPoint, Point endPoint, string color, int width) : IShape
{
    private Point StartPoint { get; set; } = startPoint;
    private Point EndPoint { get; set; } = endPoint;
    private string Color { get; set; } = color;
    private int Width { get; set; } = width;
    public string Name => "Line";
    
    public override string ToString() => 
        $"Line from {StartPoint} to {EndPoint}, Color: {Color}, Width: {Width}px";
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }

    public bool IsWithinBounds(Point tl, Point br)
    {
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