using VectorEditor.Core.Builder;
using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class AddShapeCommand(IShapeBuilder builder, Layer targetLayer) : ICommand
{
    private IShape? _createdShape;

    public void Execute()
    {
        if (targetLayer.IsBlocked)
        {
            return;
        }
        
        _createdShape = builder.Build();
        targetLayer.Add(_createdShape);
    }

    public void Undo()
    {
        if (_createdShape != null)
        {
            targetLayer.Remove(_createdShape);
        }
    }
}