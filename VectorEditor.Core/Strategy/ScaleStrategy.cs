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

        // Na razie obsługujemy tylko prostokąty (zgodnie z planem)
        /*if (target is Rectangle rect)
        {
            memento = new ScaleMemento(
                new Point(rect.GetStartPoint().X, rect.GetStartPoint().Y),
                new Point(rect.GetOppositePoint().X, rect.GetOppositePoint().Y)
            );
        }*/

        CaptureState(target, state);

        target.Scale(handle, newPos);
        return new ScaleMemento(state);
        
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
}