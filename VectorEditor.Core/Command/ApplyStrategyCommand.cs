using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;

namespace VectorEditor.Core.Command;

public class ApplyStrategyCommand: ICommand
{
    private readonly List<ICanvas> _targets;
    private readonly IModificationStrategy _strategy;
    private readonly List<object?> _mementos = [];
    private ICanvas? _activeTarget;

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

        // 1. Logika grupowania specjalnie dla ScaleStrategy
        if (_strategy is ScaleStrategy && _targets.Count > 1)
        {
            var originalParents = _targets.ToDictionary(t => t, t => t.ParentLayer);
            var proxy = new Layer("temporary_scale_group");
            foreach (var t in _targets) proxy.Add(t);
            
            _activeTarget = proxy;
            _mementos.Add(_strategy.Apply(proxy));
            
            foreach (var t in _targets)
            {
                t.ParentLayer = originalParents[t];
            }
        }
        else
        {
            // 2. Standardowe zachowanie (pojedynczo)
            foreach (var target in _targets)
            {
                _mementos.Add(_strategy.Apply(target));
            }
        }
    }

    public void Undo()
    {
        if (_strategy is ScaleStrategy && _targets.Count > 1)
        {
            _strategy.Undo(null!, _mementos[0]);
            return;
        }
        for (var i = _targets.Count - 1; i >= 0; i--)
        {
            _strategy.Undo(_targets[i], _mementos[i]);
        }
    }
}