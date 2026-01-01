using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public static class Clipboard
{
    // Przechowujemy klony, aby oryginały mogły żyć własnym życiem
    private static List<ICanvas> _content = [];

    public static void Copy(IEnumerable<ICanvas> elements)
    {
        _content.Clear();
        foreach (var el in elements)
        {
            _content.Add(el.Clone());
        }
    }

    public static List<ICanvas> Paste()
    {
        // Ponownie klonujemy przy wklejaniu, 
        // aby można było wkleić to samo kilka razy pod rząd
        return _content.Select(el => el.Clone()).ToList();
    }
}