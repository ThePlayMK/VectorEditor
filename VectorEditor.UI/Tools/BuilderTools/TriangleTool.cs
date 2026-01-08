using System.Collections.Generic;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Structures;

namespace VectorEditor.UI.Tools.BuilderTools;

public class TriangleTool : ITool
{
    private Point? _firstPoint;
    private Point? _secondPoint;
    private Polygon? _previewTriangle;
    private const double PreviewOpacity = 0.2;
    private const bool ClearsSelection = true;

    public bool ClearsSelectionBeforeUse() => ClearsSelection;
    
    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
    {
        // Tryb press-release-release:
        // Press NIC nie robi.
    }

    
    public void PointerMoved(MainWindow window, PointerEventArgs e)
    {
        if (_firstPoint == null)
        {
            return;
        }

        var current = e.GetPosition(window.CanvasCanvas);
        var point = new Point(current.X, current.Y);

        if (_previewTriangle == null)
        {
            _previewTriangle = new Polygon
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                Fill = new SolidColorBrush(window.Settings.ContentColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth
            };

            window.CanvasCanvas.Children.Add(_previewTriangle);
        }

        if (_secondPoint == null)
        {
            _previewTriangle.Points = new List<Avalonia.Point>()
            {
                new(_firstPoint.X, _firstPoint.Y),
                new(point.X, point.Y)
            };
        }
        
        else
        {
            _previewTriangle.Points = new List<Avalonia.Point>()
            {
                new(_firstPoint.X, _firstPoint.Y),
                new(_secondPoint.X, _secondPoint.Y),
                new(point.X, point.Y)
            };
        }
    }

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
    {
        var pos = e.GetPosition(window.CanvasCanvas);
        var point = new Point(pos.X, pos.Y);

        // 1. pierwszy release → ustaw pierwszy punkt
        if (_firstPoint == null)
        {
            _firstPoint = point;
            return;
        }

        // 2. drugi release → ustaw drugi punkt
        if (_secondPoint == null)
        {
            _secondPoint = point;
            return;
        }

        // 3. trzeci release → kończymy trójkąt
        Finish(window, point);
    }

    private void Finish(MainWindow window, Point end)
    {
        if (_previewTriangle != null)
        {
            window.CanvasCanvas.Children.Remove(_previewTriangle);
            _previewTriangle = null;
        }

        var builder = new TriangleBuilder()
            .SetStart(_firstPoint!)
            .SetSecond(_secondPoint!)
            .SetEnd(end)
            .SetContourColor(window.Settings.ContourColor)
            .SetContentColor(window.Settings.ContentColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100);

        window.CommandManager.Execute(new AddShapeCommand(builder, window.SelectedLayerModel));

        _firstPoint = null;
        _secondPoint = null;
    }

}