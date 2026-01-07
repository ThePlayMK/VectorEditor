using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Command.Select;
using Point = VectorEditor.Core.Structures.Point;
using VectorEditor.UI.BuilderTools;
using VectorEditor.UI.UIControllers;

namespace VectorEditor.UI.Tools.CommandTools;

public class SelectTool(SelectionManager selection) : ITool
{
    private Point? _startPoint;
    private Avalonia.Controls.Shapes.Rectangle? _previewRectangle;
    private const double PreviewOpacity = 0.2;
    private const double PreviewWidth = 3;
    
    
    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        var p = e.GetPosition(window.CanvasCanvas);
        _startPoint = new Point(p.X, p.Y);
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (_startPoint == null)
            return;

        var current = e.GetPosition(window.CanvasCanvas);

        if (_previewRectangle == null)
        {
            _previewRectangle = new Avalonia.Controls.Shapes.Rectangle()
            {
                Fill = new SolidColorBrush(Colors.DeepSkyBlue, PreviewOpacity),
                StrokeThickness = PreviewWidth
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

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        if (_startPoint is null)
            return;

        var end = e.GetPosition(window.CanvasCanvas);

        // Jeśli nie było ruchu — klik punktowy
        if (_previewRectangle == null)
        {
            var p = _startPoint;

            var endPoint = new Point(p.X + 1, p.Y + 1);

            selection.SelectArea(window.SelectedLayerModel, p, endPoint);

            _startPoint = null;
            return;
        }


        Finish(window, end);
    }



    private void Finish(MainWindow window, Avalonia.Point end)
    {
        if (_previewRectangle != null)
        {
            window.CanvasCanvas.Children.Remove(_previewRectangle);
            _previewRectangle = null;
        }

        var endPoint = new Point(end.X, end.Y);

        selection.SelectArea(window.SelectedLayerModel, _startPoint!, endPoint);

        _startPoint = null;
        _previewRectangle =  null;
    }


}