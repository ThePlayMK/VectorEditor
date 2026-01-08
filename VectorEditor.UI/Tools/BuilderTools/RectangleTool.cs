using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.UIControllers;
using VectorEditor.UI.Tools.BuilderTools;
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class RectangleTool : ITool
{
    private CorePoint? _startPoint;
    private Rectangle? _previewRect;
    private const double PreviewOpacity = 0.2;
    private const bool ClearsSelection = true;

    public bool ClearsSelectionBeforeUse() => ClearsSelection;
    
    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        var snappedPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_startPoint != null)
        {
            FinishShape(window, snappedPoint);
            return;
        }

        // TWORZYMY NOWĄ INSTANCJĘ (BEZPIECZNIE)
        _startPoint = new CorePoint(snappedPoint.X, snappedPoint.Y);
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (_startPoint == null) return;

        var snappedCurrent = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_previewRect == null)
        {
            _previewRect = new Rectangle
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth,
                IsHitTestVisible = false
            };
            window.CanvasCanvas.Children.Add(_previewRect);
        }

        // 1. Obliczamy pozycję (Lewy Górny róg) - zawsze najmniejsze X i Y
        double posX = Math.Min(_startPoint.X, snappedCurrent.X);
        double posY = Math.Min(_startPoint.Y, snappedCurrent.Y);

        // 2. Obliczamy wymiary (Różnica) - zawsze dodatnia
        double width = Math.Abs(snappedCurrent.X - _startPoint.X);
        double height = Math.Abs(snappedCurrent.Y - _startPoint.Y);

        Canvas.SetLeft(_previewRect, posX);
        Canvas.SetTop(_previewRect, posY);
        _previewRect.Width = width;
        _previewRect.Height = height;
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e) { }

    private void FinishShape(MainWindow window, CorePoint endPoint)
    {
        if (_previewRect != null)
        {
            window.CanvasCanvas.Children.Remove(_previewRect);
            _previewRect = null;
        }

        // Builder prostokąta zazwyczaj przyjmuje Start i End (narożniki)
        var builder = new RectangleBuilder()
            .SetStart(_startPoint!)
            .SetEnd(endPoint)
            .SetContourColor(window.Settings.ContourColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100);

        var cmd = new AddShapeCommand(builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        _startPoint = null;
    }
}