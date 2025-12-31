using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Triangle(Point firstPoint, Point secondPoint, Point thirdPoint, string contentColor, string contourColor, int width) : IShape
{
    private Point FirstPoint { get; set; } = firstPoint;
    private Point SecondPoint { get; set; } = secondPoint;
    private Point ThirdPoint { get; set; } = thirdPoint;
    public string ContentColor { get; set; } = contentColor;
    public string ContourColor { get; set; } = contourColor;
    private int Width { get; set; } = width;
    public string Name => "Triangle";
    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; } 
    
    public override string ToString() => 
        $"Rectangle from ({FirstPoint}), ({SecondPoint}), ({ThirdPoint}); Color: {ContentColor} and {ContourColor}, Width: {Width}px";
    
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        var h1 = new Point(Math.Min(startPoint.X, oppositePoint.X), Math.Min(startPoint.Y, oppositePoint.Y));
        var h2 = new Point(Math.Max(startPoint.X, oppositePoint.X), Math.Max(startPoint.Y, oppositePoint.Y));
        startPoint = h1;
        oppositePoint = h2;
        List<Point> vertices = [FirstPoint, SecondPoint, ThirdPoint];
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
        return IsPointInTriangle(center, FirstPoint, SecondPoint, ThirdPoint);
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
}