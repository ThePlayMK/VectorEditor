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

    public void EnterLayer(Layer layer)
    {
        CurrentContext = layer;
    }
}