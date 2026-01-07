using System.Collections.Generic;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI.LayerLogic;

public class LayerManager
{
    public Layer RootLayer { get; } = new("RootLayer");
    private readonly List<Layer> _layers = [];

    public IEnumerable<Layer> Layers => _layers;

    public Layer SelectedLayer => _selectedLayer ?? RootLayer;
    private Layer? _selectedLayer;

    public void AddLayer(Layer layer)
    {
        _layers.Add(layer);
    }


    public void SelectLayer(Layer layer)
    {
        _selectedLayer = layer;
    }

    public void RemoveLayer(Layer layer)
    {
        _layers.Remove(layer);
    }

    public void Clear()
    {
        _layers.Clear();
    }
}