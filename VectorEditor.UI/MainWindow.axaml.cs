using System.IO;
using System;
using System.Threading.Tasks; // Naprawia błąd "Task<>"
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.UI.LayerLogic;
using VectorEditor.UI.Render;
using VectorEditor.UI.Select;
using VectorEditor.UI.UIControllers;
using VectorEditor.Core.State;
using VectorEditor.Core.Strategy;


namespace VectorEditor.UI;

public partial class MainWindow : Window
{
    private readonly Canvas? _myCanvas;

    private readonly FilePickerFileType _svgFiles = new("SVG Images")
    {
        Patterns = ["*.svg", "*.SVG"],
        AppleUniformTypeIdentifiers = ["public.svg-image"],
        MimeTypes = ["image/svg+xml"]
    };

    private ColorMode _activeColorMode = ColorMode.Stroke;


    public readonly LayerController LayerController;
    private readonly CanvasController _canvasController;
    private readonly CommandController _commandController;
    private readonly ToolController _tools;
    private readonly OpacityController _opacity;
    private readonly ColorController _color;
    private readonly CanvasRenderer _renderer;
    public LayerManager Layers { get; } = new();

    public DrawingSettings Settings { get; } = new();
    public CommandManager CommandManager { get; }
    public CanvasRenderer Renderer => _renderer;
    public Canvas CanvasCanvas => _myCanvas!;
    private readonly EditorContext _editorContext;

    private Layer SelectedLayerModel =>
        LayerController.SelectedLayerWidget?.LayerModel ?? Layers.RootLayer;


    public MainWindow()
    {
        InitializeComponent();
        _editorContext = new EditorContext();
        CommandManager = new CommandManager(_editorContext);
        _myCanvas = this.FindControl<Canvas>("MyCanvas");
        var selectionManager = new SelectionManager();
        _renderer = new CanvasRenderer(CanvasCanvas);
        LayerController = new LayerController(Layers, CommandManager, selectionManager);
        _canvasController = new CanvasController();
        _tools = new ToolController(selectionManager, this);
        var layersPanel = this.FindControl<StackPanel>("LayersStackPanel");
        var layerGoBack = this.FindControl<StackPanel>("LayerGoBack");
        var dropIndicator = this.FindControl<Border>("DropIndicator");
        LayerController.BindUi(layersPanel, layerGoBack, dropIndicator);
        _editorContext = new EditorContext();


        _commandController = new CommandController(
            CommandManager,
            selectionManager,
            () => SelectedLayerModel
        );

        _opacity = new OpacityController(
            Settings,
            OpacitySlider,
            OpacityInput
        );
        
        _opacity.CommitEdit += value =>
        {
            if (selectionManager.Selected.Count == 0)
                return;

            // Create strategy and command only on commit
            var strategy = new SetOpacityStrategy(value / 100);
            var cmd = new ApplyStrategyCommand(strategy, selectionManager.Selected);
            CommandManager.Execute(cmd);

            // Optional: refresh canvas
            _renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);
        };
        

        _color = new ColorController(
            () => _activeColorMode == ColorMode.Stroke
                ? Settings.ContourColor
                : Settings.ContentColor,

            c =>
            {
                if (_activeColorMode == ColorMode.Stroke)
                    Settings.ContourColor = c;
                else
                    Settings.ContentColor = c;

                _color!.UpdateUi();
            },
            ColorPreview,
            InputColorR, InputColorG, InputColorB
        );

        _color.CommitEdit += c =>
        {
            if (selectionManager.Selected.Count == 0)
                return;

            IModificationStrategy strategy =
                _activeColorMode == ColorMode.Fill
                    ? new ChangeContentColorStrategy(c)
                    : new ChangeContourColorStrategy(c);

            var cmd = new ApplyStrategyCommand(strategy, selectionManager.Selected);
            CommandManager.Execute(cmd);
        };
        
        _color.R.KeyDown += (_, e) => { if (e.Key == Key.Enter) _color.CommitFromInput(); };
        _color.G.KeyDown += (_, e) => { if (e.Key == Key.Enter) _color.CommitFromInput(); };
        _color.B.KeyDown += (_, e) => { if (e.Key == Key.Enter) _color.CommitFromInput(); };

        //Siatka
        _tools.Grid.IsVisible = true;
        _tools.Grid.SnapEnabled = true;

        // Rysujemy siatkę na płótnie
        GridRenderer.Render(CanvasCanvas, _tools.Grid);


