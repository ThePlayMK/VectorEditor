using System;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Structures;

namespace VectorEditor.UI.BuilderTools;

public class LineTool : ITool
{
    private Point? _start;
    private Avalonia.Controls.Shapes.Line? _previewLine;
    private const double PreviewOpacity = 0.2;

    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
    {
        var p = e.GetPosition(window.CanvasCanvas); // ✔ poprawione
        _start = new Point(p.X, p.Y);
    }

    public void PointerMoved(MainWindow window, PointerEventArgs e)
    {
        if (_start == null)
            return;

        var current = e.GetPosition(window.CanvasCanvas);

        if (_previewLine == null)
        {
            _previewLine = new Avalonia.Controls.Shapes.Line
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth
            };

            window.CanvasCanvas.Children.Add(_previewLine);
        }

        _previewLine.StartPoint = new Avalonia.Point(_start.X, _start.Y);
        _previewLine.EndPoint = current;
    }

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
    {
        if (_start is null)
            return;

        var end = e.GetPosition(window.CanvasCanvas); 
        
        if (_previewLine != null)
        {
            window.CanvasCanvas.Children.Remove(_previewLine);
            _previewLine = null;
        }

        var builder = new LineBuilder()
            .SetStart(_start)
            .SetContourColor(window.Settings.ContourColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100)
            .SetEnd(new Point(end.X, end.Y));

        var cmd = new AddShapeCommand(builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        _start = null;
    }
}