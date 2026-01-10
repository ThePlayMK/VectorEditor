using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.UI.Select;

namespace VectorEditor.UI.LayerLogic;

public class LayerController(LayerManager layerManager, CommandManager commands, SelectionManager selectionManager)
{
    private StackPanel? _layerListPanel;
    private StackPanel? _layerGoBackButton;

    public Layer ActiveLayer => layerManager.CurrentContext;

    public CanvasWidget? SelectedLayerWidget { get; private set; }

    // -----------------------------------------
    // INITIALIZE UI REFERENCES
    // -----------------------------------------
    public void BindUi(StackPanel? layerList, StackPanel? goBackButton)
    {
        _layerListPanel = layerList;
        _layerGoBackButton = goBackButton;

        RefreshUi();
    }

    // -----------------------------------------
    // REFRESH UI (list + back button)
    // -----------------------------------------
    public void RefreshUi()
    {
        if (_layerListPanel == null || _layerGoBackButton == null)
            return;

        LayerGoBack();
        BuildLayerList();
    }

    // -----------------------------------------
    // LAYER GO BACK BUTTON
    // -----------------------------------------
    private void LayerGoBack()
    {
        _layerGoBackButton!.Children.Clear();

        var current = layerManager.CurrentContext;
        
        if (current.ParentLayer == null)
        {
            return;
        }
        
        var backBtn = new Button
        {
            Content = $"⬅ Back to {current.ParentLayer.GetName()}",
            HorizontalAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(10, 5),
            Margin = new Thickness(0, 0, 0, 10)
        };

        backBtn.Click += (_, _) =>
        {
            EnterLayer(current.ParentLayer);
        };

        _layerGoBackButton.Children.Add(backBtn);
    }
    
    
    private void BuildLayerList()
    {
        _layerListPanel!.Children.Clear();

        foreach (var child in layerManager.CurrentContext.GetChildren())
        {
            Control widget;

            if (child is Layer layer)
            {
                widget = CreateLayerWidget(layer);
            }
            else
            {
                widget = CreateShapeWidget(child);
            }

            _layerListPanel.Children.Add(widget);
        }
    }

    private Control CreateLayerWidget(Layer layer)
    {
        var widget = new CanvasWidget();
        widget.Bind(layer, commands, selectionManager);

        widget.PointerPressed += (_, _) =>
            selectionManager.SelectSingle(layer);

        widget.DoubleTapped += (_, _) =>
            EnterLayer(layer);

        return widget;
    }
    
    private Control CreateShapeWidget(ICanvas shape)
    {
        var widget = new CanvasWidget();
        widget.Bind(shape, commands, selectionManager);

        widget.PointerPressed += (_, _) =>
            selectionManager.SelectSingle(shape);

        return widget;
    }



    // -----------------------------------------
    // ADD LAYER (as child of current context)
    // -----------------------------------------
    public void AddLayer()
    {
        var parent = layerManager.CurrentContext;
        var cmd = new AddLayerCommand(parent);
        commands.Execute(cmd);
        RefreshUi();
    }


    private void EnterLayer(Layer layer)
    {
        layerManager.EnterLayer(layer);
        SelectedLayerWidget = null;
        layerManager.SelectLayer(layerManager.CurrentContext); // albo null, jeśli dopuszczasz
        selectionManager.Clear();
        RefreshUi();
    }

    

    // -----------------------------------------
    // REMOVE SELECTED LAYER
    // -----------------------------------------
    public void RemoveSelectedLayer()
    {
        if (SelectedLayerWidget == null)
            return;

        var layer = SelectedLayerWidget.LayerModel;

        var cmd = new RemoveLayerCommand(layer);
        commands.Execute(cmd);

        SelectedLayerWidget = null;
        RefreshUi();
    }


    // -----------------------------------------
    // RESET
    // -----------------------------------------
    public void ResetUi()
    {
        layerManager.Reset();
        SelectedLayerWidget = null;
        RefreshUi();
    }
}
