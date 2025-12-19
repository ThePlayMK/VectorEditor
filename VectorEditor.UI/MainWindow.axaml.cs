using Avalonia.Controls;
using Avalonia.Interactivity;

namespace VectorEditor.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Toggles the hamburger menu
        private void ToggleMenu(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }
    }
}