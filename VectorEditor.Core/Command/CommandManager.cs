using VectorEditor.Core.State;

namespace VectorEditor.Core.Command;

public class CommandManager(EditorContext editorContext)
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

        editorContext.Modify();
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

        editorContext.Modify();
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

        editorContext.Modify();
        OnChanged?.Invoke();
    }
}