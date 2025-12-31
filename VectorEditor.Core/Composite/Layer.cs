using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Composite;

public class Layer(string name) : ICanvas
{
    private string Name { get; set; } = name;
    private readonly List<ICanvas> _children = [];
    public Layer? ParentLayer { get; set; }
    
    public IEnumerable<ICanvas> GetChildren() => _children;
    public bool IsBlocked { get; set; } 
    
    public void Add(ICanvas canvas)
    {
        canvas.ParentLayer = this;
        _children.Add(canvas);
    }
    
    public void Insert(int index, ICanvas element)
    {
        element.ParentLayer = this;
        _children.Insert(index, element);
    }
    
    public void Remove(ICanvas canvas)
    {
        if (!_children.Contains(canvas))
        {
            return;
        }
        canvas.ParentLayer = null;
        _children.Remove(canvas);
    }
    
    public bool IsWithinBounds(Point startPoint, Point oppositePoint)
    {
        return _children.Count != 0 && 
               _children.Any(child => child.IsWithinBounds(startPoint, oppositePoint));
    }

    public int GetIndexOf(ICanvas element)
    {
        return _children.IndexOf(element);
    }

    public void ConsoleDisplay(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + Name);
        foreach (var child in _children)
        {
            child.ConsoleDisplay(depth + 2);
        }
    }
}