namespace VectorEditor.Core.Command;

public enum CommandChangeType
{
    Execute,
    Undo,
    Redo
}


public class CommandManager
{
    private readonly Stack<ICommand> _undoStack = [];
    private readonly Stack<ICommand> _redoStack = [];
    public event Action? OnChanged;

    
    public void Execute(ICommand command)
    {
        //Console.WriteLine($"EXEC: {command.GetType().Name}");
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
        
        OnChanged?.Invoke();
    }
    
    public void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }
        
        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);
        
        OnChanged?.Invoke();
    }
    
    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }
        
        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        
        OnChanged?.Invoke();
    }
}