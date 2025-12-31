using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Strategy;

public class ScaleStrategy(ScaleHandle handle, Point newPos) : IModificationStrategy
{
    // Memento przechowuje oryginalne punkty, aby uniknąć błędów zaokrągleń
    private record ScaleMemento(Point Start, Point Opposite);

    public object Apply(ICanvas target)
    {
        if (target.IsBlocked) return null;

        // Na razie obsługujemy tylko prostokąty (zgodnie z planem)
        if (target is Rectangle rect)
        {
            var memento = new ScaleMemento(
                new Point(rect.GetStartPoint().X, rect.GetStartPoint().Y), 
                new Point(rect.GetOppositePoint().X, rect.GetOppositePoint().Y)
            );
            
            target.Scale(handle, newPos);
            return memento;
        }

        return null;
    }

    public void Undo(ICanvas target, object? memento)
    {
        if (target is Rectangle rect && memento is ScaleMemento sm)
        {
            // Przywracamy stan sprzed skalowania
            // Musielibyśmy dodać metodę SetPoints lub po prostu użyć Scale z oryginałem
            // Najprościej: stworzyć metodę Restore(Point s, Point o) w Rectangle
            rect.Scale(ScaleHandle.TopLeft, sm.Start); // To ustawi TL
            rect.Scale(ScaleHandle.BottomRight, sm.Opposite); // To ustawi BR
        }
    }
}