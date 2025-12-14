using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Material.Icons;
using System;
using Avalonia.Controls.Primitives;

namespace VectorEditor.UI
{
    public partial class MainWindow : Window
    {
        private Button? _lastSelectedButton;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void ToggleMenu(object sender, RoutedEventArgs args)
        {
            MenuOverlay.IsVisible = !MenuOverlay.IsVisible;
        }
        void ToggleLayer(object sender, RoutedEventArgs args)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null) return;
            var iconButton = clickedButton.Content as Material.Icons.Avalonia.MaterialIcon;
            if(iconButton==null) return;
            if (iconButton.Kind == MaterialIconKind.Eye)
            {
                iconButton.Kind = MaterialIconKind.EyeOff;
            }
            else iconButton.Kind = MaterialIconKind.Eye;
        }
        void SelectColor(object sender, RoutedEventArgs args)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null) return;
            if (_lastSelectedButton != null)
            {
                _lastSelectedButton.BorderThickness = new Thickness(0);
            }
            clickedButton.BorderBrush = Brushes.White;
            clickedButton.BorderThickness = new Thickness(3);
            clickedButton.CornerRadius = new CornerRadius(4);
            _lastSelectedButton = clickedButton;
        }
        void SelectTool(object sender, RoutedEventArgs args)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null) return;
            if (_lastSelectedButton != null)
            {
                _lastSelectedButton.BorderThickness = new Thickness(0);
            }
            clickedButton.BorderBrush = Brushes.White;
            clickedButton.BorderThickness = new Thickness(3);
            clickedButton.CornerRadius = new CornerRadius(4);
            _lastSelectedButton = clickedButton;
        }
    }
}