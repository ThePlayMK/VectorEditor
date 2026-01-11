using System.Linq;
using Avalonia.Input;
using VectorEditor.Core.Command;
using VectorEditor.Core.Strategy;
using VectorEditor.UI.Select;
using VectorEditor.UI.Tools.BuilderTools;
using VectorEditor.UI.UIControllers;

namespace VectorEditor.UI.Tools.StrategyTools;

public class RemoveTool(SelectionManager selection) : ITool
{
    private const bool ClearsSelection = false;

    public bool ClearsSelectionBeforeUse() => ClearsSelection;

    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
        // Nie potrzebne dla tego narzędzia
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        // Nie potrzebne dla tego narzędzia
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e)
    {
        // Nie potrzebne dla tego narzędzia
    }

    public void ApplyRemove(MainWindow window)
    {
        if (selection.Selected.Count == 0)
            return;

        var actionable = selection.Selected
            .Where(canvas => !canvas.IsBlocked)
            .ToList();

        if (actionable.Count == 0)
            return; // Nic do zrobienia, nie twórz komendy

        var strategy = new RemoveStrategy();
        var cmd = new ApplyStrategyCommand(strategy, selection.Selected);
        window.CommandManager.Execute(cmd);
        selection.Clear();
    }
}