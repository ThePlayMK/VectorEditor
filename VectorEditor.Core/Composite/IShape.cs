using Avalonia.Media;

namespace VectorEditor.Core.Composite;

public interface IShape : ICanvas
{
    public void SetContentColor(Color newColor);
    public void SetContourColor(Color newColor);
    public void SetOpacity(double transparency);
    public void SetWidth(int width);
    public Color GetContentColor();
    public Color GetContourColor();
    public double GetOpacity();
    public double GetWidth();
    
}