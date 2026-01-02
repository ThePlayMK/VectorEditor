using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class ShowCanvasStrategy : IModificationStrategy
{
    public object Apply(ICanvas target)
    {
        // Zapamiętujemy poprzedni stan (jeśli był widoczny, Undo to przywróci)
        var previousState = target.IsVisible;
        target.IsVisible = true;
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