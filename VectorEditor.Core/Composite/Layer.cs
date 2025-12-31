using VectorEditor.Core.Strategy;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Composite;

public class Layer(string name) : ICanvas
{
    private readonly List<ICanvas> _children = [];
    public Layer? ParentLayer { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;

    // --- GETTERY ---
    public string GetName() => name;
    public IEnumerable<ICanvas> GetChildren() => _children;

    // --- ZARZĄDZANIE STRUKTURĄ ---
    public void Add(ICanvas canvas)
    {
        if (IsBlocked) return;

        canvas.ParentLayer = this;
        _children.Add(canvas);
    }

    public void Insert(int index, ICanvas element)
    {
        if (IsBlocked) return;

        element.ParentLayer = this;
        _children.Insert(index, element);
    }

    public void Remove(ICanvas canvas)
    {
        if (IsBlocked) return;

        if (!_children.Contains(canvas))
        {
            return;
        }

        canvas.ParentLayer = null;
        _children.Remove(canvas);
    }

    public void Move(int dx, int dy)
    {
        if (IsBlocked) return;

        foreach (var child in _children)
        {
            child.Move(dx, dy);
        }
    }

    // --- METODY GRANIC (REKURENCYJNE) ---
    public double GetMinX() => _children.Count == 0 ? 0 : _children.Min(c => c.GetMinX());
    public double GetMaxX() => _children.Count == 0 ? 0 : _children.Max(c => c.GetMaxX());
    public double GetMinY() => _children.Count == 0 ? 0 : _children.Min(c => c.GetMinY());
    public double GetMaxY() => _children.Count == 0 ? 0 : _children.Max(c => c.GetMaxY());

    // --- SKALOWANIE ---
    public void Scale(ScaleHandle handle, Point newPos)
    {
        if (IsBlocked || _children.Count == 0) return;

        // 1. Obliczamy Bounding Box warstwy
        var left = GetMinX();
        var right = GetMaxX();
        var top = GetMinY();
        var bottom = GetMaxY();

        // 2. Wyznaczamy Pivot (punkt stały - naprzeciwko uchwytu)
        var pivot = handle switch
        {
            ScaleHandle.TopLeft => new Point(right, bottom),
            ScaleHandle.Top => new Point(left, bottom), // Arbitralny X (dół warstwy)
            ScaleHandle.TopRight => new Point(left, bottom),
            ScaleHandle.Right => new Point(left, top),
            ScaleHandle.BottomRight => new Point(left, top),
            ScaleHandle.Bottom => new Point(left, top),
            ScaleHandle.BottomLeft => new Point(right, top),
            ScaleHandle.Left => new Point(right, top),
            _ => new Point(left, top)
        };

        // 3. Obliczamy współczynniki skalowania
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

        // 4. Skalujemy wszystkie dzieci względem wspólnego Pivotu
        ScaleTransform(pivot, sx, sy);
    }

    public void ScaleTransform(Point pivot, double sx, double sy)
    {
        if (IsBlocked) return;
        foreach (var child in _children)
        {
            child.ScaleTransform(pivot, sx, sy);
        }
    }

    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        return _children.Count != 0 &&
               _children.Any(child => child.IsWithinBounds(startPoint, oppositePoint));
    }

    public int GetIndexOf(ICanvas element)
    {
        return _children.IndexOf(element);
    }

    public void ConsoleDisplay(int depth = 0)
    {
        if (!IsVisible)
        {
            Console.WriteLine(new string('-', depth) + "Layer: " + name + " [HIDDEN]");
            return;
        }

        Console.WriteLine(new string('-', depth) + name);
        foreach (var child in _children)
        {
            child.ConsoleDisplay(depth + 2);
        }
    }
}