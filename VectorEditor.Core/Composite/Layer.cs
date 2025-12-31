using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Composite;

public class Layer(string name) : ICanvas
{
    public string Name { get; set; } = name;
    private readonly List<ICanvas> _children = [];
    public Layer? ParentLayer { get; set; }
    public IEnumerable<ICanvas> GetChildren() => _children;
    public bool IsBlocked { get; set; }
    public bool IsVisible { get; set; } = true;
    
    public void Add(ICanvas canvas)
    {
        if (IsBlocked) return;
        
        canvas.ParentLayer = this;
        _children.Add(canvas);
    }
    
    public void Insert(int index, ICanvas element)
    {
        if (IsBlocked) return;
        
        element.ParentLayer = this;
        _children.Insert(index, element);
    }
    
    public void Remove(ICanvas canvas)
    {
        if (IsBlocked) return;
        
        if (!_children.Contains(canvas))
        {
            return;
        }
        canvas.ParentLayer = null;
        _children.Remove(canvas);
    }
    
    public void Move(int dx, int dy)
    {
        if (IsBlocked) return;
        
        foreach (var child in _children)
        {
            child.Move(dx, dy);
        }
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
        if (!IsVisible) 
        {
            Console.WriteLine(new string('-', depth) + "Layer: " + Name + " [HIDDEN]");
            return; 
        }
        
        Console.WriteLine(new string('-', depth) + Name);
        foreach (var child in _children)
        {
            child.ConsoleDisplay(depth + 2);
        }
    }

    
}