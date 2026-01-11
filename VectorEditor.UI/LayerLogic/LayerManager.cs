using VectorEditor.Core.Composite;

namespace VectorEditor.UI.LayerLogic;

public class LayerManager
{
    public Layer RootLayer { get; } = new("RootLayer");

    public Layer CurrentContext { get; private set; }

    public LayerManager()
    {
        CurrentContext = RootLayer;
    }

    public void SelectLayer(Layer layer)
    {
    }

    public void EnterLayer(Layer layer)
    {
        CurrentContext = layer;
    }

    public void ExitToParent()
    {
        if (CurrentContext.ParentLayer != null)
            CurrentContext = CurrentContext.ParentLayer;
    }

    public void Reset()
    {
        CurrentContext = RootLayer;
    }
}