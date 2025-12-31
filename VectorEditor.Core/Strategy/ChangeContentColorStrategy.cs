using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Strategy;

public class ChangeContentColorStrategy(string newColor) : IModificationStrategy
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
            case Line line: // linia ma tylko kontur, wybrałem że jeżeli zmieniamy content to kontur zostaje zmieniony, inne rozwiązanie to żeby nic się nie działo
                memento[line] = line.ContourColor;
                line.ContourColor = newColor;
                break;
            case IShape shape:
                memento[shape] = shape.ContentColor;
                shape.ContentColor = newColor;
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
            if (kvp.Key is Line line) // to samo co w ApplyRecursive
            {
                line.ContourColor = kvp.Value;
            }
            else
            {
                kvp.Key.ContentColor = kvp.Value;
            }
        }
    }
}