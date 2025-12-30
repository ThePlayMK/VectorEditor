using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Command;

public class GroupCommand(Layer targetLayer, Point p1, Point p2) : ICommand
{
    private List<ICanvas> _foundElements = [];
    public void Execute()
    {
        // 1. Normalizacja punktów zaznaczenia
        var topLeft = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
        var bottomRight = new Point(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));

        // 2. Wyszukanie elementów
        _foundElements = targetLayer.GetChildren()
            .Where(child => child.IsWithinBounds(topLeft, bottomRight))
            .ToList();

        // 3. Wypisanie wyników (tymczasowo, przed wprowadzeniem strategii)
        DisplayResults(targetLayer, _foundElements, topLeft, bottomRight);
    }

    public void Undo()
    {
        _foundElements.Clear();
    }
    
    private static void DisplayResults(Layer layer, List<ICanvas> elements, Point tl, Point br)
    {
        Console.WriteLine($"\n--- Grouping results in {layer.GetType().Name} ---");
        Console.WriteLine($"Selection Area: {tl} to {br}");
        
        if (elements.Count == 0)
        {
            Console.WriteLine("No objects found in the selected area.");
        }
        else
        {
            Console.WriteLine($"Found {elements.Count} element(s):");
            foreach (var element in elements)
            {
                element.ConsoleDisplay(2);
            }
        }
        Console.WriteLine("------------------------------------------\n");
    }
}