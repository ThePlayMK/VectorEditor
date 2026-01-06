using System.Collections.Generic;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI.Render;

public class LayerManager
{
    public Layer RootLayer { get; } = new("RootLayer");
    private readonly List<Layer> _layers = [];

    public IEnumerable<Layer> Layers => _layers;

    public Layer SelectedLayer => _selectedLayer ?? RootLayer;
    private Layer? _selectedLayer;

    public Layer AddLayer(string name)
    {
        var layer = new Layer(name);
        _layers.Add(layer);
        return layer;
    }

    public void SelectLayer(Layer layer)
    {
        _selectedLayer = layer;
    }
}