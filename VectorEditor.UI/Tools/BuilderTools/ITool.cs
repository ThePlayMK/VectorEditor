using Avalonia.Input;

namespace VectorEditor.UI.Tools.BuilderTools;

public interface ITool
{
    void PointerPressed(MainWindow window, PointerPressedEventArgs e);
    void PointerMoved(MainWindow window, PointerEventArgs e);
    void PointerReleased(MainWindow window, PointerReleasedEventArgs e);
}