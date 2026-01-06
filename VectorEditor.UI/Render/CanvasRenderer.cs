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
}