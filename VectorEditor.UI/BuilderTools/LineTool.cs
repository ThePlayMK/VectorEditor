using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Structures;


namespace VectorEditor.UI.BuilderTools;

public class LineTool : ITool
{
    private Point _start = null!;

    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
    {
        var point = e.GetPosition(window.MyCanvas);
        _start = new Point(point.X, point.Y);
    }

    public void PointerMoved(MainWindow window, PointerEventArgs e)
    {
        // opcjonalnie: rysowanie podglądu dynamicznego (preview)
    }

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
    {
        /*if (_start == null) return;

        var end = e.GetPosition(window.MyCanvas);
        var line = new LineBuilder(window.SelectedColor, (int)window.StrokeWidth)
            .SetStart(_start)
            .SetEnd(end)
            .Build();

        var avaloniaLine = new Avalonia.Controls.Shapes.Line
        {
            StartPoint = line.Start,
            EndPoint = line.End,
            Stroke = new SolidColorBrush(line.Color),
            StrokeThickness = line.Width
        };

        window.MyCanvas.Children.Add(avaloniaLine);
        _start = null;*/
    }
}