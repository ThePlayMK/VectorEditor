using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using VectorEditor.Core.Composite;
using VectorEditor.UI.Tools.BuilderTools;
using VectorEditor.UI.Select;
using VectorEditor.Core.Net;
using VectorEditor.UI.Tools.CommandTools;
using VectorEditor.UI.Tools.StrategyTools;
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.UIControllers;

public class ToolController(SelectionManager selectionManager, MainWindow window)
{
    private Button? _activeToolButton;
    public IReadOnlyList<ICanvas>? PreviewModel { get; set; }
    public event Action? OnChanged;
    public ITool? ActiveTool { get; private set; }

    public EditorGrid Grid { get; private set; } = new(window.Settings.GridSize)
    {
        IsVisible = true,
        SnapEnabled = true
    };

    public bool IsHandToolActive =>
        _activeToolButton?.Tag as string == "Hand";
    
    public void SelectTool(Button button)
    {
        _activeToolButton?.Classes.Remove("Selected");
        _activeToolButton = button;
        _activeToolButton.Classes.Add("Selected");
        
        ActiveTool = button.Tag switch
        {
            "Line"        => new LineTool(),
            "Rectangle"   => new RectangleTool(),
            "Triangle"    => new TriangleTool(),
            "Ellipse"     => new CircleTool(),
            "Selector"    => new SelectTool(selectionManager),
            "Move"        => new MoveTool(selectionManager),
            "CustomShape" => new CustomShapeTool(),
            "Hand"        => null, // Pan tool handled separately
            "Scale"       => new ScaleTool(selectionManager),
            _             => null
        };
        
        if (ActiveTool != null && ActiveTool.ClearsSelectionBeforeUse())
        {
            selectionManager.Clear();
        }
       
        OnChanged?.Invoke();
        
    }

    public void Reset()
    {
        ActiveTool = null;
        _activeToolButton?.Classes.Remove("Selected");
        _activeToolButton = null;
    }

    // Pamiętaj, że zaktualizowaliśmy metody, przekazując 'this'
    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
        => ActiveTool?.PointerPressed(window, this, e);

    public void PointerMoved(MainWindow window, PointerEventArgs e)
        => ActiveTool?.PointerMoved(window, this, e);

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
        => ActiveTool?.PointerReleased(window, this, e);

    /// <summary>
    /// Metoda zwraca Twój CorePoint przyciągnięty do siatki
    /// </summary>
    public CorePoint GetSnappedPoint(PointerEventArgs e, Visual relativeTo)
    {
        // 1. To jest Avalonia.Point (pozycja z ekranu)
        var rawAvaloniaPoint = e.GetPosition(relativeTo);

        // 2. Konwertujemy na Twój CorePoint
        var myPoint = new CorePoint(rawAvaloniaPoint.X, rawAvaloniaPoint.Y);

        // 3. Siatka przetwarza Twój punkt i go zwraca
        return Grid.Snap(myPoint);
    }
}