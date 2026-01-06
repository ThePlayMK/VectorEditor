using Avalonia.Controls;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI;

public partial class LayerWidget : UserControl
{
    public Layer LayerModel { get; set; }
    public LayerWidget()
    {
        InitializeComponent();
    }

    public void SetLayerName(string name)
    {
        LayerNameBlock.Text = name;
    }
}