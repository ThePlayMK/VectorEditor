using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class ChangeContourColorStrategy(string newColor) : IModificationStrategy
{
    public object Apply(ICanvas target)
    {
        var oldColors = new Dictionary<IShape, string>();
        ApplyRecursive(target, oldColors);
        return oldColors;
    }

    private void ApplyRecursive(ICanvas target, Dictionary<IShape, string> memento)
    {
        switch (target)
        {
            case IShape shape:
                memento[shape] = shape.ContourColor;
                shape.ContourColor = newColor;
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
        if (memento is not Dictionary<IShape, string> oldColors || oldColors.Count == 0)
            return;

        foreach (var kvp in oldColors)
        {
            kvp.Key.ContourColor = kvp.Value;
        }
    }
}