namespace VectorEditor.Core.Composite;

public interface IShape : ICanvas
{
    string Name { get; }
    string ContourColor { get; set;}
}