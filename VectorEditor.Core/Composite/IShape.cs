using Avalonia.Media;

namespace VectorEditor.Core.Composite;

public interface IShape : ICanvas
{
    string Name { get; }
    public void SetContentColor(Color newColor);
    public void SetContourColor(Color newColor);
    public void SetTransparency(double transparency);
    public void SetWidth(int width);
    public Color GetContentColor();
    public Color GetContourColor();
    public double GetTransparency();
    public int GetWidth();
    
}