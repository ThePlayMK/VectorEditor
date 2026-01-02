using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class SetTransparencyStrategy(double transparency) : IModificationStrategy
{
    private readonly double _transparency = Math.Clamp(transparency, 0, 100);
    public object Apply(ICanvas target)
    {
        var oldTransparencies = new Dictionary<IShape, double>();
        ApplyRecursive(target, oldTransparencies);
        return oldTransparencies ;
    }

    private void ApplyRecursive(ICanvas target, Dictionary<IShape, double> memento)
    {
        if (target.IsBlocked)
        {
            return;
        }

        switch (target)
        {
            case IShape shape:
                memento[shape] = shape.GetTransparency();
                shape.SetTransparency(_transparency);
                break;
            case Layer layer:
            {
                foreach (var child in layer.GetChildren())
                {
                    ApplyRecursive(child, memento);
                }

                break;
            }
        }
    }

    public void Undo(ICanvas target, object? memento)
    {
        if (memento is not Dictionary<IShape, double> oldColors || oldColors.Count == 0)
            return;

        foreach (var kvp in oldColors)
        {
            kvp.Key.SetTransparency(kvp.Value);
        }
    }
}