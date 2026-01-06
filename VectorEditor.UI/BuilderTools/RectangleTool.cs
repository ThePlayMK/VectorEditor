using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Structures;

namespace VectorEditor.UI.BuilderTools;

public class RectangleTool : ITool
{
    private Point? _startPoint;
    private Avalonia.Controls.Shapes.Rectangle? _previewRectangle;
    private const double PreviewOpacity = 0.2;

    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
    {
        var p = e.GetPosition(window.CanvasCanvas); 
        
        if (_startPoint != null)
        {
            FinishLine(window, p);
            return;
        }
        
        _startPoint = new Point(p.X, p.Y);
    }

    public void PointerMoved(MainWindow window, PointerEventArgs e)
    {
        if (_startPoint == null)
            return;

        var current = e.GetPosition(window.CanvasCanvas);

        if (_previewRectangle == null)
        {
            _previewRectangle = new Avalonia.Controls.Shapes.Rectangle
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                Fill = new SolidColorBrush(window.Settings.ContentColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth
            };

            window.CanvasCanvas.Children.Add(_previewRectangle);
        }

        var x = Math.Min(_startPoint.X, current.X);
        var y = Math.Min(_startPoint.Y, current.Y);
        var w = Math.Abs(current.X - _startPoint.X);
        var h = Math.Abs(current.Y - _startPoint.Y);

        Canvas.SetLeft(_previewRectangle, x);
        Canvas.SetTop(_previewRectangle, y);
        _previewRectangle.Width = w;
        _previewRectangle.Height = h;

    }

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
    {
        if (_startPoint is null)
            return;

        var end = e.GetPosition(window.CanvasCanvas);

        if (_previewRectangle != null)
        {
            FinishLine(window, end);
        }
    }
    private void FinishLine(MainWindow window, Avalonia.Point end)
    {
        if (_previewRectangle != null)
        {
            window.CanvasCanvas.Children.Remove(_previewRectangle);
            _previewRectangle = null;
        }

        var builder = new RectangleBuilder()
            .SetStart(_startPoint!)
            .SetEnd(new Point(end.X, end.Y))
            .SetContourColor(window.Settings.ContourColor)
            .SetContentColor(window.Settings.ContentColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100);


        var cmd = new AddShapeCommand(builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        _startPoint = null;
        _previewRectangle = null;
    }
}