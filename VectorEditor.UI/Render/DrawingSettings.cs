using Avalonia.Media;

namespace VectorEditor.UI.Render;

public class DrawingSettings
{
    public Color Color { get; set; } = Colors.Black;
    public double StrokeWidth { get; set; } = 2;
    public double Opacity { get; set; } = 100;
}
