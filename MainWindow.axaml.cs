using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using Avalonia.Controls.Primitives;

namespace VectorEditorProject;

public partial class MainWindow : Window
{
    private Point? startPoint = null;
    private Polyline? currentPolyline = null;
    private bool isDrawing = false;
    private Avalonia.Media.IBrush selectedColor = Brushes.Black;
    private int Size = 5;
    public MainWindow()
    {
        InitializeComponent();
    }
    int selectedTool = -1;
    void OnSelectTool(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.Tag == null) return;
        selectedTool = Convert.ToInt32(button.Tag);
        startPoint = null;
        isDrawing = false;
        currentPolyline = null;
    }
    void OnSelectColor(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.Tag == null) return;
        switch (Convert.ToInt32(button.Tag))
        {
            case 0:
                selectedColor = Brushes.Red;
                break;
            case 1:
                selectedColor = Brushes.Blue;
                break;
            case 2:
                selectedColor = Brushes.Yellow;
                break;
            case 3:
                selectedColor = Brushes.Green;
                break;
            case 4:
                selectedColor = Brushes.Black;
                break;
            case 5:
                selectedColor = Brushes.White;
                break;
            case 6:
                selectedColor = Brushes.Orange;
                break;
            case 7:
                selectedColor = Brushes.Brown;
                break;
            default:
                selectedColor = Brushes.Black;
                break;
        }
    }
    void SizeSlider(object? sender, RangeBaseValueChangedEventArgs e)
    {
        var slider = sender as Slider;
        if (slider == null) return;
        Size = (int)slider.Value;
    }
    void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var pointerPoint = e.GetCurrentPoint(sender as Control);
        var currentPosition = pointerPoint.Position;
        switch (selectedTool)
        {
            case 0:
                CreateDot(currentPosition);
                startPoint = null;
                break;
            case 1:
                if (startPoint == null)
                {
                    startPoint = currentPosition;
                }
                else
                {
                    Line line = new Line
                    {
                        StartPoint = startPoint.Value,
                        EndPoint = currentPosition,
                        Stroke = selectedColor,
                        StrokeThickness = Size
                    };
                    MyDrawingCanvas.Children.Add(line);
                    startPoint = null;
                }
                break;
            case 2:
                isDrawing = true;
                CreateDot(currentPosition);
                currentPolyline = new Polyline
                {
                    Stroke = selectedColor,
                    StrokeThickness = Size
                };
                currentPolyline.Points.Add(currentPosition);
                MyDrawingCanvas.Children.Add(currentPolyline);
                break;
            default:
                break;
        }
    }
    void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        if (selectedTool == 2 && isDrawing && currentPolyline != null)
        {
            var pointerPoint = e.GetCurrentPoint(sender as Control);
            currentPolyline.Points.Add(pointerPoint.Position);
        }
    }
    void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (selectedTool == 2)
        {
            var pointerPoint = e.GetCurrentPoint(sender as Control);
            CreateDot(pointerPoint.Position);
            isDrawing = false;
            currentPolyline = null;
        }
    }
    private void CreateDot(Point position)
    {
        var ellipse = new Ellipse
        {
            Width = Size,
            Height = Size,
            Fill = selectedColor
        };
        Canvas.SetLeft(ellipse, position.X - (Size / 2.0));
        Canvas.SetTop(ellipse, position.Y - (Size / 2.0));
        MyDrawingCanvas.Children.Add(ellipse);
    }
}