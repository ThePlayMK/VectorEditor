using System;
using Avalonia.Controls;
using Avalonia.Media;

namespace VectorEditor.UI.UIControllers;

public class ColorController(
    Func<Color> getColor,
    Action<Color> setColor,
    Panel preview,
    TextBox r,
    TextBox g,
    TextBox b)
{
    public TextBox R => r;
    public TextBox G => g;
    public TextBox B => b;
    public event Action<Color>? CommitEdit;

    private void SetColorFromInput(bool commit = true)
    {
        var r1 = Parse(r.Text!);
        var g1 = Parse(g.Text!);
        var b1 = Parse(b.Text!);

        var color = Color.FromRgb((byte)r1, (byte)g1, (byte)b1);

        setColor(color);
        preview.Background = new SolidColorBrush(color);

        r.Text = r1.ToString();
        g.Text = g1.ToString();
        b.Text = b1.ToString();

        if (commit)
            CommitEdit?.Invoke(color);
    }

    // Obsługa palety
    public void OnPaletteClick(Button button)
    {
        if (button.Background is not ISolidColorBrush brush)
        {
            return;
        }
        r.Text = brush.Color.R.ToString();
        g.Text = brush.Color.G.ToString();
        b.Text = brush.Color.B.ToString();

        SetColorFromInput(commit: true);
    }

    // Aktualizacja UI na każdym wpisywanym znaku w TextBoxach
    public void OnRgbInputChange()
    {
        SetColorFromInput(commit: false);
    }

    // Commit z Enter / LostFocus
    public void CommitFromInput()
    {
        SetColorFromInput(commit: true);
    }

    private static int Parse(string s)
        => int.TryParse(s, out var v) ? Math.Clamp(v, 0, 255) : 0;

    public void Reset()
    {
        var c = Colors.Black;

        setColor(c);
        preview.Background = new SolidColorBrush(c);

        r.Text = c.R.ToString();
        g.Text = c.G.ToString();
        b.Text = c.B.ToString();
    }

    public void UpdateUi()
    {
        var c = getColor();

        preview.Background = new SolidColorBrush(c);
        r.Text = c.R.ToString();
        g.Text = c.G.ToString();
        b.Text = c.B.ToString();
    }
}