using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using VectorEditor.UI.Tools.BuilderTools;
using VectorEditor.UI.Select;
using VectorEditor.Core.Net;
using VectorEditor.UI.Tools.CommandTools;
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.UIControllers;

public class ToolController
{
    private ITool? _activeTool;
    private Button? _activeToolButton;
    private SelectionManager selectionManager;
    public EditorGrid Grid { get; private set; }

    public ToolController(SelectionManager selectionManager)
    {
        Grid = new EditorGrid(20.0);
        Grid.IsVisible = true; 
        Grid.SnapEnabled = true;
        this.selectionManager = selectionManager;
    }
    public bool IsHandToolActive =>
        _activeToolButton?.Tag as string == "Hand";
    
    public void SelectTool(Button button)
    {
        _activeToolButton?.Classes.Remove("Selected");
        _activeToolButton = button;
        _activeToolButton.Classes.Add("Selected");
        
        _activeTool = button.Tag switch
        {
            "Line"        => new LineTool(),
            "Rectangle"   => new RectangleTool(),
            "Triangle"    => new TriangleTool(),
            "Ellipse"     => new CircleTool(),
            "Selector"    => new SelectTool(selectionManager),
            "Move"        => new MoveTool(selectionManager),
            "CustomShape" => new CustomShapeTool(),
            "Hand"        => null, // Pan tool handled separately
            _             => null
        };
        
        if (_activeTool != null && _activeTool.ClearsSelectionBeforeUse())
        {
            selectionManager.Clear();
        }
    }

    public void Reset()
    {
        _activeTool = null;
        _activeToolButton?.Classes.Remove("Selected");
        _activeToolButton = null;
    }

    // Pamiętaj, że zaktualizowaliśmy metody, przekazując 'this'
    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
        => _activeTool?.PointerPressed(window, this, e);

    public void PointerMoved(MainWindow window, PointerEventArgs e)
        => _activeTool?.PointerMoved(window, this, e);

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
        => _activeTool?.PointerReleased(window, this, e);

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