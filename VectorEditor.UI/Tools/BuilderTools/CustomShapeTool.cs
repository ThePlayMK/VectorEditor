using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.BuilderTools;
using Point = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class CustomShapeTool : ITool
{
    private CustomShapeBuilder _builder = new();
    private readonly List<Point> _previewPoints = [];
    private const double PreviewOpacity = 0.2;

    private Polyline? _preview;
    private bool _isDrawing;

    

    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(window.CanvasCanvas).Properties.IsRightButtonPressed)
        {
            if (_isDrawing)
                Finish(window);   // kończymy kształt
            return;               // nie dodajemy punktu
        }
        
        var pos = e.GetPosition(window.CanvasCanvas);
        var p = new Point(pos.X, pos.Y);
        
        if (!_isDrawing)
        {
            _isDrawing = true;

            _previewPoints.Clear();
            _previewPoints.Add(p);

            _preview = new Polyline
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                Fill = new SolidColorBrush(window.Settings.ContentColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth
            };

            window.CanvasCanvas.Children.Add(_preview);
        }
        else
        {
            _previewPoints.Add(p);
        }
        
        _builder.AddPoint(p);

        if (e.GetCurrentPoint(window.CanvasCanvas).Properties.IsMiddleButtonPressed)
        {
            if (_isDrawing)
                Finish(window); 
        }
        UpdatePreview();
    }

    public void PointerMoved(MainWindow window, PointerEventArgs e)
    {
        if (!_isDrawing || _preview == null)
            return;

        var pos = e.GetPosition(window.CanvasCanvas);
        var hover = new Point(pos.X, pos.Y);

        UpdatePreview(hover);
    }

    private void UpdatePreview(Point? hover = null)
    {
        if (_preview == null)
            return;

        var pts = new Avalonia.Collections.AvaloniaList<Avalonia.Point>();

        foreach (var p in _previewPoints)
            pts.Add(new Avalonia.Point(p.X, p.Y));

        if (hover != null)
            pts.Add(new Avalonia.Point(hover.X, hover.Y));

        _preview.Points = pts;
    }
    

    private void Finish(MainWindow window)
    {
        if (_preview != null)
        {
            window.CanvasCanvas.Children.Remove(_preview);
            _preview = null;
        }
        
        if (_builder.GetPoints().Count() < 2)
        {
            _previewPoints.Clear();
            _isDrawing = false;
            _builder = new CustomShapeBuilder();
            return;
        }

        
        _builder
            .SetContentColor(window.Settings.ContentColor)
            .SetContourColor(window.Settings.ContourColor)
            .SetOpacity(window.Settings.Opacity / 100)
            .SetWidth(window.Settings.StrokeWidth);
        
        var cmd = new AddShapeCommand(_builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        _previewPoints.Clear();
        _isDrawing = false;
        _builder = new CustomShapeBuilder();
    }

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
    {
        
    }
}