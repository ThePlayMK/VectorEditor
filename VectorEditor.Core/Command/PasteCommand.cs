using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class PasteCommand(Layer targetLayer) : ICommand
{
    private List<ICanvas> _pastedElements = new();

    public void Execute()
    {
        // 1. Pobieramy klony ze schowka
        _pastedElements = Clipboard.Paste().ToList();

        // 2. Dodajemy je do warstwy i opcjonalnie przesuwamy
        foreach (var element in _pastedElements)
        {
            // Przesunięcie, aby użytkownik widział, że wkleił obiekt
            element.Move(10, 10); 
            targetLayer.Add(element);
        }
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