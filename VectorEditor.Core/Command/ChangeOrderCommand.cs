using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class ChangeOrderCommand(ICanvas target, OrderAction action) : ICommand
{
    private Layer? _parent;
    private int _oldIndex = -1;

    public void Execute()
    {
        _parent = target.ParentLayer;
        
        // Jeśli obiekt nie ma rodzica, nie możemy zmienić jego kolejności
        if (_parent == null) return;

        // 1. Zapamiętujemy stary indeks dla Undo
        _oldIndex = _parent.GetChildIndex(target);
        if (_oldIndex == -1) return;

        // 2. Obliczamy nową pozycję
        // Używamy GetChildren().Count(), bo jeden element zaraz usuniemy
        var currentCount = _parent.GetChildren().Count();
        var newIndex = CalculateNewIndex(_oldIndex, currentCount);

        // 3. Wykonujemy operację używając Twoich metod
        _parent.Remove(target);
        _parent.Insert(newIndex, target);
    }

    public void Undo()
    {
        if (_parent == null || _oldIndex == -1) return;

        // Przywracamy obiekt na dokładnie to samo miejsce
        _parent.Remove(target);
        _parent.Insert(_oldIndex, target);
    }

    private int CalculateNewIndex(int currentIndex, int count)
    {
        return action switch
        {
            OrderAction.BringToFront => count - 1,
            OrderAction.SendToBack => 0,
            OrderAction.BringForward => Math.Min(count - 1, currentIndex + 1),
            OrderAction.SendBackward => Math.Max(0, currentIndex - 1),
            _ => currentIndex
        };
    }
}