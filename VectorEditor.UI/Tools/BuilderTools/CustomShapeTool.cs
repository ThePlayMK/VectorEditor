using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.UIControllers;

// Alias dla Twojej struktury
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class CustomShapeTool : ITool
{
    private CustomShapeBuilder _builder = new();
    private readonly List<CorePoint> _previewPoints = [];
    
    private Polyline? _previewPolyline;
    private const double PreviewOpacity = 0.5;
    
    private bool _isDrawing;
    private const bool ClearsSelection = true;
    
    public bool ClearsSelectionBeforeUse() => ClearsSelection;

    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(window.CanvasCanvas).Properties.IsRightButtonPressed)
        {
            if (_isDrawing)
                Finish(window);   // kończymy kształt
            return;               // nie dodajemy punktu
        }

        // 2. Pobierz punkt z siatki
        var snappedPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);
        var newPoint = new CorePoint(snappedPoint.X, snappedPoint.Y);

        if (!_isDrawing)
        {
            _isDrawing = true;

            _previewPoints.Clear();
            _previewPoints.Add(newPoint);

            _previewPolyline = new Polyline
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                Fill = new SolidColorBrush(window.Settings.ContentColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth
            };

            window.CanvasCanvas.Children.Add(_previewPolyline);
        }
        else
        {
            _previewPoints.Add(newPoint);
        }
        
        _builder.AddPoint(newPoint);

        if (e.GetCurrentPoint(window.CanvasCanvas).Properties.IsMiddleButtonPressed)
        {
            if (_isDrawing)
                Finish(window); 
        }
        UpdatePreview();
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (!_isDrawing || _previewPolyline == null)
            return;

        var pos = e.GetPosition(window.CanvasCanvas);
        var hover = new CorePoint(pos.X, pos.Y);

        UpdatePreview(hover);
    }

    private void UpdatePreview(CorePoint? hover = null)
    {
        if (_previewPolyline == null)
            return;

        var pts = new Avalonia.Collections.AvaloniaList<Avalonia.Point>();

        foreach (var p in _previewPoints)
            pts.Add(new Avalonia.Point(p.X, p.Y));

        if (hover != null)
            pts.Add(new Avalonia.Point(hover.X, hover.Y));

        _previewPolyline.Points = pts;
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e) { }

    private void Finish(MainWindow window)
    {
        if (_previewPolyline != null)
        {
            window.CanvasCanvas.Children.Remove(_previewPolyline);
            _previewPolyline = null;
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

        ResetTool();
    }

    private void ResetTool()
    {
        _previewPoints.Clear();
        _isDrawing = false;
        _builder = new CustomShapeBuilder();
    }
}