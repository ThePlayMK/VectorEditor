using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace VectorEditor.UI
{
    public partial class MainWindow : Window
    {
        private Button? _activeToolButton;
        private IBrush _selectedColor =  new SolidColorBrush(Colors.Black);
        public MainWindow()
        {
            InitializeComponent();
        }

        void ToggleThemeChange(object? sender, RoutedEventArgs? e)
        {
            if (ActualThemeVariant == ThemeVariant.Dark)
            {
                RequestedThemeVariant = ThemeVariant.Light;
            }
            else
            {
                RequestedThemeVariant = ThemeVariant.Dark;
            }
        }

        void SelectTool(object? sender, RoutedEventArgs? e)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null) return;
            if (_activeToolButton != null)
            {
                _activeToolButton.Classes.Remove("Selected");
            }
            clickedButton.Classes.Add("Selected");
            _activeToolButton = clickedButton;
        }

        void UpdateColor(IBrush color)
        {
            _selectedColor = color;
            SelectedColor.Background = color;
        }
        
        void ToggleMenu(object? sender, RoutedEventArgs? e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }
        
        public void OnColorButtonClick(object? sender, RoutedEventArgs? e)
        {
            var button = sender as Button;

            if (button?.Background != null)
            {
                UpdateColor(button.Background);
            }
        }
    }
}