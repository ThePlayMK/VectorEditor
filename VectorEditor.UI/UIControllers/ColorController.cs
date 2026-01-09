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
    
    public event Action<Color>? CommitEdit;

    public void OnColorButtonClick(ISolidColorBrush brush)
    {
        var c = brush.Color;

        setColor(c);
        preview.Background = new SolidColorBrush(c);

        r.Text = c.R.ToString();
        g.Text = c.G.ToString();
        b.Text = c.B.ToString();
        
        CommitEdit?.Invoke(c);
    }

    public void OnRgbInputChange()
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
            OnColorButtonClick(brush);
    }


}
