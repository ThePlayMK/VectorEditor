using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Command;
using VectorEditor.Core.Command.Select;
using VectorEditor.Core.Strategy;
using VectorEditor.Core.Structures;
using VectorEditor.UI.BuilderTools;
using VectorEditor.UI.UIControllers;

namespace VectorEditor.UI.Tools.CommandTools;

public class MoveTool(SelectionManager selection) : ITool
{
    private Point? _lastMouse;
    private double _accDx;
    private double _accDy;
    
    private readonly List<Control> _previewShapes = [];

    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        if (selection.Selected.Count == 0)
            return;

        var pos = e.GetPosition(window.CanvasCanvas);
        _lastMouse = new Point(pos.X, pos.Y);
        
        _accDx = 0;
        _accDy = 0;
        
        CreatePreview(window);
    }

    public void PointerMoved(MainWindow window, ToolController controller,PointerEventArgs e)
    {
        if (_lastMouse == null || selection.Selected.Count == 0)
            return;

        var pos = e.GetPosition(window.CanvasCanvas);
        var current = new Point(pos.X, pos.Y);

        var dx = current.X - _lastMouse.X;
        var dy = current.Y - _lastMouse.Y;

        if (dx == 0 && dy == 0)
            return;

        // Akumulujemy przesunięcie
        _accDx += dx;
        _accDy += dy;

        MovePreview(dx, dy);
        _lastMouse = current;
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        if (selection.Selected.Count == 0)
            return;

        RemovePreview(window);

        // Jeśli nie było ruchu — nie tworzymy komendy
        if (_accDx == 0 && _accDy == 0)
            return;

        // Tworzymy strategię i komendę
        var strategy = new MoveCanvasStrategy(_accDx, _accDy);
        var command = new ApplyStrategyCommand(strategy, selection.Selected);

        window.CommandManager.Execute(command);

        _lastMouse = null;
    }

    private void CreatePreview(MainWindow window)
    {
        foreach (var shape in selection.Selected)
        {
            var preview = shape switch
            {
                Line line => CreateLinePreview(line),
                Rectangle rect => CreateRectPreview(rect),
                Triangle tri => CreateTrianglePreview(tri),
                _ => null!
            };

            _previewShapes.Add(preview);
            window.CanvasCanvas.Children.Add(preview);
        }
    }

    private void MovePreview(double dx, double dy)
    {
        foreach (var ui in _previewShapes)
        {
            switch (ui)
            {
                case Avalonia.Controls.Shapes.Line line:
                    line.StartPoint = new Avalonia.Point(line.StartPoint.X + dx, line.StartPoint.Y + dy);
                    line.EndPoint = new Avalonia.Point(line.EndPoint.X + dx, line.EndPoint.Y + dy);
                    break;

                case Avalonia.Controls.Shapes.Polygon poly:
                    var newPoints = new Avalonia.Collections.AvaloniaList<Avalonia.Point>();
                    foreach (var p in poly.Points)
                        newPoints.Add(new Avalonia.Point(p.X + dx, p.Y + dy));
                    poly.Points = newPoints;
                    break;

                default:
                    Canvas.SetLeft(ui, Canvas.GetLeft(ui) + dx);
                    Canvas.SetTop(ui, Canvas.GetTop(ui) + dy);
                    break;
            }
        }

    }

    private void RemovePreview(MainWindow window)
    {
        foreach (var ui in _previewShapes)
            window.CanvasCanvas.Children.Remove(ui);

        _previewShapes.Clear();
    }

    private Control CreateLinePreview(Line line)
    {
        var s = line.GetStartPoint();
        var e = line.GetEndPoint();

        return new Avalonia.Controls.Shapes.Line
        {
            StartPoint = new Avalonia.Point(s.X, s.Y),
            EndPoint = new Avalonia.Point(e.X, e.Y),
            Stroke = Brushes.Gray,
            StrokeThickness = line.GetWidth(),
            StrokeDashArray = [4, 4]
        };
    }


    private Control CreateRectPreview(Rectangle rect)
    {
        var s = rect.GetStartPoint();
        var e = rect.GetOppositePoint();

        var x = Math.Min(s.X, e.X);
        var y = Math.Min(s.Y, e.Y);
        var w = Math.Abs(e.X - s.X);
        var h = Math.Abs(e.Y - s.Y);

        var ui = new Avalonia.Controls.Shapes.Rectangle
        {
            Width = w,
            Height = h,
            Stroke = Brushes.Gray,
            StrokeThickness = rect.GetWidth(),
            StrokeDashArray = [4, 4]
        };

        Canvas.SetLeft(ui, x);
        Canvas.SetTop(ui, y);

        return ui;
    }


    private Control CreateTrianglePreview(Triangle tri)
    {
        return new Avalonia.Controls.Shapes.Polygon
        {
            Points =
            {
                new Avalonia.Point(tri.GetFirstPoint().X, tri.GetFirstPoint().Y),
                new Avalonia.Point(tri.GetSecondPoint().X, tri.GetSecondPoint().Y),
                new Avalonia.Point(tri.GetThirdPoint().X, tri.GetThirdPoint().Y)
            },
            Stroke = Brushes.Gray,
            StrokeThickness = tri.GetWidth(),
            StrokeDashArray = [4, 4]
        };
    }

}