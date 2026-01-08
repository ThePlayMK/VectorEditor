using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Command;

public static class Clipboard
{
    // Przechowujemy klony, aby oryginały mogły żyć własnym życiem
    private static readonly List<ICanvas> Content = [];

    public static void Copy(IEnumerable<ICanvas> elements)
    {
        Content.Clear();
        foreach (var el in elements)
        {
            Content.Add(el.Clone());
        }
    }

    public static List<ICanvas> Paste()
    {
        // Ponownie klonujemy przy wklejaniu, 
        // aby można było wkleić to samo kilka razy pod rząd
        return Content.Select(el => el.Clone()).ToList();
    }
}