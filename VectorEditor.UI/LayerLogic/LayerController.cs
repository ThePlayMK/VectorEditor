using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;
using VectorEditor.UI.Select;

namespace VectorEditor.UI.LayerLogic;

public class LayerController(LayerManager layerManager, CommandManager commands, SelectionManager selectionManager)
{
    private StackPanel? _layerListPanel;
    private StackPanel? _breadcrumbPanel;

    public Layer ActiveLayer => layerManager.CurrentContext;

    public CanvasWidget? SelectedLayerWidget { get; set; }

    // -----------------------------------------
    // INITIALIZE UI REFERENCES
    // -----------------------------------------
    public void BindUi(StackPanel? layerList, StackPanel? breadcrumb)
    {
        _layerListPanel = layerList;
        _breadcrumbPanel = breadcrumb;

        RefreshUi();
    }

    // -----------------------------------------
    // REFRESH UI (list + breadcrumb)
    // -----------------------------------------
    public void RefreshUi()
    {
        if (_layerListPanel == null || _breadcrumbPanel == null)
            return;

        BuildBreadcrumb();
        BuildLayerList();
    }

    // -----------------------------------------
    // BUILD BREADCRUMB
    // -----------------------------------------
    private void BuildBreadcrumb()
    {
        _breadcrumbPanel!.Children.Clear();

        var chain = new List<Layer>();
        var node = layerManager.CurrentContext;

        while (node != null)
        {
            chain.Insert(0, node);
            node = node.ParentLayer;
        }

        foreach (var layer in chain)
        {
            var btn = new Button
            {
                Content = layer.GetName(),
                Tag = layer,
                Padding = new Thickness(4, 2)
            };

            btn.Click += (_, _) =>
            {
                layerManager.EnterLayer(layer);
                layerManager.SelectLayer(layer);
                SelectedLayerWidget = null;
                RefreshUi();
            };

            _breadcrumbPanel.Children.Add(btn);

            if (layer != chain[^1])
                _breadcrumbPanel.Children.Add(new TextBlock { Text = ">" });
        }
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
        var widget = new CanvasWidget
        {
            LayerModel = layer
        };

        widget.SetLayerName(layer.GetName());

        widget.PointerPressed += (_, _) => OnLayerClicked(widget);
        widget.DoubleTapped += (_, _) => EnterLayer(layer);

        return widget;
    }
    
    private Control CreateShapeWidget(ICanvas shape)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Margin = new Thickness(4)
        };

        // Ikonka zależna od typu
        var icon = new TextBlock
        {
            Text = shape switch
            {
                Line => "📏",
                Rectangle => "▭",
                Triangle => "▲",
                Circle => "●",
                CustomShape => "⬠",
                _ => "?"
            }
        };

        var name = new TextBlock
        {
            Text = shape.Name // albo shape.GetName() jeśli masz
        };

        panel.Children.Add(icon);
        panel.Children.Add(name);

        // Kliknięcie → zaznaczenie shape’a
        panel.PointerPressed += (_, _) =>
        {
            selectionManager.SelectSingle(shape);
        };

        return panel;
    }


    
    private void OnLayerClicked(CanvasWidget widget)
    {
        // żadnej logiki LayerManager, żadnego kontekstu
        selectionManager.SelectSingle(widget.LayerModel);
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
