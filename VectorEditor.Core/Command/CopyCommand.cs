using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class CopyCommand(IEnumerable<ICanvas> selectedElements)
{
    public void Execute()
    {
        // Kopiujemy zaznaczone elementy do schowka (wykorzystujemy klonowanie)
        Clipboard.Copy(selectedElements);
    }
}