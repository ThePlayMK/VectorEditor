using Avalonia.Controls;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI;

public partial class CanvasWidget : UserControl
{
    public Layer LayerModel
    {
        get => (Layer)Tag!;
        init => Tag = value;
    }

    public CanvasWidget()
    {
        InitializeComponent();
    }

    public void SetLayerName(string name)
    {
        LayerNameBlock.Text = name;
    }
}