using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;
using VectorEditor.Core.Structures;
using VectorEditor.UI.Select;
using VectorEditor.UI.Tools.BuilderTools;
using VectorEditor.UI.UIControllers;

namespace VectorEditor.UI.Tools.StrategyTools;

public class ScaleTool(SelectionManager selection) : ITool
{
    private Point? _lastMouse;
    private List<ICanvas>? _previewModel;
    private Dictionary<ICanvas, List<Point>>? _previewState;
    private const double HitTolerance = 30;
    private ScaleHandle? _activeHandle;
    
    private const bool ClearsSelection = false;

    public bool ClearsSelectionBeforeUse() => ClearsSelection;
    
    
    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        if (selection.Selected.Count == 0)
            return;

        _lastMouse = controller.GetSnappedPoint(e, window.CanvasCanvas);

        _activeHandle = HitTest(_lastMouse);

        if (_activeHandle == null)
            return; // klik poza konturem — ignorujemy

        CreatePreview();
        controller.PreviewModel = _previewModel;
        Console.WriteLine($"PreviewModel count: {_previewModel?.Count}");
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (_lastMouse == null || _activeHandle == null)
            return;

        var current = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (current == _lastMouse)
            return;

        ScalePreview(_activeHandle, current);
        
        window.Renderer.Render(window.Layers.RootLayer, selection.Selected, controller);
        
        _lastMouse = current;
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        if (_activeHandle == null || _lastMouse == null)
            return;
        

        var endPos = controller.GetSnappedPoint(e, window.CanvasCanvas);

        if (endPos == _lastMouse)
            return;

        var strategy = new ScaleStrategy(_activeHandle.Value, endPos);
        var command = new ApplyStrategyCommand(strategy, selection.Selected);

        window.CommandManager.Execute(command);

        controller.PreviewModel = null;
        _activeHandle = null;
        _lastMouse = null;
        _previewModel = null;
        _previewState = null;
    }

    private void CreatePreview()
    {
        _previewModel = selection.Selected
            .Select(e => e.Clone())
            .ToList();

        _previewState = new Dictionary<ICanvas, List<Point>>();

        foreach (var element in _previewModel)
        {
            _previewState[element] = element.GetPoints().ToList();
        }
    }


    private void ScalePreview(ScaleHandle? handle, Point mouse)
    {
        if (_previewModel == null || _previewState == null || handle == null)
            return;

        // 🔁 reset preview
        foreach (var (canvas, points) in _previewState)
        {
            canvas.SetPoints(points);
        }

        var strategy = new ScaleStrategy(handle.Value, mouse);

        foreach (var element in _previewModel)
        {
            strategy.Apply(element);
        }
    }

    

    private (double left, double right, double top, double bottom)
        GetSelectionBounds()
    {
        var left = double.PositiveInfinity;
        var right = double.NegativeInfinity;
        var top = double.PositiveInfinity;
        var bottom = double.NegativeInfinity;

        foreach (var shape in selection.Selected)
        {
            left = Math.Min(left, shape.GetMinX());
            right = Math.Max(right, shape.GetMaxX());
            top = Math.Min(top, shape.GetMinY());
            bottom = Math.Max(bottom, shape.GetMaxY());
        }

        return (left, right, top, bottom);
    }
    
    private ScaleHandle? HitTest(Point mouse)
    {
        if (selection.Selected.Count == 0)
            return null;

        var (left, right, top, bottom) = GetSelectionBounds();

        // 🔹 NAROŻNIKI (PRIORYTET)
        if (Near(mouse, left, top))
            return ScaleHandle.TopLeft;

        if (Near(mouse, right, top))
            return ScaleHandle.TopRight;

        if (Near(mouse, right, bottom))
            return ScaleHandle.BottomRight;

        if (Near(mouse, left, bottom))
            return ScaleHandle.BottomLeft;

        // 🔹 KRAWĘDZIE
        if (Math.Abs(mouse.X - left) <= HitTolerance &&
            mouse.Y >= top && mouse.Y <= bottom)
            return ScaleHandle.Left;

        if (Math.Abs(mouse.X - right) <= HitTolerance &&
            mouse.Y >= top && mouse.Y <= bottom)
            return ScaleHandle.Right;

        if (Math.Abs(mouse.Y - top) <= HitTolerance &&
            mouse.X >= left && mouse.X <= right)
            return ScaleHandle.Top;

        if (Math.Abs(mouse.Y - bottom) <= HitTolerance &&
            mouse.X >= left && mouse.X <= right)
            return ScaleHandle.Bottom;

        return null;
    }
    
    private static bool Near(Point mouse, double x, double y)
    {
        return Math.Abs(mouse.X - x) <= HitTolerance &&
               Math.Abs(mouse.Y - y) <= HitTolerance;
    }
}

