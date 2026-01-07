using Avalonia.Input;
using VectorEditor.Core.Command;
using VectorEditor.Core.Command.Select;
using VectorEditor.Core.Strategy;
using VectorEditor.Core.Structures;
using VectorEditor.UI.BuilderTools;

namespace VectorEditor.UI.Tools.CommandTools;

public class MoveTool(SelectionManager selection) : ITool
{
    private Point? _lastMouse;
    private ApplyStrategyCommand? _command;
    private double _accDx;
    private double _accDy;
    


    public void PointerPressed(MainWindow window, PointerPressedEventArgs e)
    {
        if (selection.Selected.Count == 0)
            return;

        var pos = e.GetPosition(window.CanvasCanvas);
        _lastMouse = new Point(pos.X, pos.Y);

        // Komenda zostanie utworzona dopiero przy PointerReleased
        _accDx = 0;
        _accDy = 0;
        

    }

    public void PointerMoved(MainWindow window, PointerEventArgs e)
    {
        if (_lastMouse == null || selection.Selected.Count == 0)
            return;

        var pos = e.GetPosition(window.CanvasCanvas);
        var current = new Point(pos.X, pos.Y);

        var dx = current.X - _lastMouse.X;
        var dy = current.Y - _lastMouse.Y;

        if (dx == 0 && dy == 0)
            return;

        // Akumulujemy przesunięcie
        _accDx += dx;
        _accDy += dy;

        _lastMouse = current;
    }

    public void PointerReleased(MainWindow window, PointerReleasedEventArgs e)
    {
        if (selection.Selected.Count == 0)
            return;

        // Jeśli nie było ruchu — nie tworzymy komendy
        if (_accDx == 0 && _accDy == 0)
            return;

        // Tworzymy strategię i komendę
        var strategy = new MoveCanvasStrategy(_accDx, _accDy);
        _command = new ApplyStrategyCommand(strategy, selection.Selected);

        window.CommandManager.Execute(_command);

        _command = null;
        _lastMouse = null;
    }

}