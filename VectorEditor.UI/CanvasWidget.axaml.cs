using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;
using VectorEditor.UI.Select;

namespace VectorEditor.UI;

public partial class CanvasWidget : UserControl
{
    private ICanvas CanvasModel { get; set; } = null!;

    public Layer LayerModel => (Layer)Tag!;

    public CanvasWidget()
    {
        InitializeComponent();
    }

    [Obsolete("Obsolete")]
    public void Bind(
        ICanvas canvas,
        CommandManager commands,
        SelectionManager selectionManager)
    {
        CanvasModel = canvas;

        LayerNameBlock.Text = canvas.Name;

        BindVisibility(commands);
        BindLock(commands, selectionManager);

        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    private void BindVisibility(CommandManager commands)
    {
        VisibilityButton.Click += (_, _) =>
        {
            IModificationStrategy strategy =
                CanvasModel.IsVisible
                    ? new HideCanvasStrategy()
                    : new ShowCanvasStrategy();
            commands.Execute(new ApplyStrategyCommand(strategy, CanvasModel));

            UpdateVisibilityIcon();
        };
        UpdateVisibilityIcon();
    }

    private void UpdateVisibilityIcon()
    {
        VisibilityButton.IsChecked = CanvasModel.IsVisible;

        VisibilityIcon.Kind = CanvasModel.IsVisible
            ? Material.Icons.MaterialIconKind.Eye
            : Material.Icons.MaterialIconKind.EyeOff;
    }

    private void BindLock(
        CommandManager commands,
        SelectionManager selectionManager)
    {
        LockButton.Click += (_, _) =>
        {
            IModificationStrategy strategy =
                CanvasModel.IsBlocked
                    ? new UnblockCanvasStrategy()
                    : new BlockCanvasStrategy();

            commands.Execute(new ApplyStrategyCommand(strategy, CanvasModel));

            if (CanvasModel.IsBlocked)
                selectionManager.Clear();

            UpdateLockIcon();
        };
        UpdateLockIcon();
    }

    private void UpdateLockIcon()
    {
        LockButton.IsChecked = !CanvasModel.IsBlocked;
    }

    private Point _dragStartPoint;
    private bool _isPressed;

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var pointer = e.GetCurrentPoint(this);
        if (!pointer.Properties.IsLeftButtonPressed)
        {
            return;
        }
        _isPressed = true;
        _dragStartPoint = e.GetPosition(this);

        // Pozwól zdarzeniu polecieć dalej, aby LayerController mógł zaznaczyć warstwę
    }

    [Obsolete("Obsolete")]
    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isPressed) return;

        var currentPoint = e.GetPosition(this);
        var delta = _dragStartPoint - currentPoint;

        // Próg 5 pikseli zapobiega przypadkowemu draggowaniu przy zwykłym kliknięciu
        if (!(Math.Abs(delta.X) > 5) && !(Math.Abs(delta.Y) > 5))
        {
            return;
        }
        _isPressed = false; // Resetujemy stan, by nie odpalać DragDrop wielokrotnie

        var dragData = new DataObject();
        dragData.Set("CanvasModel", this.CanvasModel);

        // Uruchomienie sesji przeciągania
        await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isPressed = false;
    }
}