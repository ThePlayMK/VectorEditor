using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.UIControllers;
using System.Diagnostics; // Do logów
using VectorEditor.UI.BuilderTools;

// Alias dla Twojego punktu z Core
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class LineTool : ITool
{
    // PANCERNA ZMIANA: Przechowujemy proste liczby, a nie obiekt
    private double _startX = 0;
    private double _startY = 0;
    private bool _isDrawing = false; // Flaga: czy właśnie rysujemy?

    private Avalonia.Controls.Shapes.Line? _previewLine;
    private const double PreviewOpacity = 0.5;

    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        // 1. Pobierz punkt startowy (przyciągnięty)
        var snappedPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);
        
        Debug.WriteLine($"[PRESSED] Kliknięto: {snappedPoint.X}, {snappedPoint.Y}");

        if (_isDrawing)
        {
            // Jeśli już rysujemy i klikniemy drugi raz -> kończymy linię
            FinishLine(window, snappedPoint);
            return;
        }
        
        // Zapisujemy start do prostych zmiennych (to nie ma prawa zniknąć!)
        _startX = snappedPoint.X;
        _startY = snappedPoint.Y;
        _isDrawing = true; // Zaczynamy rysowanie
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        // Jeśli nie rysujemy, ignorujemy ruch
        if (!_isDrawing) return;

        // 2. Pobierz punkt aktualny (przyciągnięty)
        var snappedCurrent = controller.GetSnappedPoint(e, window.CanvasCanvas);

        // DIAGNOSTYKA: Sprawdź w konsoli, czy Start nadal pamięta wartość!
        // Debug.WriteLine($"[MOVED] Start: {_startX},{_startY} -> Koniec: {snappedCurrent.X},{snappedCurrent.Y}");

        if (_previewLine == null)
        {
            _previewLine = new Avalonia.Controls.Shapes.Line
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth,
                IsHitTestVisible = false
            };
            window.CanvasCanvas.Children.Add(_previewLine);
        }

        // Rysujemy linię od zapamiętanego startu do aktualnej pozycji myszki
        _previewLine.StartPoint = new Avalonia.Point(_startX, _startY);
        _previewLine.EndPoint = new Avalonia.Point(snappedCurrent.X, snappedCurrent.Y);
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        // Tutaj nic nie robimy, bo rysujemy metodą "Kliknij - Przesuń - Kliknij"
    }

    public void FinishLine(MainWindow window, CorePoint endPoint)
    {
        if (_previewLine != null)
        {
            window.CanvasCanvas.Children.Remove(_previewLine);
            _previewLine = null;
        }

        // Tworzymy finalny kształt
        var startPointObj = new CorePoint(_startX, _startY);

        var builder = new LineBuilder()
            .SetStart(startPointObj)
            .SetContourColor(window.Settings.ContourColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100)
            .SetEnd(endPoint);

        var cmd = new AddShapeCommand(builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        // Resetujemy stan
        _isDrawing = false;
        _startX = 0;
        _startY = 0;
    }
}