using Avalonia.Input;
using VectorEditor.UI.UIControllers; // Dodajemy to, by widzieć klasę ToolController

namespace VectorEditor.UI.Tools.BuilderTools;

public interface ITool
{
    // Dodajemy 'ToolController controller' do każdej metody.
    // Dzięki temu narzędzie (np. LineTool) może wywołać controller.GetSnappedPoint(...)
    
    void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e);
    void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e);
    void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e);
}