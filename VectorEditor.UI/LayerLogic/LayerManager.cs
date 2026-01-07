using VectorEditor.Core.Composite;

namespace VectorEditor.UI.LayerLogic;

public class LayerManager
{
    public Layer RootLayer { get; } = new("RootLayer");

    public Layer? SelectedLayer { get; private set; }
    public Layer CurrentContext { get; set; }

    public LayerManager()
    {
        CurrentContext = RootLayer;
    }

    public void SelectLayer(Layer layer)
    {
        SelectedLayer = layer;
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
        SelectedLayer = null;
        CurrentContext = RootLayer;
    }
}