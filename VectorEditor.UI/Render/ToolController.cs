using Avalonia.Input;
using VectorEditor.UI.BuilderTools;

namespace VectorEditor.UI.Render;

public class ToolController
{
    private ITool? _activeTool;

    public void SetTool(ITool? tool)
    {
        _activeTool = tool;
    }

    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
        => _activeTool?.PointerPressed(window, e);

    public void PointerMoved(MainWindow window, PointerEventArgs e)
        => _activeTool?.PointerMoved(window, e);

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
        => _activeTool?.PointerReleased(window, e);
}
