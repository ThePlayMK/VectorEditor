using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.Core.Structures;
using VectorEditor.UI.BuilderTools;
using VectorEditor.UI.UIControllers;
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class LineTool : ITool
{
    private CorePoint? _start;
    private Avalonia.Controls.Shapes.Line? _previewLine;
    private const double PreviewOpacity = 0.2;

    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        var p = controller.GetSnappedPoint(e, window.CanvasCanvas);
        if (_start != null)
        {
            FinishLine(window, p);
            return;
        }
        
        _start = p;
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (_start == null)
            return;

        var current = controller.GetSnappedPoint(e, window.CanvasCanvas);

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
        new Avalonia.Point(current.X, current.Y);
    }

    public void PointerReleased(MainWindow window, ToolController controller,PointerReleasedEventArgs e)
    {
        if (_start is null)
            return;

        var end = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (_previewLine != null)
        {
            FinishLine(window, end);
        }
    }
    public void FinishLine(MainWindow window, CorePoint endPoint)
    {
        if (_previewLine != null)
        {
            window.CanvasCanvas.Children.Remove(_previewLine);
            _previewLine = null;
        }

        var builder = new LineBuilder()
            .SetStart(_start!)
            .SetContourColor(window.Settings.ContourColor)
            .SetWidth(window.Settings.StrokeWidth)
            .SetOpacity(window.Settings.Opacity / 100)
            .SetEnd(endPoint);

        var cmd = new AddShapeCommand(builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        _start = null;
    }
}