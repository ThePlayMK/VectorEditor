using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI.LayerLogic;

public class LayerController(LayerManager layerManager, CommandManager commands)
{
    private StackPanel? _layerListPanel;
    private StackPanel? _breadcrumbPanel;

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

        for (int i = 0; i < chain.Count; i++)
        {
            var layer = chain[i];

            var btn = new Button
            {
                Content = layer.GetName(),
                Tag = layer,
                Padding = new Thickness(4, 2)
            };

            btn.Click += (_, _) =>
            {
                layerManager.EnterLayer(layer);

                // zaznaczamy tę warstwę, bo jest przodkiem kontekstu
                SelectedLayerWidget = null;
                layerManager.SelectLayer(layer);

                RefreshUi();
            };



            _breadcrumbPanel.Children.Add(btn);

            if (i < chain.Count - 1)
                _breadcrumbPanel.Children.Add(new TextBlock { Text = ">" });
        }
    }

    // -----------------------------------------
    // BUILD LAYER LIST (children of current context)
    // -----------------------------------------
    private void BuildLayerList()
    {
        _layerListPanel!.Children.Clear();

        foreach (var child in layerManager.CurrentContext.GetChildren())
        {
            if (child is not Layer layer) continue;
            var widget = new CanvasWidget
            {
                LayerModel = layer
            };
            widget.SetLayerName(layer.GetName());

            widget.PointerPressed += (_, _) => SelectLayerWidget(widget);
            widget.DoubleTapped += (_, _) => EnterLayer(layer);

            _layerListPanel.Children.Add(widget);
        }
    }

    // -----------------------------------------
    // SELECT LAYER
    // -----------------------------------------
    private void SelectLayerWidget(CanvasWidget widget)
    {
        if (widget.LayerModel.ParentLayer != layerManager.CurrentContext)
            return;
        // Unhighlight old
        if (SelectedLayerWidget != null)
        {
            var oldBtn = SelectedLayerWidget.FindDescendantOfType<Button>();
            if (oldBtn != null)
                oldBtn.Background = Brushes.Transparent;
        }

        SelectedLayerWidget = widget;

        // Highlight new
        var btn = widget.FindDescendantOfType<Button>();
        if (btn != null)
            btn.Background = Brushes.Gray;

        layerManager.SelectLayer(widget.LayerModel);
    }

    // -----------------------------------------
    // ENTER LAYER (double-click)
    // -----------------------------------------
    private void EnterLayer(Layer layer)
    {
        layerManager.EnterLayer(layer);
        SelectedLayerWidget = null;
        layerManager.SelectLayer(layerManager.CurrentContext); // albo null, jeśli dopuszczasz
        RefreshUi();
    }



    // -----------------------------------------
    // ADD LAYER (as child of current context)
    // -----------------------------------------
    public void AddLayer()
    {
        var cmd = new AddLayerCommand(layerManager.CurrentContext);
        commands.Execute(cmd);
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
