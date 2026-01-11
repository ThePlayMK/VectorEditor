using System.Collections.Generic;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.UIControllers;

// Alias dla Twojego punktu z Core (żeby nie mylić z Avalonia.Point)
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class TriangleTool : ITool
{
    // Przechowujemy dwa pierwsze punkty jako obiekty CorePoint
    private CorePoint? _firstPoint;
    private CorePoint? _secondPoint;

    private Polygon? _previewTriangle;
    private const double PreviewOpacity = 0.2;
    private const bool ClearsSelection = true;

    public bool ClearsSelectionBeforeUse() => ClearsSelection;


    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        // 1. Pobieramy punkt przyciągnięty do siatki
        var snappedPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);

        // 2. Logika maszyny stanów:

        // ETAP 1: Nie mamy jeszcze nic -> Ustawiamy pierwszy punkt
        if (_firstPoint == null)
        {
            // Tworzymy NOWĄ instancję punktu (bezpieczne podejście)
            _firstPoint = new CorePoint(snappedPoint.X, snappedPoint.Y);
            return;
        }

        // ETAP 2: Mamy pierwszy, brakuje drugiego -> Ustawiamy drugi punkt
        if (_secondPoint == null)
        {
            _secondPoint = new CorePoint(snappedPoint.X, snappedPoint.Y);
            return;
        }

        // ETAP 3: Mamy oba punkty -> To kliknięcie jest trzecim punktem -> KONIEC
        Finish(window, snappedPoint);
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        // Jeśli nie zaczęliśmy rysować (brak pierwszego punktu), nic nie robimy
        if (_firstPoint == null)
        {
            return;
        }

        var snappedCurrent = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_previewTriangle == null)
        {
            _previewTriangle = new Polygon
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor,
                    window.Settings.Opacity * PreviewOpacity / 100),
                Fill = new SolidColorBrush(window.Settings.ContentColor,
                    window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth,
                IsHitTestVisible = false
            };
            window.CanvasCanvas.Children.Add(_previewTriangle);
        }

        if (_secondPoint == null)
        {
            _previewTriangle.Points = new List<Avalonia.Point>()
            {
                new(_firstPoint.X, _firstPoint.Y),
                new(snappedCurrent.X, snappedCurrent.Y)
            };
        }

        else
        {
            _previewTriangle.Points = new List<Avalonia.Point>()
            {
                new(_firstPoint.X, _firstPoint.Y),
                new(_secondPoint.X, _secondPoint.Y),
                new(snappedCurrent.X, snappedCurrent.Y)
            };
        }
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        // Puste - w tym narzędziu wygodniej używać PointerPressed
    }

    private void Finish(MainWindow window, CorePoint endPoint)
    {
        if (_previewTriangle != null)
        {
            window.CanvasCanvas.Children.Remove(_previewTriangle);
            _previewTriangle = null;
        }

        // Budujemy trójkąt. Zakładam, że Twój TriangleBuilder ma metody SetStart, SetSecond, SetEnd.
        var builder = new TriangleBuilder()
            .SetStart(_firstPoint!)
            .SetSecond(_secondPoint!)
            .SetEnd(endPoint)
            .SetContourColor(window.Settings.ContourColor)
            .SetContentColor(window.Settings.ContentColor) // Jeśli masz wypełnienie
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100);

        // Wykonujemy komendę
        var cmd = new AddShapeCommand(builder, window.LayerController.ActiveLayer);
        window.CommandManager.Execute(cmd);

        // Resetujemy stan
        _firstPoint = null;
        _secondPoint = null;
    }
}