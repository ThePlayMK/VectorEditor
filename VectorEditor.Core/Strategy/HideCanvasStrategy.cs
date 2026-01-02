using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class HideCanvasStrategy : IModificationStrategy
{
    public object Apply(ICanvas target)
    {
        // Zapamiętujemy poprzedni stan konkretnego obiektu
        var previousState = target.IsVisible;
        target.IsVisible = false;
        return previousState;
    }

    public void Undo(ICanvas target, object? memento)
    {
        if (memento is bool previousState)
        {
            target.IsVisible = previousState;
        }
    }
}