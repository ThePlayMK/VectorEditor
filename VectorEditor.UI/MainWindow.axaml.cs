using System;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.UI.BuilderTools;
using VectorEditor.UI.LayerLogic;
using VectorEditor.UI.Render;
using VectorEditor.UI.UIControllers;

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
        private readonly LayerController _layerController;
        private readonly CanvasController _canvasController;

        private LayerManager Layers { get; } = new();
        private ToolController Tools { get; } = new();
        public DrawingSettings Settings { get; } = new();
        public CommandManager CommandManager { get; } = new();

        public Canvas CanvasCanvas => _myCanvas!;
        //public Layer SelectedLayerModel => _selectedLayer?.LayerModel ?? Layers.RootLayer;
        public Layer SelectedLayerModel =>
            _layerController.SelectedLayer?.LayerModel ?? Layers.RootLayer;


        public MainWindow()
        {
            InitializeComponent();
            _myCanvas = this.FindControl<Canvas>("MyCanvas");
            var renderer = new CanvasRenderer(CanvasCanvas);
            _layerController = new LayerController(Layers);
            _canvasController = new CanvasController();


            CommandManager.OnChanged += () =>
                renderer.Render(Layers.RootLayer, Layers.Layers);
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
            
            Tools.SetTool(button.Tag switch
            {
                "Line" => new LineTool(),
                "Rectangle" => new RectangleTool(),
                "Triangle" => new TriangleTool(),
                _ => null
            });

        }

        private void ToggleMenu(object? sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void SelectColor(object? sender, RoutedEventArgs e)
        {
            if (e.Source is not Button { Background: ISolidColorBrush brush }) return;

            Color selectedColor = brush.Color;

            Settings.ContourColor = selectedColor;              // ← NOWE

            UpdateColor(brush, selectedColor.R, selectedColor.G, selectedColor.B);
        }
        
        private void UpdateColor(IBrush color, int r, int g, int b)
        {
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
            
            var newColor = Color.FromRgb((byte)r, (byte)g, (byte)b);


            Settings.ContourColor = newColor;                      // ← NOWE

            SelectedColor.Background = new SolidColorBrush(newColor);

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
            //var svgContent = await reader.ReadToEndAsync();
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
            _layerController.Reset();
            Settings.Opacity = 100;
            OpacityInput.Text = "100";
            OpacitySlider.Value = 100;
            UpdateColor(Brushes.Black, 0, 0, 0);
            LayersStackPanel.Children.Clear();
            _activeToolButton?.Classes.Remove("Selected");
            _activeToolButton = null;
            CenterCanvas();
        }

        /*private void AddLayer(object? sender, RoutedEventArgs e)
        {
            _layerCount++;
            var newLayer = new LayerWidget();
            newLayer.SetLayerName($"Layer{_layerCount}");
            LayersStackPanel.Children.Insert(0, newLayer);
        }*/
        private void AddLayer(object? sender, RoutedEventArgs e)
        {
            _layerController.AddLayer(LayersStackPanel);
        }

        /*private void SelectLayer(object? sender, RoutedEventArgs e)
        {
            if (e.Source is not Button button) return;
            var oldBtn = _selectedLayer?.FindDescendantOfType<Button>();
            if (oldBtn != null) oldBtn.Background = Brushes.Transparent;
            _selectedLayer = button.FindAncestorOfType<LayerWidget>();
            if (_selectedLayer != null)
            {
                button.Background = Brushes.Gray;
            }
        }*/
        private void SelectLayer(object? sender, RoutedEventArgs e)
        {
            if (e.Source is not Button button) return;
            _layerController.SelectLayer(button);
        }

        /*private void RemoveLayer(object? sender, RoutedEventArgs e)
        {
            if (_selectedLayer == null) return;
            LayersStackPanel.Children.Remove(_selectedLayer);
            _selectedLayer = null;
        }*/
        private void RemoveLayer(object? sender, RoutedEventArgs e)
        {
            _layerController.RemoveSelectedLayer(LayersStackPanel);
        }
        
        private void Canvas_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            CanvasController.Zoom(_myCanvas!, e);
        }
        
        private void CenterCanvas()
        {
            CanvasController.CenterCanvas(_myCanvas!);
        }
        
        private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            Tools.PointerPressed(this, e);

            if (_activeToolButton?.Tag as string == "Hand" && sender is Border border)
                _canvasController.PanStart(border, e);
        }
        
        private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            Tools.PointerMoved(this, e);

            if (_activeToolButton?.Tag as string == "Hand" && sender is Border border)
                _canvasController.PanMove(_myCanvas!, border, e);
        }
        
        private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            Tools.PointerReleased(this, e);
            _canvasController.PanEnd(e);
        }

        private void Opacity_SliderChanged(object? sender, RoutedEventArgs e)
        {
            Settings.Opacity = (int)OpacitySlider.Value;
            OpacityInput.Text = Settings.Opacity.ToString(CultureInfo.InvariantCulture);
        }

        private void Opacity_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (e.Delta.Y > 0 && Settings.Opacity < 100)
            {
                Settings.Opacity++;
                OpacitySlider.Value = Settings.Opacity;
                OpacityInput.Text = Settings.Opacity.ToString(CultureInfo.InvariantCulture);
            }
            else if (e.Delta.Y < 0 && Settings.Opacity > 0)
            {
                Settings.Opacity--;
                OpacitySlider.Value = Settings.Opacity;
                OpacityInput.Text = Settings.Opacity.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void Opacity_InputChange(object? sender, RoutedEventArgs e)
        {
            Settings.Opacity = int.TryParse(OpacityInput.Text, out int result) ? Math.Clamp(result, 0, 100) : 0;
            OpacitySlider.Value = Settings.Opacity;
            var newText = Settings.Opacity.ToString(CultureInfo.InvariantCulture);
            if (OpacityInput.Text == newText) return;
            OpacityInput.Text = newText;
            OpacityInput.CaretIndex = OpacityInput.Text.Length;
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyModifiers)
            {
                case KeyModifiers.Control when e.Key == Key.Z:
                    CommandManager.Undo();
                    return;
                case KeyModifiers.Control when e.Key == Key.Y:
                    CommandManager.Redo();
                    return;
                default:
                    return;
            }
        }
        
        private void Undo_Click(object? sender, RoutedEventArgs e)
        {
            CommandManager.Undo();
        }

        private void Redo_Click(object? sender, RoutedEventArgs e)
        {
            CommandManager.Redo();
        }
    }
}