namespace VectorEditor.Core.Composite;

public class Layer(string name) : ICanvas
{
    private string Name { get; set; } = name;
    private readonly List<ICanvas> _children = [];
    
    public void Add(ICanvas canvas)
    {
        _children.Add(canvas);
    }
    
    public void Remove(ICanvas canvas)
    {
        _children.Remove(canvas);
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