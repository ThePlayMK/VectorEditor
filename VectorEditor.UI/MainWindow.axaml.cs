using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System;
using Avalonia.Controls.Primitives;

namespace VectorEditor.UI
{
    public partial class MainWindow : Window
    {

        private Point? _startPoint;
        private Polyline? _currentPolyline;
        private bool _isDrawing;
        private IBrush _selectedColor = Brushes.Black;
        private int _size = 5;

        public MainWindow()
        {
            InitializeComponent();
        }

        int _selectedTool = -1;
        void OnSelectTool(object? sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;
            _selectedTool = Convert.ToInt32(button.Tag);
            _startPoint = null;
            _isDrawing = false;
            _currentPolyline = null;
        }
        void OnSelectColor(object? sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;
            switch (Convert.ToInt32(button.Tag))
            {
                case 0:
                    _selectedColor = Brushes.Red;
                    break;
                case 1:
                    _selectedColor = Brushes.Blue;
                    break;
                case 2:
                    _selectedColor = Brushes.Yellow;
                    break;
                case 3:
                    _selectedColor = Brushes.Green;
                    break;
                case 4:
                    _selectedColor = Brushes.Black;
                    break;
                case 5:
                    _selectedColor = Brushes.White;
                    break;
                case 6:
                    _selectedColor = Brushes.Orange;
                    break;
                case 7:
                    _selectedColor = Brushes.Brown;
                    break;
                default:
                    _selectedColor = Brushes.Black;
                    break;
            }
        }
        void SizeSlider(object? sender, RangeBaseValueChangedEventArgs e)
        {
            var slider = sender as Slider;
            if (slider == null) return;
            _size = (int)slider.Value;
        }
        void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(sender as Control);
            var currentPosition = pointerPoint.Position;
            switch (_selectedTool)
            {
                case 0:
                    CreateDot(currentPosition);
                    _startPoint = null;
                    break;
                case 1:
                    if (_startPoint == null)
                    {
                        _startPoint = currentPosition;
                    }
                    else
                    {
                        Line line = new Line
                        {
                            StartPoint = _startPoint.Value,
                            EndPoint = currentPosition,
                            Stroke = _selectedColor,
                            StrokeThickness = _size
                        };
                        MyDrawingCanvas.Children.Add(line);
                        _startPoint = null;
                    }
                    break;
                case 2:
                    _isDrawing = true;
                    CreateDot(currentPosition);
                    _currentPolyline = new Polyline
                    {
                        Stroke = _selectedColor,
                        StrokeThickness = _size
                    };
                    _currentPolyline.Points.Add(currentPosition);
                    MyDrawingCanvas.Children.Add(_currentPolyline);
                    break;
            }
        }
        void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_selectedTool == 2 && _isDrawing && _currentPolyline != null)
            {
                var pointerPoint = e.GetCurrentPoint(sender as Control);
                _currentPolyline.Points.Add(pointerPoint.Position);
            }
        }
        void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_selectedTool == 2)
            {
                var pointerPoint = e.GetCurrentPoint(sender as Control);
                CreateDot(pointerPoint.Position);
                _isDrawing = false;
                _currentPolyline = null;
            }
        }
        private void CreateDot(Point position)
        {
            var ellipse = new Ellipse
            {
                Width = _size,
                Height = _size,
                Fill = _selectedColor
            };
            Canvas.SetLeft(ellipse, position.X - (_size / 2.0));
            Canvas.SetTop(ellipse, position.Y - (_size / 2.0));
            MyDrawingCanvas.Children.Add(ellipse);
        }
    }
}