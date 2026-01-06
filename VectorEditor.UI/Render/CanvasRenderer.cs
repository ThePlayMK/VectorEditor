using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.UI.Render;

public class CanvasRenderer(Canvas canvas)
{
    public void Render(Layer rootLayer, IEnumerable<Layer> userLayers)
    {
        canvas.Children.Clear();

        // Root
        RenderLayer(rootLayer);

        // User layers (bottom → top)
        foreach (var layer in userLayers)
            RenderLayer(layer);
    }

    private void RenderLayer(Layer layer)
    {
        foreach (var child in layer.GetChildren())
            RenderCanvas(child);
    }

    private void RenderCanvas(ICanvas canva)
    {
        switch (canva)
        {
            case Line line:
                RenderLine(line);
                break;
            
            case Rectangle rect:
                RenderRect(rect);
                break;

            case Layer layer:
                RenderLayer(layer);
                break;
        }
    }

    private void RenderLine(Line line)
    {
        var start = line.GetStartPoint();
        var end = line.GetEndPoint();

        var ui = new Avalonia.Controls.Shapes.Line
        {
            StartPoint = new Avalonia.Point(start.X, start.Y),
            EndPoint = new Avalonia.Point(end.X, end.Y),
            Stroke = new SolidColorBrush(line.GetContourColor(), line.GetOpacity()),
            StrokeThickness = line.GetWidth()
        };

        canvas.Children.Add(ui);
    }

    private void RenderRect(Rectangle rect)
    {
        var start = rect.GetStartPoint();
        var end = rect.GetOppositePoint();

        var x = Math.Min(start.X, end.X);
        var y = Math.Min(start.Y, end.Y);
        var w = Math.Abs(end.X - start.X);
        var h = Math.Abs(end.Y - start.Y);

        var ui = new Avalonia.Controls.Shapes.Rectangle
        {
            Width = w,
            Height = h,
            Stroke = new SolidColorBrush(rect.GetContourColor(), rect.GetOpacity()),
            Fill = new SolidColorBrush(rect.GetContentColor(), rect.GetOpacity()),
            StrokeThickness = rect.GetWidth()
        };

        Canvas.SetLeft(ui, x);
        Canvas.SetTop(ui, y);

        canvas.Children.Add(ui);
    }
}