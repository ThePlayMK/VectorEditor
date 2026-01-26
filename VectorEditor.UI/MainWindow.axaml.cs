using System;
using System.IO;
using System.Threading.Tasks; // Naprawia błąd "Task<>"
using System.Collections.Generic; // <- TO NAPRAWI BŁĄD IEnumerable
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
using VectorEditor.UI.Utils;
using VectorEditor.Core.Strategy;


namespace VectorEditor.UI;

public partial class MainWindow : Window
{
    private readonly Canvas? _myCanvas;

    private ColorMode _activeColorMode = ColorMode.Stroke;


    public readonly LayerController LayerController;
    private readonly CanvasController _canvasController;
    private readonly CommandController _commandController;
    private readonly ToolController _tools;
    private readonly OpacityController _opacity;
    private readonly ColorController _color;
    private readonly ProjectExporter _projectExporter;
    public LayerManager Layers { get; } = new();

    public DrawingSettings Settings { get; } = new();
    public CommandManager CommandManager { get; }
    public CanvasRenderer Renderer { get; }

    public Canvas CanvasCanvas => _myCanvas!;
    private readonly EditorContext _editorContext;

    private Layer SelectedLayerModel =>
        LayerController.SelectedLayerWidget?.LayerModel ?? Layers.RootLayer;


    [Obsolete("Obsolete")]
    public MainWindow()
    {
        InitializeComponent();
        _editorContext = new EditorContext();
        CommandManager = new CommandManager(_editorContext);
        _myCanvas = this.FindControl<Canvas>("MyCanvas");
        var selectionManager = new SelectionManager();
        Renderer = new CanvasRenderer(CanvasCanvas);
        LayerController = new LayerController(Layers, CommandManager, selectionManager);
        _canvasController = new CanvasController();
        _tools = new ToolController(selectionManager, this);
        var layersPanel = this.FindControl<StackPanel>("LayersStackPanel");
        var layerGoBack = this.FindControl<StackPanel>("LayerGoBack");
        var dropIndicator = this.FindControl<Border>("DropIndicator");
        LayerController.BindUi(layersPanel, layerGoBack, dropIndicator);
        _editorContext = new EditorContext();
        _projectExporter = new ProjectExporter(CanvasCanvas);


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
            Renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);
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

