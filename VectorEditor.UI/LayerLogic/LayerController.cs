using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI.LayerLogic;

public class LayerController
{
    private int _layerCount;

    public LayerWidget? SelectedLayer { get; private set; }
    public LayerManager LayerManager { get; }

    public LayerController(LayerManager layerManager)
    {
        LayerManager = layerManager;
    }

    // -------------------------
    // ADD LAYER
    // -------------------------
    public void AddLayer(StackPanel panel)
    {
        _layerCount++;

        var widget = new LayerWidget();
        var widgetName = $"Layer{_layerCount}";
        widget.SetLayerName(widgetName);

        // Tworzymy model warstwy i dodajemy do LayerManager
        var model = new Layer(widgetName);
        widget.LayerModel = model;
        LayerManager.AddLayer(widgetName);

        // Dodajemy do UI (na górę)
        panel.Children.Insert(0, widget);
    }

    // -------------------------
    // REMOVE SELECTED LAYER
    // -------------------------
    public void RemoveSelectedLayer(StackPanel panel)
    {
        if (SelectedLayer == null)
            return;

        // Usuwamy model
        LayerManager.RemoveLayer(SelectedLayer.LayerModel);

        // Usuwamy widget
        panel.Children.Remove(SelectedLayer);

        SelectedLayer = null;
    }

    // -------------------------
    // SELECT LAYER
    // -------------------------
    public void SelectLayer(Button clickedButton)
    {
        // Odznacz poprzednią
        var oldBtn = SelectedLayer?.FindDescendantOfType<Button>();
        if (oldBtn != null)
            oldBtn.Background = Brushes.Transparent;

        // Znajdź widget
        var widget = clickedButton.FindAncestorOfType<LayerWidget>();
        if (widget == null)
            return;

        SelectedLayer = widget;

        // Zaznacz nową
        clickedButton.Background = Brushes.Gray;
    }

    public void Reset()
    {
        _layerCount = 0;
        SelectedLayer = null;
        LayerManager.Clear();
    }
}