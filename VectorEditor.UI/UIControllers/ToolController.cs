using Avalonia.Controls;
using Avalonia.Input;
using VectorEditor.Core.Command.Select;
using VectorEditor.UI.Tools.BuilderTools;
using VectorEditor.UI.Tools.CommandTools;

namespace VectorEditor.UI.UIControllers;

public class ToolController(SelectionManager selectionManager)
{
    private ITool? _activeTool;
    private Button? _activeToolButton;
    
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

    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
        => _activeTool?.PointerPressed(window, e);

    public void PointerMoved(MainWindow window, PointerEventArgs e)
        => _activeTool?.PointerMoved(window, e);

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
        => _activeTool?.PointerReleased(window, e);
}