        _color.R.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter) _color.CommitFromInput();
        };
        _color.G.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter) _color.CommitFromInput();
        };
        _color.B.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter) _color.CommitFromInput();
        };

        //Siatka
        _tools.Grid.IsVisible = true;
        _tools.Grid.SnapEnabled = true;

        // Rysujemy siatkę na płótnie
        GridRenderer.Render(CanvasCanvas, _tools.Grid);


        CommandManager.OnChanged += () =>
        {
            ValidateCurrentLayerContext();
            Renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);
            LayerController.RefreshUi();
        };

        selectionManager.OnChanged += () => Renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);
        _tools.OnChanged += () => Renderer.Render(Layers.RootLayer, selectionManager.Selected, _tools);

        _editorContext.SaveAction = async (path) => await PerformPhysicalSave(path);
    }
    
    private void ValidateCurrentLayerContext()
    {
        // Sprawdzamy, czy aktualny kontekst nie jest RootLayerem
        // i czy stracił połączenie z drzewem (Parent == null)
        if (Layers.CurrentContext != Layers.RootLayer && Layers.CurrentContext.ParentLayer == null)
        {
            Layers.EnterLayer(Layers.RootLayer);
        }
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

    [Obsolete("Obsolete")]
    private void AddLayer(object? sender, RoutedEventArgs e) => LayerController.AddLayer();

    private void Canvas_PointerWheelChanged(object? s, PointerWheelEventArgs e)
        => CanvasController.OnPointerWheel(_myCanvas!, e);

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

    // Ta metoda wyciągnie wszystkie figury z całego drzewa warstw (rekurencyjnie)
    // Metoda rekurencyjna - wyciąga wszystkie kształty z warstw i podwarstw
    private IEnumerable<IShape> GetAllShapes(Layer rootLayer)
    {
        var allShapes = new List<IShape>();

        // Iterujemy po wszystkich dzieciach (Layer implementuje ICanvas, więc pobieramy dzieci)
        foreach (var child in rootLayer.GetChildren())
        {
            // 1. Jeśli dziecko to kolejna Warstwa -> wchodzimy głębiej (rekurencja)
            if (child is Layer childLayer)
            {
                if (childLayer.IsVisible) // Opcjonalnie: pomijamy ukryte warstwy
                {
                    allShapes.AddRange(GetAllShapes(childLayer));
                }
            }
            // 2. Jeśli dziecko to Kształt (ale nie warstwa) -> dodajemy do listy
            else if (child is IShape shape)
            {
                allShapes.Add(shape);
            }
        }

        return allShapes;
    }

    private void SaveFile(object? sender, RoutedEventArgs e)
    {
        _editorContext.Save();
    }

    private async Task<bool> PerformPhysicalSave(string? path)
    {
        //testy
        //var layerToSave = SelectedLayerModel; // Ta właściwość musi być publiczna!

        // --- DIAGNOSTYKA (ODCISK PALCA) ---
        //System.Diagnostics.Debug.WriteLine($"[SAVE] Używam warstwy ID: {layerToSave.GetHashCode()}");
        //System.Diagnostics.Debug.WriteLine($"[SAVE] Liczba dzieci w tej warstwie: {layerToSave.GetChildren().ToList().Count}");
        // ----------------------------------
        if (string.IsNullOrEmpty(path))
        {
            var topLevel = GetTopLevel(this);
            if (topLevel == null) return false;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Zapisz projekt SVG",
                DefaultExtension = "svg",
                SuggestedFileName = "projekt.svg",
                FileTypeChoices =
                [
                    new FilePickerFileType("SVG Image") { Patterns = ["*.svg"] }
                ]
            });

            if (file == null) return false; // Anulowano

            path = file.Path.LocalPath;

            // ZAPAMIĘTUJEMY ŚCIEŻKĘ! To klucz do "cichego zapisu" w przyszłości
            _editorContext.CurrentFilePath = path;
        }

        // KROK 2: Właściwy zapis "po cichu" (mechanika Export, ale do znanego pliku)
        try
        {
            var root = LayerController.RootLayer;
            var shapes = GetAllShapes(root);

            // Generujemy treść
            var svgContent = SvgExporter.GenerateSvg(shapes, CanvasCanvas.Bounds.Width, CanvasCanvas.Bounds.Height);

            // --- SZPIEG 1: Gdzie zapisuję? ---
            //System.Diagnostics.Debug.WriteLine($"[PATH CHECK] Zapisuję do pliku: {path}");

            // --- SZPIEG 2: Czy treść nie jest pusta? ---
            //System.Diagnostics.Debug.WriteLine($"[CONTENT CHECK] Długość tekstu SVG: {svgContent.Length} znaków");
            // Jeśli długość jest mała (np. < 100), to znaczy, że SVG jest puste!

            await File.WriteAllTextAsync(path, svgContent);

            // --- SZPIEG 3: Potwierdzenie ---
            //System.Diagnostics.Debug.WriteLine($"[DISK CHECK] Fizyczny zapis zakończony o: {DateTime.Now}");

            return true;
        }
        catch
        {
            return false;
        }
    }

    // Podpięcie w XAML: Click="ExportPng"
    private async void ExportPng(object? sender, RoutedEventArgs e)
    {
        await ExportRasterImage("png");
    }

// Podpięcie w XAML: Click="ExportJpg"
    private async void ExportJpg(object? sender, RoutedEventArgs e)
    {
        // Avalonia natywnie zapisuje głównie do PNG. 
        // Zapiszemy jako PNG, ale z rozszerzeniem .jpg (większość przeglądarek to obsłuży),
        // lub po prostu potraktujmy to jako eksport obrazka.
        await ExportRasterImage("jpg");
    }

    private async Task ExportRasterImage(string extension)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Eksportuj do {extension.ToUpper()}",
            DefaultExtension = extension
        });

        if (file != null)
        {
            await _projectExporter.ExportToRaster(file, (isVisible) =>
            {
                // Przekazujemy logikę przełączania siatki jako prosty callback
                _tools.Grid.IsVisible = isVisible;
                GridRenderer.Render(CanvasCanvas, _tools.Grid);
            });
        }
    }

    private async void ExportSvg(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Eksportuj jako SVG",
            DefaultExtension = "svg",
            FileTypeChoices = [new FilePickerFileType("Obraz SVG") { Patterns = ["*.svg"] }]
        });

        if (file != null)
        {
            var shapes = _projectExporter.GetAllShapes(LayerController.RootLayer);
            await _projectExporter.ExportToSvg(shapes, file);
        }
    }
}