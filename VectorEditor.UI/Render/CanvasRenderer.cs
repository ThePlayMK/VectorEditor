using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;
using Point = Avalonia.Point;

namespace VectorEditor.UI.Render;

public class CanvasRenderer(Canvas canvas)
{
    public void Render(Layer rootLayer, IEnumerable<Layer> userLayers, IReadOnlyList<ICanvas> selected)
    {
        canvas.Children.Clear();

        // Root
        RenderLayer(rootLayer, selected);

        // User layers (bottom → top)
        foreach (var layer in userLayers)
            RenderLayer(layer, selected);
    }

    private void RenderLayer(Layer layer, IReadOnlyList<ICanvas> selected)
    {
        foreach (var child in layer.GetChildren())
            RenderCanvas(child, selected);
    }

    private void RenderCanvas(ICanvas canva, IReadOnlyList<ICanvas> selected)
    {
        switch (canva)
        {
            case Line line:
                RenderLine(line);
                break;
            
            case Rectangle rect:
                RenderRect(rect);
                break;
            
            case Triangle triangle:
                RenderTriangle(triangle);
                break;

            case Circle circle:
                RenderCircle(circle);
                break;
            
            case CustomShape customShape:
                RenderCustom(customShape);
                break;
            
            case Layer layer:
                RenderLayer(layer, selected);
                break;
        }
        
        if (selected.Contains(canva))
            RenderHighlight(canva);

    }

    private void RenderLine(Line line)
    {
        var start = line.GetStartPoint();
        var end = line.GetEndPoint();

        var ui = new Avalonia.Controls.Shapes.Line
        {
            StartPoint = new Point(start.X, start.Y),
            EndPoint = new Point(end.X, end.Y),
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

    private void RenderTriangle(Triangle triangle)
    {
        var ui = new Avalonia.Controls.Shapes.Polygon
        {
            Points =
            {
                new Point(triangle.GetFirstPoint().X, triangle.GetFirstPoint().Y),
                new Point(triangle.GetSecondPoint().X, triangle.GetSecondPoint().Y),
                new Point(triangle.GetThirdPoint().X, triangle.GetThirdPoint().Y)
            },
            Stroke = new SolidColorBrush(triangle.GetContourColor(), triangle.GetOpacity()),
            Fill = new SolidColorBrush(triangle.GetContentColor(), triangle.GetOpacity()),
            StrokeThickness = triangle.GetWidth()
        };

        canvas.Children.Add(ui);

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

    private void RenderCircle(Circle circle)
    {
        var start = circle.GetCenterPoint();
        var radiusX = circle.GetRadiusX();
        var radiusY = circle.GetRadiusY();

        var ui = new Avalonia.Controls.Shapes.Ellipse
        {
            Width = radiusX * 2,
            Height = radiusY * 2,
            Stroke = new SolidColorBrush(circle.GetContourColor(), circle.GetOpacity()),
            Fill = new SolidColorBrush(circle.GetContentColor(), circle.GetOpacity()),
            StrokeThickness = circle.GetWidth()
        };

        Canvas.SetLeft(ui, start.X - radiusX);
        Canvas.SetTop(ui, start.Y - radiusY);
        canvas.Children.Add(ui);
    }

    private void RenderCustom(CustomShape customShape)
    {
        var list = customShape.GetPoints();
        var avaloniaList = list.Select(p => new Point(p.X, p.Y)).ToList();
        avaloniaList.Add(avaloniaList[0]);

        var ui = new Avalonia.Controls.Shapes.Polygon
        {
            Points = new List<Point>(avaloniaList),
            Stroke = new SolidColorBrush(customShape.GetContourColor(), customShape.GetOpacity()),
            Fill = new SolidColorBrush(customShape.GetContentColor(), customShape.GetOpacity()),
            StrokeThickness = customShape.GetWidth()
        };

        canvas.Children.Add(ui);
    }
}