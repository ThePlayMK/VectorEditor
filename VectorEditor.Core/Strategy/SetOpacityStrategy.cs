using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class SetOpacityStrategy(double opacity) : IModificationStrategy
{
    private readonly double _opacity = Math.Clamp(opacity, 0, 100);
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
                memento[shape] = shape.GetOpacity();
                shape.SetOpacity(_opacity);
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

    public void PreviewApply(IReadOnlyList<ICanvas> targets)
    {
        foreach (var target in targets)
        {
            PreviewApplyRecursive(target);
        }
    }
    
    public void PreviewApplyRecursive(ICanvas target)
    {
        if (target.IsBlocked)
        {
            return;
        }

        switch (target)
        {
            case IShape shape:
                shape.SetOpacity(_opacity);
                break;
            case Layer layer:
            {
                foreach (var child in layer.GetChildren())
                {
                    PreviewApplyRecursive(child);
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
            kvp.Key.SetOpacity(kvp.Value);
        }
    }
}