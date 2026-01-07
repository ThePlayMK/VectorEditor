using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI.Render;

public class CanvasRenderer(Canvas canvas)
{
    public void Render(Layer rootLayer, IReadOnlyList<ICanvas> selected)
    {
        canvas.Children.Clear();

        rootLayer.Render(canvas);
        
        foreach (var shape in selected)
            RenderHighlight(shape);
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
            Fill = null
        };

        Canvas.SetLeft(ui, x);
        Canvas.SetTop(ui, y);

        canvas.Children.Add(ui);
    }
}