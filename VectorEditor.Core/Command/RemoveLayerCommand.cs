using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class RemoveLayerCommand(Layer layer) : ICommand
{
    private readonly Layer _parent = layer.ParentLayer!;
    private int _index;

    public void Execute()
    {
        _index = _parent.GetChildIndex(layer);
        _parent.Remove(layer);
    }

    public void Undo()
    {
        _parent.Insert(_index, layer);
    }
}