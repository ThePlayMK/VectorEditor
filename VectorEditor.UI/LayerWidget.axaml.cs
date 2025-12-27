using Avalonia.Controls;

namespace VectorEditor.UI;

public partial class LayerWidget : UserControl
{
    public LayerWidget()
    {
        InitializeComponent();
    }

    public void SetLayerName(string name)
    {
        LayerNameBlock.Text = name;
    }
}