using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.UI.Tools.StrategyTools;
using VectorEditor.UI.UIControllers;

namespace VectorEditor.UI.Render;

public class CanvasRenderer(Canvas canvas)
{
    public void Render(Layer rootLayer, IReadOnlyList<ICanvas> selected, ToolController tool, IReadOnlyList<ICanvas>? toIgnore = null)
    {
        canvas.Children.Clear();

        RenderFiltered(rootLayer, toIgnore);
        
        if (tool.PreviewModel != null)
        {
            foreach (var element in tool.PreviewModel)
            {
                element.Render(canvas);
            }
        }

        if (tool.ActiveTool is ScaleTool)
        {
            if(selected.Count != 0)
                RenderHighlight(selected);
        }
        else
        {
            foreach (var shape in selected)
            {
                RenderHighlight(shape);
            }
        }
    }
    
    private void RenderHighlight(ICanvas shape)
    {
        var left = shape.GetMinX();
        var right = shape.GetMaxX();
        var top = shape.GetMinY();
        var bottom = shape.GetMaxY();

        var x = left - 3;
        var y = top - 3;
        var w = (right - left) + 6;
        var h = (bottom - top) + 6;

        var ui = new Avalonia.Controls.Shapes.Rectangle
        {
            Width = w,
            Height = h,
            Stroke = new SolidColorBrush(Colors.DeepSkyBlue),
            StrokeThickness = 2,
            Fill = Brushes.Transparent
        };

        Canvas.SetLeft(ui, x);
        Canvas.SetTop(ui, y);

        canvas.Children.Add(ui);
    }
    
    private void RenderHighlight(IReadOnlyList<ICanvas> canvasList)
    {
        var left = double.PositiveInfinity;
        double right = 0;
        var top = double.PositiveInfinity;
        double bottom = 0;
        foreach (var shape in canvasList)
        {
            left = Math.Min(left, shape.GetMinX());
            right = Math.Max(right, shape.GetMaxX());
            top = Math.Min(top, shape.GetMinY());
            bottom = Math.Max(bottom, shape.GetMaxY());
        }
        
        

        var x = left - 3;
        var y = top - 3;
        var w = (right - left) + 6;
        var h = (bottom - top) + 6;

        var ui = new Avalonia.Controls.Shapes.Rectangle
        {
            Width = w,
            Height = h,
            Stroke = new SolidColorBrush(Colors.DeepSkyBlue),
            StrokeThickness = 2,
            Fill = Brushes.Transparent
        };

        Canvas.SetLeft(ui, x);
        Canvas.SetTop(ui, y);

        canvas.Children.Add(ui);
    }
    
    private void RenderFiltered(ICanvas element, IReadOnlyList<ICanvas>? toIgnore)
    {
        // Jeśli to jest element, którego mamy nie rysować: wychodzimy
        if (toIgnore != null && toIgnore.Contains(element))
            return;

        if (element is Layer layer)
        {
            // Jeśli to warstwa, rekurencyjnie rysujemy jej dzieci
            foreach (var child in layer.GetChildren())
            {
                RenderFiltered(child, toIgnore);
            }
        }
        else
        {
            // Jeśli to zwykły kształt i nie jest na czarnej liście: rysujemy
            element.Render(canvas);
        }
    }
    
}