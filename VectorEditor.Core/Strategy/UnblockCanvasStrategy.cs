using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class UnblockCanvasStrategy : IModificationStrategy
{
    public object Apply(ICanvas target)
    {
        var states = new Dictionary<ICanvas, bool>();
        ApplyRecursive(target, states);
        return states;
    }

    private static void ApplyRecursive(ICanvas target, Dictionary<ICanvas, bool> states)
    {
        states[target] = target.IsBlocked;
        target.IsBlocked = false;

        if (target is not Layer layer)
        {
            return;
        }
        foreach (var child in layer.GetChildren())
        {
            ApplyRecursive(child, states);
        }
    }

    public void Undo(ICanvas target, object? memento)
    {
        if (memento is Dictionary<ICanvas, bool> states)
        {
            foreach (var kvp in states)
            {
                kvp.Key.IsBlocked = kvp.Value;
            }
        }
    }
}