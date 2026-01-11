using VectorEditor.Core.Builder;
using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public class AddShapeCommand(IShapeBuilder builder, Layer targetLayer) : ICommand
{
    private IShape? _shape;

    public void Execute()
    {
        //testy
        //System.Diagnostics.Debug.WriteLine($"[CMD] Execute start. Warstwa ID: {targetLayer.GetHashCode()}");
        //if (targetLayer.IsBlocked){
        //System.Diagnostics.Debug.WriteLine("[CMD] BLOKADA! Warstwa jest zablokowana (IsBlocked = true).");
          //  return;
        //}
        _shape ??= builder.Build(); // ✅ tylko raz
        targetLayer.Add(_shape);
        //System.Diagnostics.Debug.WriteLine($"[CMD] SUKCES. Dodałem kształt. Nowa liczba dzieci: {targetLayer.GetChildren().ToList().Count}");
    }

    public void Undo()
    {
        if (_shape != null)
            targetLayer.Remove(_shape);
    }
}