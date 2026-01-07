using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using VectorEditor.UI.Render;

namespace VectorEditor.UI.UIControllers;

public class OpacityController(DrawingSettings settings, Slider slider, TextBox input)
{
    public void OnSliderChanged()
    {
        settings.Opacity = slider.Value;
        input.Text = settings.Opacity.ToString(CultureInfo.InvariantCulture);
    }

    public void OnInputChanged()
    {
        settings.Opacity = double.TryParse(input.Text, out var result)
            ? Math.Clamp(result, 0, 100)
            : 0;

        slider.Value = settings.Opacity;
        input.Text = settings.Opacity.ToString(CultureInfo.InvariantCulture);
        input.CaretIndex = input.Text.Length;
    }

    public void OnWheel(PointerWheelEventArgs e)
    {
        switch (e.Delta.Y)
        {
            case > 0 when settings.Opacity < 100:
                settings.Opacity++;
                break;
            case < 0 when settings.Opacity > 0:
                settings.Opacity--;
                break;
        }

        slider.Value = settings.Opacity;
        input.Text = settings.Opacity.ToString(CultureInfo.InvariantCulture);
    }

    public void Reset()
    {
        settings.Opacity = 100;
        slider.Value = 100;
        input.Text = "100";
    }
}
