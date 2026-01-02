using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Strategy;

public class ScaleStrategy(ScaleHandle handle, Point newPos) : IModificationStrategy
{
    // Memento przechowuje oryginalne punkty, aby uniknąć błędów zaokrągleń
    private record ScaleMemento(Dictionary<ICanvas, List<Point>> State);

    public object? Apply(ICanvas target)
    {
        if (target.IsBlocked)
        {
            return null;
        }

        var state = new Dictionary<ICanvas, List<Point>>();
        CaptureState(target, state);

        target.Scale(handle, newPos);
        return new ScaleMemento(state);
    }

    public object? Apply(List<ICanvas> target)
    {
        var elements = target.Where(e => !e.IsBlocked).ToList();
        if (elements.Count == 0) return null;

        // 1. Obliczamy wspólny Bounding Box
        var minX = elements.Min(e => e.GetMinX());
        var maxX = elements.Max(e => e.GetMaxX());
        var minY = elements.Min(e => e.GetMinY());
        var maxY = elements.Max(e => e.GetMaxY());

        var pivot = CalculatePivot(handle, minX, maxX, minY, maxY);
        var (sx, sy) = CalculateScaleFactors(handle, newPos, minX, maxX, minY, maxY);

        var states = new Dictionary<ICanvas, List<Point>>();

        foreach (var element in elements)
        {
            // Zapisujemy punkty tego elementu i jego dzieci (rekurencja)
            CaptureState(element, states);

            // SKALOWANIE: Każdy element używa tego samego Pivotu i współczynników
            element.ScaleTransform(pivot, sx, sy);
        }

        return new ScaleMemento(states);
    }

    public void Undo(ICanvas target, object? memento)
    {
        if (memento is not ScaleMemento scaleMemento)
        {
            return;
        }

        foreach (var entry in scaleMemento.State)
        {
            // Przywracamy punkty bezpośrednio do każdego obiektu
            entry.Key.SetPoints(entry.Value);
        }
    }

    private static void CaptureState(ICanvas target, Dictionary<ICanvas, List<Point>> state)
    {
        // Zapisujemy punkty bieżącego obiektu (ToList() tworzy kopię!)
        state[target] = target.GetPoints().ToList();

        // Jeśli to Layer, musimy zebrać punkty od dzieci (rekurencja)
        if (target is not Layer layer)
        {
            return;
        }

        foreach (var child in layer.GetChildren())
        {
            CaptureState(child, state);
        }
    }
    // --- LOGIKA MATEMATYCZNA (PRZEKOPIOWANA Z LAYER LUB UPROSZCZONA) ---

    private Point CalculatePivot(ScaleHandle h, double minX, double maxX, double minY, double maxY)
    {
        return h switch
        {
            ScaleHandle.TopLeft => new Point(maxX, maxY),
            ScaleHandle.TopRight => new Point(minX, maxY),
            ScaleHandle.BottomLeft => new Point(maxX, minY),
            ScaleHandle.BottomRight => new Point(minX, minY),
            ScaleHandle.Left => new Point(maxX, (minY + maxY) / 2),
            ScaleHandle.Right => new Point(minX, (minY + maxY) / 2),
            ScaleHandle.Top => new Point((minX + maxX) / 2, maxY),
            ScaleHandle.Bottom => new Point((minX + maxX) / 2, minY),
            _ => new Point(minX, minY)
        };
    }

    private (double sx, double sy) CalculateScaleFactors(ScaleHandle h, Point pos, double minX, double maxX,
        double minY, double maxY)
    {
        var oldW = Math.Max(1, maxX - minX);
        var oldH = Math.Max(1, maxY - minY);
        var newW = oldW;
        var newH = oldH;

        switch (h)
        {
            case ScaleHandle.TopLeft:
                newW = maxX - pos.X;
                newH = maxY - pos.Y;
                break;
            case ScaleHandle.Top:
                newH = maxY - pos.Y;
                break;
            case ScaleHandle.TopRight:
                newW = pos.X - minX;
                newH = maxY - pos.Y;
                break;
            case ScaleHandle.Right:
                newW = pos.X - minX;
                break;
            case ScaleHandle.BottomRight:
                newW = pos.X - minX;
                newH = pos.Y - minY;
                break;
            case ScaleHandle.Bottom:
                newH = pos.Y - minY;
                break;
            case ScaleHandle.BottomLeft:
                newW = maxX - pos.X;
                newH = pos.Y - minY;
                break;
            case ScaleHandle.Left:
                newW = maxX - pos.X;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(h), h, null);
        }

        return (newW / oldW, newH / oldH);
    }
}