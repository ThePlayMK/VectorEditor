using VectorEditor.Core.Builder;
using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class AddShapeCommand(IShapeBuilder builder, Layer targetLayer) : ICommand
{
    private IShape? _shape;

    public void Execute()
    {
        if (targetLayer.IsBlocked)
            return;

        _shape ??= builder.Build(); // ✅ tylko raz
        targetLayer.Add(_shape);
    }

    public void Undo()
    {
        if (_shape != null)
            targetLayer.Remove(_shape);
    }
}