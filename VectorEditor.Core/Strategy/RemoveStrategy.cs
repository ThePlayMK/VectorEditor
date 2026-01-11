using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public class RemoveStrategy : IModificationStrategy
{
    // Memento przechowuje pary: obiekt i jego (były) rodzic
    private record RemovalMemento(ICanvas Child, Layer Parent, int Index);

    public object Apply(ICanvas target)
    {
        if (target.IsBlocked || target.ParentLayer == null)
        {
            return new List<RemovalMemento>();
        }

        var parent = target.ParentLayer;
        var index = parent.GetIndexOf(target); // Zapamiętujemy pozycję
        var memento = new RemovalMemento(target, parent, index);

        parent.Remove(target);

        return memento;
    }

    public void Undo(ICanvas target, object? memento)
    {
        if (memento is RemovalMemento rm)
        {
            rm.Parent.Insert(rm.Index, rm.Child);
        }
    }
}