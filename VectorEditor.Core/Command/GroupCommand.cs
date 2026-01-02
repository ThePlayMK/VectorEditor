using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Command;

public class GroupCommand(Layer targetLayer, Point p1, Point p2) : ICommand
{
    private List<ICanvas> _foundElements = [];
    
    public IEnumerable<ICanvas> FoundElements => _foundElements;
    
    public void Execute()
    {
        var topLeft = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
        var bottomRight = new Point(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
        
        _foundElements = targetLayer.GetChildren()
            .Where(child => child.IsWithinBounds(topLeft, bottomRight))
            .ToList();
    }

    public void Undo()
    {
        _foundElements.Clear();
    }
    
    public void DisplayResults()
    {
        Console.WriteLine($"\n--- Grouping results in {targetLayer.GetType().Name} ---");
        Console.WriteLine($"Selection Area: {p1} to {p2}");
        
        if (_foundElements.Count == 0)
        {
            Console.WriteLine("No objects found in the selected area.");
        }
        else
        {
            Console.WriteLine($"Found {_foundElements.Count} element(s):");
            foreach (var element in _foundElements)
            {
                element.ConsoleDisplay(2);
            }
        }
        Console.WriteLine("------------------------------------------\n");
    }
}