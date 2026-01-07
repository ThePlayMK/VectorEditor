using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class AddLayerCommand(Layer parentLayer) : ICommand
{
    private Layer? _createdLayer;

    public void Execute()
    {
        // Tworzymy warstwę tylko raz
        _createdLayer ??= new Layer($"Layer {parentLayer.GetChildren().Count()}");

        parentLayer.Add(_createdLayer);
    }

    public void Undo()
    {
        if (_createdLayer != null)
            parentLayer.Remove(_createdLayer);
    }
}