using Avalonia.Media;
using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class ChangeContentColorStrategy(Color newColor) : IModificationStrategy
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
                if (!memento.ContainsKey(shape)) // WAŻNE – nie nadpisujemy wcześniejszego koloru
                    memento[shape] = shape.GetContentColor();
                shape.SetContentColor(newColor);
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
            kvp.Key.SetContentColor(kvp.Value);
        }
    }
}