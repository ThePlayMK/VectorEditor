using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class PasteCommand(Layer targetLayer) : ICommand
{
    private readonly List<ICanvas> _pastedElements = [];
    private const int Dx = 10;
    private const int Dy = 10;
    private bool _executedOnce;

    public IReadOnlyList<ICanvas> PastedElements => _pastedElements;

    public void Execute()
    {
        if (!_executedOnce)
        {
            var clones = Clipboard.Paste().ToList();

            foreach (var c in clones)
            {
                c.Move(Dx, Dy);
                c.ParentLayer = targetLayer;
                _pastedElements.Add(c);
            }

            _executedOnce = true;
        }

        // dodaj TE SAME instancje do warstwy
        foreach (var obj in _pastedElements)
            targetLayer.Add(obj);
    }

    public void Undo()
    {
        // Usuwamy dokładnie te elementy, które wkleiliśmy
        foreach (var element in _pastedElements)
        {
            targetLayer.Remove(element);
        }
    }
}