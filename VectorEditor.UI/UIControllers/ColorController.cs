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

    private void OnColorButtonClick(Color c, bool commit = true)
    {
        setColor(c);
        preview.Background = new SolidColorBrush(c);

        r.Text = c.R.ToString();
        g.Text = c.G.ToString();
        b.Text = c.B.ToString();
        
        if (commit)
            CommitEdit?.Invoke(c);
    }

    // ColorController
    public void OnRgbInputChange()
    {
        var r1 = Parse(r.Text!);
        var g1 = Parse(g.Text!);
        var b1 = Parse(b.Text!);

        var color = Color.FromRgb((byte)r1, (byte)g1, (byte)b1);

        // Tylko aktualizacja UI/podglądu, commit = false
        OnColorButtonClick(color, commit: false);
    }
    
    public void CommitFromInput()
    {
        var r1 = Parse(r.Text!);
        var g1 = Parse(g.Text!);
        var b1 = Parse(b.Text!);

        var color = Color.FromRgb((byte)r1, (byte)g1, (byte)b1);
        preview.Background = new SolidColorBrush(color);

        CommitEdit?.Invoke(color);
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
    
    public void OnPaletteClick(Button button)
    {
        if (button.Background is ISolidColorBrush brush)
            OnColorButtonClick(brush.Color, commit: true);
    }

}
