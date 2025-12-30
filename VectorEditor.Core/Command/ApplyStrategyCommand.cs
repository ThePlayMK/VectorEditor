using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Command;

public class ApplyStrategyCommand(IModificationStrategy strategy, ICanvas canvas) : ICommand
{
    private object? _memento;

    public void Execute()
    {
        // Wykonujemy strategię i zapamiętujemy stan poprzedni
        _memento = strategy.Apply(canvas);
    }

    public void Undo()
    {
        // Przywracamy stan poprzedni przy użyciu zapamiętanej pamiątki
        if (_memento != null)
        {
            strategy.Undo(canvas, _memento);
        }
    }
}