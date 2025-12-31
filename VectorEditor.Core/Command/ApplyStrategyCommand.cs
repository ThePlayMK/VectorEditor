using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Command;

public class ApplyStrategyCommand: ICommand
{
    private readonly List<ICanvas> _targets;
    private readonly IModificationStrategy _strategy;
    private readonly List<object?> _mementos = [];

    // Konstruktor dla pojedynczego elementu (np. całej Canvy)
    public ApplyStrategyCommand(IModificationStrategy strategy, ICanvas target)
    {
        _strategy = strategy;
        _targets = [target];
    }

    // Konstruktor dla zaznaczenia (listy elementów)
    public ApplyStrategyCommand(IModificationStrategy strategy, IEnumerable<ICanvas> targets)
    {
        _strategy = strategy;
        _targets = targets.ToList();
    }

    public void Execute()
    {
        _mementos.Clear();
        foreach (var target in _targets)
        {
            _mementos.Add(_strategy.Apply(target));
        }
    }

    public void Undo()
    {
        for (var i = 0; i < _targets.Count; i++)
        {
            _strategy.Undo(_targets[i], _mementos[i]);
        }
    }
}