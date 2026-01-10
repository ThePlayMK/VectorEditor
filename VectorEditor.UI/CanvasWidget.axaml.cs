using Avalonia.Controls;
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
       // _lockButton = LockButton;
    }

    public void Bind(
        ICanvas canvas,
        CommandManager commands,
        SelectionManager selectionManager)
    {
        CanvasModel = canvas;

        LayerNameBlock.Text = canvas.Name;

        BindVisibility(commands);
        BindLock(commands, selectionManager);
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
        
    }
    
    private void UpdateVisibilityIcon()
    {
        VisibilityButton.IsChecked = CanvasModel.IsVisible;
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
    }

    private void UpdateLockIcon()
    {
        LockButton.IsChecked = !CanvasModel.IsBlocked;
    }
    
}