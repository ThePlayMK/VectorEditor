using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Structures;

public class Circle(Point centerPoint, double radius, string contentColor, string contourColor, int width) : IShape
{
    private Point CenterPoint { get; set; } = centerPoint;
    private double Radius { get; set; } = radius;
    private string ContentColor { get; set; } = contentColor;
    public string ContourColor { get; set; } = contourColor;
    private int Width { get; set; } = width;
    public string Name => "Circle";
    
    public override string ToString() => 
        $"Circle Center: {CenterPoint}, Radius: {Radius}, Color: {ContentColor} and {ContourColor}, Width: {Width}px";
    
    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name + ": " + ToString());
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        // 1. Znajdź punkt na prostokącie (lub w jego środku), który jest najbliżej środka koła
        // Clampujemy współrzędne środka koła do zakresu prostokąta [tl, br]
        var closestX = Math.Clamp(CenterPoint.X, startPoint.X, oppositePoint.X);
        var closestY = Math.Clamp(CenterPoint.Y, startPoint.Y, oppositePoint.Y);

        // 2. Oblicz odległość (euklidesową) od środka koła do tego najbliższego punktu
        var dx = CenterPoint.X - closestX;
        var dy = CenterPoint.Y - closestY;

        // 3. Sprawdź, czy kwadrat odległości jest mniejszy lub równy kwadratowi promienia
        // (Unikamy wyciągania pierwiastka dla wydajności)
        return (dx * dx + dy * dy) <= (Radius * Radius);
    }
}