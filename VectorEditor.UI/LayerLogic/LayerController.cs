using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;
using VectorEditor.UI.Select;

namespace VectorEditor.UI.LayerLogic;

public class LayerController(LayerManager layerManager, CommandManager commands, SelectionManager selectionManager)
{
    private StackPanel? _layerListPanel;
    private StackPanel? _layerGoBackButton;

    public Layer ActiveLayer => layerManager.CurrentContext;

    public CanvasWidget? SelectedLayerWidget { get; set; }

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
        
        var eyeBtn = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(2),
            Width = 24, 
            Height = 24,
            Content = new Material.Icons.Avalonia.MaterialIcon
            {
                Kind = shape.IsVisible 
                       ? Material.Icons.MaterialIconKind.Eye 
                       : Material.Icons.MaterialIconKind.EyeOff,
                Width = 14, Height = 14
            }
        };

        eyeBtn.Click += (_, _) =>
        {
            shape.IsVisible = !shape.IsVisible;

            eyeBtn.Content = new Material.Icons.Avalonia.MaterialIcon
            {
                Kind = shape.IsVisible 
                       ? Material.Icons.MaterialIconKind.Eye 
                       : Material.Icons.MaterialIconKind.EyeOff,
                Width = 14, Height = 14
            };
        };
        
        var lockBtn = new Button
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(2),
            Width = 24, 
            Height = 24,
            Content = new Material.Icons.Avalonia.MaterialIcon
            {
                Kind = shape.IsBlocked 
                       ? Material.Icons.MaterialIconKind.Lock 
                       : Material.Icons.MaterialIconKind.LockOpenVariant,
                Width = 14, Height = 14
            }
        };

        lockBtn.Click += (_, _) =>
        {
            shape.IsBlocked = !shape.IsBlocked;
            
            lockBtn.Content = new Material.Icons.Avalonia.MaterialIcon
            {
                Kind = shape.IsBlocked 
                       ? Material.Icons.MaterialIconKind.Lock 
                       : Material.Icons.MaterialIconKind.LockOpenVariant,
                Width = 14, Height = 14
            };
            
            if (shape.IsBlocked)
            {
                // Tutaj blokowanie
            }
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
        
        panel.Children.Add(eyeBtn);
        panel.Children.Add(lockBtn);
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
