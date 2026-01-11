using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;
using VectorEditor.UI.Select;

namespace VectorEditor.UI.LayerLogic;

public class LayerController(LayerManager layerManager, CommandManager commands, SelectionManager selectionManager)
{
    private StackPanel? _layerListPanel;
    private StackPanel? _layerGoBackButton;
    public Layer RootLayer => layerManager.RootLayer;
    
    
    private Border? _dropIndicator = new Border
    {
        Height = 2,
        Background = Brushes.DodgerBlue,
        IsVisible = false,
        ZIndex = 100, // Nad innymi elementami
        Margin = new Thickness(0, -1)
    };

    public Layer ActiveLayer => layerManager.CurrentContext;

    public CanvasWidget? SelectedLayerWidget { get; private set; }

    // -----------------------------------------
    // INITIALIZE UI REFERENCES
    // -----------------------------------------
    public void BindUi(StackPanel? layerList, StackPanel? goBackButton, Border? dropIndicator)
    {
        _layerListPanel = layerList;
        _layerGoBackButton = goBackButton;
        _dropIndicator = dropIndicator; // Przypisujemy referencję

        if (_layerListPanel != null)
        {
            DragDrop.SetAllowDrop(_layerListPanel, true);
            _layerListPanel.AddHandler(DragDrop.DropEvent, OnDrop);
            
            _layerListPanel.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            _layerListPanel.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        }

        RefreshUi();
    }
    
    private void OnDrop(object? sender, DragEventArgs e)
    {
        OnDragLeave(null, e);
        
        var droppedModel = e.Data.Get("CanvasModel") as ICanvas;
        if (droppedModel == null || _layerListPanel == null) return;

        // 1. Obliczamy targetIndex na podstawie pozycji Y myszy
        var point = e.GetPosition(_layerListPanel);
        int targetIndex = CalculateTargetIndex(point.Y);

        // 2. Pobieramy aktualny indeks elementu w modelu
        var currentIndex = layerManager.CurrentContext.GetIndexOf(droppedModel);

        // 3. Logika korygująca indeks (UX)
        // Jeśli przesuwamy w dół, docelowy indeks w kolekcji musi uwzględnić 
        // fakt, że element zostanie najpierw usunięty
        if (currentIndex != -1 && targetIndex > currentIndex)
        {
            targetIndex--; 
        }

        // Jeśli upuszczamy dokładnie tam, gdzie element już jest - nic nie rób
        if (currentIndex == targetIndex) return;

        // 4. Wykonanie Twojej strategii
        var strategy = new LayerOrganisationStrategy(
            new[] { droppedModel }, 
            layerManager.CurrentContext, 
            targetIndex
        );

        commands.Execute(new ApplyStrategyCommand(strategy, layerManager.CurrentContext));
    }
    
    private int CalculateTargetIndex(double yCursor)
    {
        if (_layerListPanel == null) return 0;

        int index = 0;
        double currentAccumulatedY = 0;

        foreach (var child in _layerListPanel.Children)
        {
            // Sprawdzamy środek każdego widgetu, aby "celowanie" było bardziej naturalne
            double midPoint = currentAccumulatedY + (child.Bounds.Height / 2);
        
            if (yCursor < midPoint)
                return index;

            currentAccumulatedY += child.Bounds.Height;
            index++;
        }

        return index; // Jeśli kursor jest poniżej wszystkich elementów, wstaw na koniec
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
    
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (_layerListPanel == null || _dropIndicator == null) return;

        var point = e.GetPosition(_layerListPanel);
        int targetIndex = CalculateTargetIndex(point.Y);

        // Obliczamy Y na podstawie sumy wysokości widgetów nad indeksem docelowym
        double targetY = 0;
        for (int i = 0; i < targetIndex && i < _layerListPanel.Children.Count; i++)
        {
            targetY += _layerListPanel.Children[i].Bounds.Height + _layerListPanel.Spacing;
        }

        // Ustawiamy szerokość i pozycję bez modyfikowania struktury StackPanel
        _dropIndicator.IsVisible = true;
        _dropIndicator.Width = _layerListPanel.Bounds.Width;
        Canvas.SetTop(_dropIndicator, targetY);

        e.DragEffects = DragDropEffects.Move;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        if (_dropIndicator != null) _dropIndicator.IsVisible = false;
    }
    
    
}
