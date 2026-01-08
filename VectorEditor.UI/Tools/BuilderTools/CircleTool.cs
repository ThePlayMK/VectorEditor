using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.UIControllers;
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class CircleTool : ITool
{
    private CorePoint? _centerPoint; // To jest środek koła
    private Ellipse? _previewEllipse;
    private const double PreviewOpacity = 0.5;
    private const bool ClearsSelection = true;
    
    public bool ClearsSelectionBeforeUse() => ClearsSelection;
    
    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        var snappedPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_centerPoint != null)
        {
            FinishShape(window, snappedPoint);
            return;
        }

        // Pierwsze kliknięcie to ŚRODEK koła
        _centerPoint = new CorePoint(snappedPoint.X, snappedPoint.Y);
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (_centerPoint == null) return;

        var snappedCurrent = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_previewEllipse == null)
        {
            _previewEllipse = new Ellipse
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth,
                IsHitTestVisible = false
            };
            window.CanvasCanvas.Children.Add(_previewEllipse);
        }

        var dx = snappedCurrent.X - _centerPoint.X;
        var dy = snappedCurrent.Y - _centerPoint.Y;
        var radius = Math.Sqrt(dx * dx + dy * dy);
        
        
        Canvas.SetLeft(_previewEllipse, _centerPoint.X - radius);
        Canvas.SetTop(_previewEllipse, _centerPoint.Y - radius);
        _previewEllipse.Width = radius * 2;
        _previewEllipse.Height = radius * 2;
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        if (_centerPoint is null)
            return;

        var end = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_previewEllipse != null)
        {
            FinishShape(window, end);
        }
    }

    private void FinishShape(MainWindow window, CorePoint endPoint)
    {
        if (_previewEllipse != null)
        {
            window.CanvasCanvas.Children.Remove(_previewEllipse);
            _previewEllipse = null;
        }
        
        
        // Twój Builder idealnie tu pasuje: SetStart (środek) i SetRadius (punkt na obwodzie)
        var builder = new CircleBuilder()
            .SetStart(_centerPoint!)    // Środek
            .SetRadius(endPoint)        // Punkt końcowy (do wyliczenia promienia)
            .SetContourColor(window.Settings.ContourColor)
            .SetContentColor(window.Settings.ContentColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100);
            
        var cmd = new AddShapeCommand(builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);
        
        _centerPoint = null;
        _previewEllipse = null;
    }
}