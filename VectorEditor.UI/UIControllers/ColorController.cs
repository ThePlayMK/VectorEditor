using System;
using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.UI.Render;

namespace VectorEditor.UI.UIControllers;

public class ColorController(DrawingSettings settings, Panel preview, TextBox r, TextBox g, TextBox b)
{
    public void OnColorButtonClick(ISolidColorBrush brush)
    {
        var c = brush.Color;
        settings.ContourColor = c;

        preview.Background = brush;
        r.Text = c.R.ToString();
        g.Text = c.G.ToString();
        b.Text = c.B.ToString();
    }

    public void OnRgbInputChange()
    {
        var r1 = Parse(r.Text!);
        var g1 = Parse(g.Text!);
        var b1 = Parse(b.Text!);

        var color = Color.FromRgb((byte)r1, (byte)g1, (byte)b1);
        settings.ContourColor = color;

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
        settings.ContourColor = c;

        preview.Background = new SolidColorBrush(c);
        r.Text = "0";
        g.Text = "0";
        b.Text = "0";
    }
}