using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class MoveCanvasStrategy(double dx, double dy) : IModificationStrategy
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