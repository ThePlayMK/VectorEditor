using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Structures;
using VectorEditor.UI.BuilderTools;

namespace VectorEditor.UI.Tools.BuilderTools;

public class CircleTool : ITool
{
    private Point? _start;
    private Avalonia.Controls.Shapes.Ellipse? _previewCircle;
    private const double PreviewOpacity = 0.2;

    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
    {
        var p = e.GetPosition(window.CanvasCanvas);
        
        if (_start != null)
        {
            FinishCircle(window, p);
            return;
        }
        
        _start = new Point(p.X, p.Y);
    }

    public void PointerMoved(MainWindow window, PointerEventArgs e)
    {
        if (_start == null)
            return;

        var current = e.GetPosition(window.CanvasCanvas);

        if (_previewCircle == null)
        {
            _previewCircle = new Avalonia.Controls.Shapes.Ellipse()
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth
            };

            window.CanvasCanvas.Children.Add(_previewCircle);
        }
        
        var dx = current.X - _start.X;
        var dy = current.Y - _start.Y;
        var radius = Math.Sqrt(dx * dx + dy * dy);
        
        
        Canvas.SetLeft(_previewCircle, _start.X - radius);
        Canvas.SetTop(_previewCircle, _start.Y - radius);
        _previewCircle.Width = radius * 2;
        _previewCircle.Height = radius * 2;
    }

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
    {
        if (_start is null)
            return;

        var end = e.GetPosition(window.CanvasCanvas);

        if (_previewCircle != null)
        {
            FinishCircle(window, end);
        }
    }

    private void FinishCircle(MainWindow window, Avalonia.Point end)
    {
        if (_previewCircle != null)
        {
            window.CanvasCanvas.Children.Remove(_previewCircle);
            _previewCircle = null;
        }

        var dx = end.X - _start!.X;
        var dy = end.Y - _start.Y;
        var radius = Math.Sqrt(dx * dx + dy * dy);


        var builder = new CircleBuilder()
            .SetStart(_start)
            .SetRadius(radius)
            .SetContourColor(window.Settings.ContourColor)
            .SetContentColor(window.Settings.ContentColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100);


        var cmd = new AddShapeCommand(builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        _start = null;
        _previewCircle = null;
    }

}