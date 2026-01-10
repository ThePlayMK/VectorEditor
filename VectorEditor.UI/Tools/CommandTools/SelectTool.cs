using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.UI.Select;
using Point = VectorEditor.Core.Structures.Point;
using VectorEditor.UI.Tools.BuilderTools;
using VectorEditor.UI.UIControllers;

namespace VectorEditor.UI.Tools.CommandTools;

public class SelectTool(SelectionManager selection) : ITool
{
    private Point? _startPoint;
    private Avalonia.Controls.Shapes.Rectangle? _previewRectangle;
    private const double PreviewOpacity = 0.2;
    private const double PreviewWidth = 3;
    private static readonly AvaloniaList<double>? PreviewDashArray = [4, 4];
    private const bool ClearsSelection = false;

    public bool ClearsSelectionBeforeUse() => ClearsSelection;

    public void PointerPressed(MainWindow window,  ToolController controller,PointerPressedEventArgs e)
    {
        _startPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (_startPoint == null)
            return;

        var current = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_previewRectangle == null)
        {
            _previewRectangle = new Avalonia.Controls.Shapes.Rectangle()
            {
                Fill = new SolidColorBrush(Colors.DeepSkyBlue, PreviewOpacity),
                StrokeThickness = PreviewWidth,
                StrokeDashArray = PreviewDashArray
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

        var end = controller.GetSnappedPoint(e, window.CanvasCanvas);

        // Jeśli nie było ruchu — klik punktowy
        if (_previewRectangle == null)
        {
            var p = _startPoint;

            var endPoint = new Point(p.X + 1, p.Y + 1);

            selection.SelectArea(window.LayerController.ActiveLayer, p, endPoint);

            _startPoint = null;
            return;
        }


        Finish(window, end);
    }



    private void Finish(MainWindow window, Point end)
    {
        if (_previewRectangle != null)
        {
            window.CanvasCanvas.Children.Remove(_previewRectangle);
            _previewRectangle = null;
        }

        selection.SelectArea(window.LayerController.ActiveLayer, _startPoint!, end);

        _startPoint = null;
        _previewRectangle =  null;
    }


}