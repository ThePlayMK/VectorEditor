using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using VectorEditor.Core.Net;
using VectorEditor.UI.BuilderTools;

// ROZWIĄZANIE BŁĘDU:
// Tworzymy alias "CorePoint" dla Twojej struktury, żeby nie myliła się z Avalonia.Point
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.UIControllers;

public class ToolController
{
    private ITool? _activeTool;

    public EditorGrid Grid { get; private set; }

    public ToolController()
    {
        Grid = new EditorGrid(20.0);
        Grid.IsVisible = true; 
        Grid.SnapEnabled = true;
    }

    public void SetTool(ITool? tool)
    {
        _activeTool = tool;
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