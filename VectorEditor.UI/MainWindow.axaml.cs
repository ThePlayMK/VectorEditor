using System.IO;
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

    private enum ColorMode
    {
        Stroke,
        Fill
    }

    private ColorMode _activeColorMode = ColorMode.Stroke;


    private readonly LayerController _layerController;
    private readonly CanvasController _canvasController;
    private readonly CommandController _commandController;
    private readonly ToolController _tools;
    private readonly OpacityController _opacity;
    private readonly ColorController _color;
    private LayerManager Layers { get; } = new();

    public DrawingSettings Settings { get; } = new();
    public CommandManager CommandManager { get; } = new();
    public Canvas CanvasCanvas => _myCanvas!;

    public Layer SelectedLayerModel =>
        _layerController.SelectedLayerWidget?.LayerModel ?? Layers.RootLayer;


    public MainWindow()
    {
        InitializeComponent();
        _myCanvas = this.FindControl<Canvas>("MyCanvas");
        var selectionManager = new SelectionManager();
        var renderer = new CanvasRenderer(CanvasCanvas);
        _layerController = new LayerController(Layers, CommandManager, selectionManager);
        _canvasController = new CanvasController();
        _tools = new ToolController(selectionManager);
        var layersPanel = this.FindControl<StackPanel>("LayersStackPanel");
        var breadcrumb = this.FindControl<StackPanel>("LayerBreadcrumb");
        _layerController.BindUi(layersPanel, breadcrumb);


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

        _color = new ColorController(
            () => _activeColorMode == ColorMode.Stroke ? Settings.ContourColor : Settings.ContentColor,
            c =>
            {
                if (_activeColorMode == ColorMode.Stroke)
                    Settings.ContourColor = c;
                else
                    Settings.ContentColor = c;
            },
            ColorPreview,
            InputColorR, InputColorG, InputColorB
        );


        //Siatka
        _tools.Grid.IsVisible = true;
        _tools.Grid.SnapEnabled = true;

        // Rysujemy siatkę na płótnie
        GridRenderer.Render(CanvasCanvas, _tools.Grid);


        CommandManager.OnChanged += () =>
        {
            renderer.Render(Layers.RootLayer, selectionManager.Selected);
            _layerController.RefreshUi();
        };

        selectionManager.OnChanged += () => renderer.Render(Layers.RootLayer, selectionManager.Selected);
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

    private async void SaveFile(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;
        var fileToSave = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save as SVG",
            DefaultExtension = ".svg",
            SuggestedFileName = "project.svg",
        });
        if (fileToSave is null) return;
        await using var writeStream = await fileToSave.OpenWriteAsync();
        await using var writer = new StreamWriter(writeStream);
        string projectData = "test";
        await writer.WriteAsync(projectData);
        // This needs to be handled to do something with this
    }

    private void NewProject(object? sender, RoutedEventArgs e)
    {
        _layerController.ResetUi();
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
        _layerController.AddLayer();
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
        => _layerController.RemoveSelectedLayer();

    private void Canvas_PointerWheelChanged(object? s, PointerWheelEventArgs e)
        => _canvasController.OnPointerWheel(_myCanvas!, e);

    private void Canvas_PointerPressed(object? s, PointerPressedEventArgs e)
    {
        _tools.PointerPressed(this, e);
        if (_tools.IsHandToolActive && s is Border b)
            _canvasController.OnPointerPressed(b, e);
    }

    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        _tools.PointerMoved(this, e);

        if (_tools.IsHandToolActive && sender is Border border)
            _canvasController.OnPointerMoved(_myCanvas!, border, e);
    }

    private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _tools.PointerReleased(this, e);
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
        if (sender is CheckBox checkBox)
        {
            // Pobieramy stan (zaznaczony lub nie) - null traktujemy jako false
            bool isChecked = checkBox.IsChecked ?? false;

            // 1. Aktualizujemy logikę w Core
            _tools.Grid.IsVisible = isChecked;
        
            // Opcjonalnie: Wyłączamy też przyciąganie, gdy siatka jest niewidoczna.
            // (Zazwyczaj użytkownik nie chce, by myszka "skakała", gdy nie widzi kratek).
            _tools.Grid.SnapEnabled = isChecked;

            // 2. Odświeżamy widok (Renderowanie)
            GridRenderer.Render(CanvasCanvas, _tools.Grid);
        }
    }
}