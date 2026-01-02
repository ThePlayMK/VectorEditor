using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class MoveCanvasStrategy(int dx, int dy) : IModificationStrategy
{
    public object Apply(ICanvas target)
    {
        if (target.IsBlocked) return false;

        target.Move(dx, dy);
        return true;
    }

    public void Undo(ICanvas target, object? memento)
    {
        if (memento is true)
        {
            target.Move(-dx, -dy);
        }
    }
}