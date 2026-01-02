namespace VectorEditor.Core.Composite;

public interface IShape : ICanvas
{
    string Name { get; }
    public void SetContentColor(string newColor);
    public void SetContourColor(string newColor);
    public void SetTransparency(double transparency);
    public void SetWidth(int width);
    public string GetContentColor();
    public string GetContourColor();
    public double GetTransparency();
    public int GetWidth();
    
}