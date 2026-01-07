using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.Core.Command.Select;

public class SelectionManager(CommandManager commandManager)
{
    private readonly List<ICanvas> _currentSelection = [];

    public IReadOnlyList<ICanvas> Selected => _currentSelection;

    /// <summary>
    /// Czyści zaznaczenie
    /// </summary>
    public void Clear()
    {
        _currentSelection.Clear();
    }

    /// <summary>
    /// Zaznaczenie pojedynczego elementu (np. kliknięcie)
    /// </summary>
    public void SelectSingle(ICanvas element)
    {
        _currentSelection.Clear();
        _currentSelection.Add(element);
    }

    public void AddRange(IEnumerable<ICanvas> elements)
    {
        _currentSelection.AddRange(elements);
    }

    /// <summary>
    /// Przełączenie zaznaczenia (Ctrl+Click)
    /// </summary>
    public void Toggle(ICanvas element)
    {
        if (!_currentSelection.Remove(element))
            _currentSelection.Add(element);
    }

    /// <summary>
    /// Zaznaczenie obszarem prostokątnym
    /// </summary>
    public void SelectArea(Layer layer, Point p1, Point p2)
    {
        var command = new SelectionCommand(layer, p1, p2, this);

        // wykonujemy komendę przez CommandManager
        commandManager.Execute(command);
    }

}
