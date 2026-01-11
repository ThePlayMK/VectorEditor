using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class CutCommand(IEnumerable<ICanvas> elements) : ICommand
{
    private readonly List<ICanvas> _elementsToCut = elements.ToList();
    private readonly List<(Layer Parent, int Index, ICanvas Element)> _memento = new();

    public void Execute()
    {
        // 1. KOPIOWANIE: Najpierw dodajemy do schowka (wykorzystuje klonowanie)
        Clipboard.Copy(_elementsToCut);

        // 2. CZYSZCZENIE MEMENTO: Przygotowujemy dane do Undo
        _memento.Clear();

        // 3. USUWANIE: Usuwamy obiekty z ich warstw
        foreach (var element in _elementsToCut)
        {
            var parent = element.ParentLayer;
            if (parent == null)
            {
                continue;
            }

            var index = parent.GetChildIndex(element);
            _memento.Add((parent, index, element));

            // Korzystamy z Twojej metody Remove
            parent.Remove(element);
        }
    }

    public void Undo()
    {
        // Przywracamy w odwrotnej kolejności, aby zachować Z-Order
        for (var i = _memento.Count - 1; i >= 0; i--)
        {
            var item = _memento[i];
            // Korzystamy z Twojej metody Insert
            item.Parent.Insert(item.Index, item.Element);
        }
    }
}