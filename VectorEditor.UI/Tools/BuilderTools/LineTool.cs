using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.UIControllers;
// Alias dla Twojego punktu z Core
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class LineTool : ITool
{
    // Używamy obiektu, ale z pytajnikiem (Nullable).
    // Jeśli jest null -> nie rysujemy. Jeśli jest obiekt -> rysujemy.
    private CorePoint? _startPoint;

    private Avalonia.Controls.Shapes.Line? _previewLine;
    private const double PreviewOpacity = 0.2;
    private const bool ClearsSelection = true;

    public bool ClearsSelectionBeforeUse() => ClearsSelection;

    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        var snappedPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);

        // Jeśli _startPoint ma wartość, to znaczy, że to drugie kliknięcie (koniec linii)
        if (_startPoint != null)
        {
            FinishLine(window, snappedPoint);
            return;
        }

        // TU BYŁ PROBLEM WCZEŚNIEJ:
        // Zamiast przypisywać referencję, tworzymy NOWY, niezależny obiekt.
        // To jest bezpieczne podejście.
        _startPoint = new CorePoint(snappedPoint.X, snappedPoint.Y);
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        // Jeśli jest null, to znaczy, że jeszcze nie kliknięto startu -> wychodzimy
        if (_startPoint == null) return;

        var snappedCurrent = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_previewLine == null)
        {
            _previewLine = new Avalonia.Controls.Shapes.Line
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor,
                    window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth,
                IsHitTestVisible = false
            };
            window.CanvasCanvas.Children.Add(_previewLine);
        }

        // Teraz bezpiecznie używamy _startPoint, bo wiemy, że nie jest nullem
        _previewLine.StartPoint = new Avalonia.Point(_startPoint.X, _startPoint.Y);
        _previewLine.EndPoint = new Avalonia.Point(snappedCurrent.X, snappedCurrent.Y);
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        if (_startPoint is null)
            return;

        var end = controller.GetSnappedPoint(e, window.CanvasCanvas);


        if (_previewLine != null)
        {
            FinishLine(window, end);
        }
    }

    private void FinishLine(MainWindow window, CorePoint endPoint)
    {
        if (_previewLine != null)
        {
            window.CanvasCanvas.Children.Remove(_previewLine);
            _previewLine = null;
        }

        var builder = new LineBuilder()
            .SetStart(_startPoint!)
            .SetContourColor(window.Settings.ContourColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100)
            .SetEnd(endPoint);

        var cmd = new AddShapeCommand(builder, window.LayerController.ActiveLayer);
        window.CommandManager.Execute(cmd);

        // Resetujemy stan na null -> gotowość do nowej linii
        _startPoint = null;
    }
}