using VectorEditor.Core.Command.Select;
using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class PasteCommand(Layer targetLayer, SelectionManager selection) : ICommand
{
    private List<ICanvas> _pastedElements = [];
    private const int Dx = 10;
    private const int Dy = 10;

    public void Execute()
    {
        _pastedElements.Clear();
        // 1. Pobieramy klony ze schowka
        var clones = Clipboard.Paste().ToList();

        // 2. Dodajemy je do warstwy i opcjonalnie przesuwamy
        foreach (var element in clones)
        {
            // Przesunięcie, aby użytkownik widział, że wkleił obiekt
            element.Move(Dx, Dy); 
            targetLayer.Add(element);
            _pastedElements.Add(element);
        }
        selection.Clear();
        selection.AddRange(_pastedElements);
    }

    public void Undo()
    {
        // Usuwamy dokładnie te elementy, które wkleiliśmy
        foreach (var element in _pastedElements)
        {
            targetLayer.Remove(element);
        }
        
        selection.Clear();
    }
}