using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Structures;

public class Circle(Point centerPoint, double radius, string contentColor, string contourColor, int width) : IShape
{
    private Point _centerPoint = centerPoint;
    private double _radius = radius;
    private string _contentColor = contentColor;
    private string _contourColor = contourColor;
    private int _width = width;

    public string Name => "Circle";
    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; } 
    public bool IsVisible { get; set; } = true;
    
    // --- GETTERY ---
    public string GetContentColor() => _contentColor;
    public string GetContourColor() => _contourColor;
    public int GetWidth() => _width;
    public Point GetCenterPoint() => _centerPoint;
    public double GetRadius() => _radius;
    
    public IEnumerable<Point> GetPoints()
    {
        throw new NotImplementedException();
    }
    
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

    public void SetRadius(double radius)
    {
        if (IsBlocked) return;
        _radius = radius;
    }
    
    public void SetPoints(List<Point> scaleMementoPoints)
    {
        throw new NotImplementedException();
    }
    
    // --- GEOMETRIA ---
    
    public void Move(int dx, int dy)
    {
        if (IsBlocked) return;
        _centerPoint = new Point(_centerPoint.X + dx, _centerPoint.Y + dy);
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
        // 1. Znajdź punkt na prostokącie (lub w jego środku), który jest najbliżej środka koła
        // Clampujemy współrzędne środka koła do zakresu prostokąta [tl, br]
        var closestX = Math.Clamp(_centerPoint.X, startPoint.X, oppositePoint.X);
        var closestY = Math.Clamp(_centerPoint.Y, startPoint.Y, oppositePoint.Y);

        // 2. Oblicz odległość (euklidesową) od środka koła do tego najbliższego punktu
        var dx = _centerPoint.X - closestX;
        var dy = _centerPoint.Y - closestY;

        // 3. Sprawdź, czy kwadrat odległości jest mniejszy lub równy kwadratowi promienia
        // (Unikamy wyciągania pierwiastka dla wydajności)
        return (dx * dx + dy * dy) <= (_radius * _radius);
    }
    
    public override string ToString() => 
        $"Circle Center: {_centerPoint}, Radius: {_radius}, Color: {_contentColor} and {_contourColor}, Width: {_width}px";
    
    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible) return;
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }
}