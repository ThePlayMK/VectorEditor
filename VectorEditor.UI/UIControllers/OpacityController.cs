using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using VectorEditor.UI.Render;

namespace VectorEditor.UI.UIControllers;

public class OpacityController
{
    private readonly DrawingSettings _settings;
    private readonly Slider _slider;
    private readonly TextBox _input;
    private double _liveOpacity;
    private bool _isDragging;

    
    public event Action<double>? CommitEdit;
    
    public OpacityController(DrawingSettings settings, Slider slider, TextBox input)
    {
        _settings = settings;
        _slider = slider;
        _input = input;

        input.LostFocus += (_, _) => Commit();
        
        slider.AddHandler(
            InputElement.PointerPressedEvent,
            OnSliderPointerPressed,
            Avalonia.Interactivity.RoutingStrategies.Tunnel,
            handledEventsToo: true);

        slider.AddHandler(
            InputElement.PointerReleasedEvent,
            OnSliderPointerReleased,
            Avalonia.Interactivity.RoutingStrategies.Tunnel,
            handledEventsToo: true);
    }
    
    private void OnSliderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = true;
    }
    
    private void OnSliderPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            Commit();
        }
    }



    public void OnSliderChanged()
    {
        var value = Math.Round(_slider.Value);
        _settings.Opacity = value;
        _input.Text = ((int)value).ToString(CultureInfo.InvariantCulture);

        _liveOpacity = value;
    }

    public void OnInputChanged()
    {
        var value = double.TryParse(_input.Text, out var result) ? Math.Clamp(result, 0, 100) : 0;
        _settings.Opacity = value;
        _slider.Value = value;
        _input.Text = value.ToString(CultureInfo.InvariantCulture);
        _input.CaretIndex = _input.Text.Length;

        _liveOpacity = value;
        
    }

    public void OnWheel(PointerWheelEventArgs e)
    {
        switch (e.Delta.Y)
        {
            case > 0 when _settings.Opacity < 100:
                _settings.Opacity++;
                break;
            case < 0 when _settings.Opacity > 0:
                _settings.Opacity--;
                break;
        }

        _slider.Value = _settings.Opacity;
        _input.Text = _settings.Opacity.ToString(CultureInfo.InvariantCulture);
        _liveOpacity = _settings.Opacity;
    }

    private void Commit()
    {
        CommitEdit?.Invoke(_liveOpacity); // now commit
    }
    

    public void Reset()
    {
        _settings.Opacity = 100;
        _slider.Value = 100;
        _input.Text = "100";
        _liveOpacity = 100;
    }
}
