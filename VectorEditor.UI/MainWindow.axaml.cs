using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace VectorEditor.UI
{
    public partial class MainWindow : Window
    {
        private readonly Canvas? _myCanvas;

        private readonly FilePickerFileType _svgFiles = new("SVG Images")
        {
            Patterns = ["*.svg", "*.SVG"],
            AppleUniformTypeIdentifiers = ["public.svg-image"],
            MimeTypes = ["image/svg+xml"]
        };

        private Button? _activeToolButton;
        private Control? _capturedControl;
        private Point _initialMousePosition;
        private bool _isDragging;
        private int _layerCount;
        private int _opacity = 100;
        private IBrush _selectedColor = new SolidColorBrush(Colors.Black);

        private LayerWidget? _selectedLayer;

        public MainWindow()
        {
            InitializeComponent();
            _myCanvas = this.FindControl<Canvas>("MyCanvas");
        }

        private void ToggleThemeChange(object? sender, RoutedEventArgs e)
        {
            RequestedThemeVariant = (ActualThemeVariant == ThemeVariant.Dark)
                ? ThemeVariant.Light
                : ThemeVariant.Dark;
        }

        private void SelectTool(object? sender, RoutedEventArgs e)
        {
            if (e.Source is not Button button) return;
            _activeToolButton?.Classes.Remove("Selected");
            _activeToolButton = button;
            _activeToolButton.Classes.Add("Selected");
        }

        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void SelectColor(object? sender, RoutedEventArgs e)
        {
            if (e.Source is not Button { Background: ISolidColorBrush brush }) return;
            Color selectedColor = brush.Color;
            UpdateColor(brush, selectedColor.R, selectedColor.G, selectedColor.B);
        }

        private void UpdateColor(IBrush color, int r, int g, int b)
        {
            _selectedColor = color;
            SelectedColor.Background = color;
            InputColorR.Text = r.ToString();
            InputColorG.Text = g.ToString();
            InputColorB.Text = b.ToString();
        }

        private void Color_InputChange(object? sender, RoutedEventArgs e)
        {
            if (int.TryParse(InputColorR.Text, out int r))
            {
                r = Math.Clamp(r, 0, 255);
            }
            else
            {
                r = 0;
            }

            if (InputColorR.Text != r.ToString())
            {
                InputColorR.Text = r.ToString();
                InputColorR.CaretIndex = InputColorR.Text.Length;
            }

            if (int.TryParse(InputColorG.Text, out int g))
            {
                g = Math.Clamp(g, 0, 255);
            }
            else
            {
                g = 0;
            }

            if (InputColorG.Text != g.ToString())
            {
                InputColorG.Text = g.ToString();
                InputColorG.CaretIndex = InputColorG.Text.Length;
            }

            if (int.TryParse(InputColorB.Text, out int b))
            {
                b = Math.Clamp(b, 0, 255);
            }
            else
            {
                b = 0;
            }

            if (InputColorB.Text != b.ToString())
            {
                InputColorB.Text = b.ToString();
                InputColorB.CaretIndex = InputColorB.Text.Length;
            }

            byte a = (byte)(_opacity / 100.0 * 255);
            Color newColor = Color.FromArgb(a, (byte)r, (byte)g, (byte)b);

            var newBrush = new SolidColorBrush(newColor);
            _selectedColor = newBrush;
            SelectedColor.Background = newBrush;
        }

        private async void OpenFile(object? sender, RoutedEventArgs e)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel == null) return;
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select an SVG File",
                FileTypeFilter = [_svgFiles],
                AllowMultiple = false
            });
            if (files.Count < 1) return;
            await using var stream = await files[0].OpenReadAsync();
            using var reader = new StreamReader(stream);
            var svgContent = await reader.ReadToEndAsync();
            // This needs to be handled to do something with this
        }

        private async void SaveFile(object? sender, RoutedEventArgs e)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel == null) return;
            var fileToSave = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save as SVG",
                DefaultExtension = ".svg",
                SuggestedFileName = "project.svg",
            });
            if (fileToSave is null) return;
            await using var writeStream = await fileToSave.OpenWriteAsync();
            await using var writer = new StreamWriter(writeStream);
            string projectData = "test";
            await writer.WriteAsync(projectData);
            // This needs to be handled to do something with this
        }

        private void NewProject(object? sender, RoutedEventArgs e)
        {
            _layerCount = 0;
            _opacity = 100;
            OpacityInput.Text = "100";
            OpacitySlider.Value = 100;
            UpdateColor(Brushes.Black, 0, 0, 0);
            LayersStackPanel.Children.Clear();
            _selectedLayer = null;
            _activeToolButton?.Classes.Remove("Selected");
            _activeToolButton = null;
            CenterCanvas();
        }

        private void AddLayer(object? sender, RoutedEventArgs e)
        {
            _layerCount++;
            var newLayer = new LayerWidget();
            newLayer.SetLayerName($"Layer{_layerCount}");
            LayersStackPanel.Children.Insert(0, newLayer);
        }

        private void SelectLayer(object? sender, RoutedEventArgs e)
        {
            if (e.Source is not Button button) return;
            var oldBtn = _selectedLayer?.FindDescendantOfType<Button>();
            if (oldBtn != null) oldBtn.Background = Brushes.Transparent;
            _selectedLayer = button.FindAncestorOfType<LayerWidget>();
            if (_selectedLayer != null)
            {
                button.Background = Brushes.Gray;
            }
        }

        private void RemoveLayer(object? sender, RoutedEventArgs e)
        {
            if (_selectedLayer == null) return;
            LayersStackPanel.Children.Remove(_selectedLayer);
            _selectedLayer = null;
        }

        private void Canvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (_myCanvas?.RenderTransform is not MatrixTransform transform) return;
            var matrix = transform.Matrix;
            var scaleFactor = e.Delta.Y > 0 ? 1.1 : 0.9;
            var point = e.GetPosition(_myCanvas);
            matrix = MatrixHelper.ScaleAt(matrix, scaleFactor, scaleFactor, point.X, point.Y);
            transform.Matrix = matrix;
            e.Handled = true;
        }

        private void CenterCanvas()
        {
            if (_myCanvas?.RenderTransform is not MatrixTransform transform) return;
            if (_myCanvas.Parent is not Control container) return;
            var canvasBounds = _myCanvas.Bounds;
            var containerBounds = container.Bounds;
            var targetX = (containerBounds.Width - canvasBounds.Width) / 2;
            var targetY = (containerBounds.Height - canvasBounds.Height) / 2;
            var offsetX = targetX - canvasBounds.X;
            var offsetY = targetY - canvasBounds.Y;
            transform.Matrix = Matrix.CreateTranslation(offsetX, offsetY);
        }

        private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            if (properties.IsLeftButtonPressed && sender is Border border)
            {
                _isDragging = true;
                _initialMousePosition = e.GetPosition(border);
                e.Pointer.Capture(border);
                _capturedControl = border;
            }
        }

        private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging && sender is Border border && _myCanvas?.RenderTransform is MatrixTransform transform &&
                _activeToolButton?.Tag as string == "Hand")
            {
                var currentPosition = e.GetPosition(border);
                var offset = currentPosition - _initialMousePosition;

                var matrix = transform.Matrix;
                matrix = MatrixHelper.Translate(matrix, offset.X, offset.Y);

                transform.Matrix = matrix;
                _initialMousePosition = currentPosition;
            }
        }

        private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDragging = false;
            if (_capturedControl == null) return;
            e.Pointer.Capture(null);
            _capturedControl = null;
        }

        private void Opacity_SliderChanged(object? sender, RoutedEventArgs e)
        {
            _opacity = (int)OpacitySlider.Value;
            OpacityInput.Text = _opacity.ToString();
        }

        private void Opacity_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y > 0 && _opacity < 100)
            {
                _opacity++;
                OpacitySlider.Value = _opacity;
                OpacityInput.Text = _opacity.ToString();
            }
            else if (e.Delta.Y < 0 && _opacity > 0)
            {
                _opacity--;
                OpacitySlider.Value = _opacity;
                OpacityInput.Text = _opacity.ToString();
            }
        }

        private void Opacity_InputChange(object? sender, RoutedEventArgs e)
        {
            _opacity = int.TryParse(OpacityInput.Text, out int result) ? Math.Clamp(result, 0, 100) : 0;
            OpacitySlider.Value = _opacity;
            string newText = _opacity.ToString();
            if (OpacityInput.Text == newText) return;
            OpacityInput.Text = newText;
            OpacityInput.CaretIndex = OpacityInput.Text.Length;
        }
    }

    public static class MatrixHelper
    {
        public static Matrix ScaleAt(Matrix matrix, double scaleX, double scaleY, double centerX, double centerY)
        {
            return matrix * Matrix.CreateTranslation(-centerX, -centerY)
                          * Matrix.CreateScale(scaleX, scaleY)
                          * Matrix.CreateTranslation(centerX, centerY);
        }

        public static Matrix Translate(Matrix matrix, double offsetX, double offsetY)
        {
            return matrix * Matrix.CreateTranslation(offsetX, offsetY);
        }
    }
}