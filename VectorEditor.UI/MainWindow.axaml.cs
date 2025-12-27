using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Platform.Storage;
using System.IO;
using Avalonia.VisualTree;

namespace VectorEditor.UI
{
    public partial class MainWindow : Window
    {
        private LayerWidget? _selectedLayer;
        private Button? _activeToolButton;
        private IBrush _selectedColor = new SolidColorBrush(Colors.Black);
        private int _layerCount;

        private readonly FilePickerFileType _svgFiles = new("SVG Images")
        {
            Patterns = ["*.svg", "*.SVG"],
            AppleUniformTypeIdentifiers = ["public.svg-image"],
            MimeTypes = ["image/svg+xml"]
        };

        public MainWindow()
        {
            InitializeComponent();
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
            if (e.Source is Button { Background: not null } button)
            {
                UpdateColor(button.Background);
            }
        }

        private void UpdateColor(IBrush color)
        {
            _selectedColor = color;
            SelectedColor.Background = color;
        }

        private async void OpenFile(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
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
                System.Diagnostics.Debug.WriteLine(svgContent);
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
            if (_selectedLayer != null)
            {
                var oldBtn = _selectedLayer.FindDescendantOfType<Button>();
                if (oldBtn != null) oldBtn.Background = Brushes.Transparent;
            }
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
    }
}