using Avalonia.Media;

namespace VectorEditor.UI.Render;

public class DrawingSettings
{
    public Color ContourColor { get; set; } = Colors.Black;
    public Color ContentColor { get; set; } = Colors.White;
    public double StrokeWidth { get; set; } = 2;
    public double Opacity { get; set; } = 100;
}
