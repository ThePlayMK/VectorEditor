using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class LayerOrganisationStrategy(IEnumerable<ICanvas> canvases, Layer layer, int targetIndex)
    : IModificationStrategy
{
    private readonly List<ICanvas> _canvases = canvases.ToList();
    private List<int>? _originalIndices;

    public object Apply(ICanvas _)
    {
        // 1. Zapisz oryginalne indeksy
        _originalIndices = _canvases.Select(layer.GetIndexOf).ToList();

        // 2. Usuń elementy z warstwy
        foreach (var canvas in _canvases)
        {
            layer.Remove(canvas);
        }

        // 3. Wstaw elementy w nowe miejsce
        int insertIndex = Math.Min(targetIndex, layer.GetChildren().Count());
        foreach (var canvas in _canvases)
        {
            layer.Insert(insertIndex, canvas);
            insertIndex++;
        }

        return _originalIndices;
    }

    // Undo – przywraca elementy na poprzednie indeksy
    public void Undo(ICanvas _, object? memento)
    {
        if (_originalIndices == null) return;

        var originalIndices = (List<int>)memento!;

        // Usuń elementy z warstwy
        foreach (var canvas in _canvases)
        {
            layer.Remove(canvas);
        }

        // Wstaw elementy z powrotem na ich oryginalne miejsca
        for (int i = 0; i < _canvases.Count; i++)
        {
            int index = Math.Min(originalIndices[i], layer.GetChildren().Count());
            layer.Insert(index, _canvases[i]);
        }
    }
}