        CommandManager.OnChanged += () =>
        {
            _renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);
            LayerController.RefreshUi();
        };

        selectionManager.OnChanged += () => _renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);
        _tools.OnChanged += () => _renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);
        
        _editorContext.SaveAction = async (path) => await PerformPhysicalSave(path);
    }
    
    private void ColorModeChanged(object? sender, RoutedEventArgs e)
    {
        if (Equals(sender, StrokeModeButton))
        {
            StrokeModeButton.IsChecked = true;
            FillModeButton.IsChecked = false;
            _activeColorMode = ColorMode.Stroke;
        }
        else
        {
            FillModeButton.IsChecked = true;
            StrokeModeButton.IsChecked = false;
            _activeColorMode = ColorMode.Fill;
        }

        _color.UpdateUi();
    }

    private void ToggleThemeChange(object? sender, RoutedEventArgs e)
    {
        RequestedThemeVariant = (ActualThemeVariant == ThemeVariant.Dark)
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
    }

    private void SelectTool(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button button)
            _tools.SelectTool(button);
    }

    private void ToggleMenu(object? sender, RoutedEventArgs e)
    {
        MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
    }

    private void SelectColor(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button button)
            _color.OnPaletteClick(button);
    }

    private void Color_InputChange(object? sender, RoutedEventArgs e)
    {
        _color.OnRgbInputChange();
    }

    private async void OpenFile(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select an SVG File",
            FileTypeFilter = [_svgFiles],
            AllowMultiple = false
        });
        if (files.Count < 1) return;
        await using var stream = await files[0].OpenReadAsync();
        using var reader = new StreamReader(stream);
        //var svgContent = await reader.ReadToEndAsync();
        // This needs to be handled to do something with this
    }

    private void SaveFile(object? sender, RoutedEventArgs e)
    {
        _editorContext.Save();
        // This needs to be handled to do something with this
    }

    private void NewProject(object? sender, RoutedEventArgs e)
    {
        LayerController.ResetUi();
        _opacity.Reset();
        _color.Reset();
        _tools.Reset();
        CanvasController.CenterCanvas(_myCanvas!);
    }

    /*private void AddLayer(object? sender, RoutedEventArgs e)
    {
        _layerCount++;
        var newLayer = new CanvasWidget();
        newLayer.SetLayerName($"Layer{_layerCount}");
        LayersStackPanel.Children.Insert(0, newLayer);
    }*/
    private void AddLayer(object? sender, RoutedEventArgs e)
    {
        LayerController.AddLayer();
    }

    /*private void SelectLayer(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not Button button) return;
        var oldBtn = _selectedLayer?.FindDescendantOfType<Button>();
        if (oldBtn != null) oldBtn.Background = Brushes.Transparent;
        _selectedLayer = button.FindAncestorOfType<CanvasWidget>();
        if (_selectedLayer != null)
        {
            button.Background = Brushes.Gray;
        }
    }*/

    /*private void RemoveLayer(object? sender, RoutedEventArgs e)
    {
        if (_selectedLayer == null) return;
        LayersStackPanel.Children.Remove(_selectedLayer);
        _selectedLayer = null;
    }*/

    private void RemoveLayer(object? s, RoutedEventArgs e)
        => LayerController.RemoveSelectedLayer();

    private void Canvas_PointerWheelChanged(object? s, PointerWheelEventArgs e)
        => _canvasController.OnPointerWheel(_myCanvas!, e);

    private void Canvas_PointerPressed(object? s, PointerPressedEventArgs e)
    {
        _tools.PointerPressed(e);
        if (_tools.IsHandToolActive && s is Border b)
            _canvasController.OnPointerPressed(b, e);
    }

    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        _tools.PointerMoved(e);

        if (_tools.IsHandToolActive && sender is Border border)
            _canvasController.OnPointerMoved(_myCanvas!, border, e);
    }

    private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _tools.PointerReleased(e);
        _canvasController.OnPointerReleased(e);
    }
    private void Opacity_SliderChanged(object? s, RoutedEventArgs e) => _opacity.OnSliderChanged();
    private void Opacity_PointerWheelChanged(object? s, PointerWheelEventArgs e) => _opacity.OnWheel(e);
    private void Opacity_InputChange(object? s, RoutedEventArgs e) => _opacity.OnInputChanged();

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        _commandController.OnKeyDown(e);
    }
    private void Undo_Click(object? s, RoutedEventArgs e) => _commandController.OnUndoClick();
    private void Redo_Click(object? s, RoutedEventArgs e) => _commandController.OnRedoClick();
    
    private void ToggleGrid(object? sender, RoutedEventArgs e)
    {
        // Sprawdzamy, czy to na pewno Checkbox wywołał zdarzenie
        if (sender is not CheckBox checkBox) return;
        // Pobieramy stan (zaznaczony lub nie) - null traktujemy jako false
        var isChecked = checkBox.IsChecked ?? false;

        // 1. Aktualizujemy logikę w Core
        _tools.Grid.IsVisible = isChecked;
        
        // Opcjonalnie: Wyłączamy też przyciąganie, gdy siatka jest niewidoczna.
        // (Zazwyczaj użytkownik nie chce, by myszka "skakała", gdy nie widzi kratek).
        _tools.Grid.SnapEnabled = isChecked;

        // 2. Odświeżamy widok (Renderowanie)
        GridRenderer.Render(CanvasCanvas, _tools.Grid);
    }
    private async Task<bool> PerformPhysicalSave(string? path)
    {
    if (string.IsNullOrEmpty(path))
    {
        var topLevel = GetTopLevel(this); // Upewnij się, że używasz TopLevel.GetTopLevel
        if (topLevel == null) return false;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Zapisz projekt",
            DefaultExtension = "vec"
        });

        if (file == null) return false;
        
        path = file.Path.LocalPath;
        _editorContext.CurrentFilePath = path;
    }

    try 
    {
        var dataToSave = SelectedLayerModel.GetChildren(); 
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(dataToSave, options);
        
        await File.WriteAllTextAsync(path, jsonString);
        
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Błąd zapisu: {ex.Message}");
        return false;
    }
    }
}