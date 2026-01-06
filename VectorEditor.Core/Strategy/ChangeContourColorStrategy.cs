using Avalonia.Media;
using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class ChangeContourColorStrategy(Color newColor) : IModificationStrategy
{
    public object Apply(ICanvas target)
    {
        var oldColors = new Dictionary<IShape, Color>();
        ApplyRecursive(target, oldColors);
        return oldColors;
    }

    private void ApplyRecursive(ICanvas target, Dictionary<IShape, Color> memento)
    {
        if (target.IsBlocked)
        {
            return;
        }
        
        switch (target)
        {
            case IShape shape:
                memento[shape] = shape.GetContourColor();
                shape.SetContourColor(newColor);
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
        if (memento is not Dictionary<IShape, Color> oldColors || oldColors.Count == 0)
            return;

        foreach (var kvp in oldColors)
        {
            kvp.Key.SetContourColor(kvp.Value);
        }
    }
}