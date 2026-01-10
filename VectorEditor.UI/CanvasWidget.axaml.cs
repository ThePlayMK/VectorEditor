using System.Threading;
using Avalonia.Controls;
using Material.Icons;
using Material.Icons.Avalonia;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;
using VectorEditor.UI.Select;

namespace VectorEditor.UI;

public partial class CanvasWidget : UserControl
{
    //private readonly Button _lockButton;
    public Layer LayerModel
    {
        get => (Layer)Tag!;
        init => Tag = value;
    }

    public CanvasWidget()
    {
        InitializeComponent();
       // _lockButton = LockButton;
    }

    public void SetLayerName(string name)
    {
        LayerNameBlock.Text = name;
    }
    
    public void BindLock(
        CommandManager commands,
        SelectionManager selectionManager)
    {
        UpdateLockIcon();

        LockButton.Click += (_, _) =>
        {
            IModificationStrategy strategy =
                LayerModel.IsBlocked
                    ? new UnblockCanvasStrategy()
                    : new BlockCanvasStrategy();

            var cmd = new ApplyStrategyCommand(strategy, LayerModel);
            commands.Execute(cmd);

            if (LayerModel.IsBlocked)
                selectionManager.Clear();

            UpdateLockIcon();
        };
    }

    private void UpdateLockIcon()
    {
        LockButton.Content = new MaterialIcon
        {
            Kind = LayerModel.IsBlocked
                ? MaterialIconKind.Lock
                : MaterialIconKind.LockOpenVariant
        };
    }
    
}