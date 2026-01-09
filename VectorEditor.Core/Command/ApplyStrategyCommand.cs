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
        if (_mementos.Count > 0) // już wykonana wcześniej
        {
            // redo – tylko wywołujemy strategie bez tworzenia nowych mement
            foreach (var (target, memento) in _targets.Zip(_mementos, (t,m)=> (t,m)))
                _strategy.Apply(target); // lub specjalnie redo, jeśli strategy ma redo
            return;
        }

        // pierwsze wykonanie – tworzymy mementa
        _mementos.Clear();
        foreach (var target in _targets)
            _mementos.Add(_strategy.Apply(target));
    }

    public void Undo()
    {
        for (var i = _targets.Count - 1; i >= 0; i--)
        {
            _strategy.Undo(_targets[i], _mementos[i]);
        }
    }
}