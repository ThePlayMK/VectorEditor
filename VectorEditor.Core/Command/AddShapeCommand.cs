using VectorEditor.Core.Builder;
using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class AddShapeCommand(IShapeBuilder builder, IList<IShape> canvas) : ICommand
{
    private IShape? _createdShape = null;

    public void Execute()
    {
        _createdShape = builder.Build();
        canvas.Add(_createdShape);
    }

    public void Undo()
    {
        if (_createdShape != null)
            canvas.Remove(_createdShape);
    }
}