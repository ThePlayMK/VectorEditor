using System;
using Avalonia.Input;
using VectorEditor.Core.Command;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Strategy;
using VectorEditor.UI.Select;

namespace VectorEditor.UI.UIControllers;

public class CommandController(
    CommandManager commandManager,
    SelectionManager selectionManager,
    Func<Layer> getSelectedLayer)
{
    public void OnKeyDown(KeyEventArgs e)
    {
        switch (e.KeyModifiers)
        {
            // CTRL + Z → Undo
            case KeyModifiers.Control when e.Key == Key.Z:
                OnUndoClick();
                return;
            // CTRL + Y → Redo
            case KeyModifiers.Control when e.Key == Key.Y:
                OnRedoClick();
                return;
            // CTRL + C → Copy
            case KeyModifiers.Control when e.Key == Key.C:
                OnCopyClick();
                return;
            case KeyModifiers.Control when e.Key == Key.X:
                OnCutClick();
                return;
            // CTRL + V → Paste
            case KeyModifiers.Control when e.Key == Key.V:
                OnPasteClick();
                return;
        }

        if (e.Key == Key.Delete)
        {
            OnDeleteClick();
        }
    }

    public void OnUndoClick()
    {
        commandManager.Undo();
        selectionManager.Clear();
    }

    public void OnRedoClick()
    {
        commandManager.Redo();
        selectionManager.Clear();
    }

    private void OnPasteClick()
    {
        var cmd = new PasteCommand(getSelectedLayer());
        commandManager.Execute(cmd);

        selectionManager.Clear();
        selectionManager.AddRange(cmd.PastedElements);
    }

    private void OnCopyClick()
    {
        if (selectionManager.Selected.Count == 0)
            return;

        var copy = new CopyCommand(selectionManager.Selected);
        copy.Execute();
    }

    private void OnDeleteClick()
    {
        if (selectionManager.Selected.Count == 0)
            return;

        var strategy = new RemoveStrategy();
        var cmd = new ApplyStrategyCommand(strategy, selectionManager.Selected);
        commandManager.Execute(cmd);

        selectionManager.Clear();
    }
    
    private void OnCutClick()
    {
        if (selectionManager.Selected.Count == 0)
            return;

        // Tworzymy komendę przekazując kopię listy zaznaczonych elementów
        var cmd = new CutCommand(selectionManager.Selected);
    
        // Wykonujemy przez CommandManager, aby trafiła na stos Undo
        commandManager.Execute(cmd);

        // Po wycięciu obiekty znikają, więc czyścimy zaznaczenie
        selectionManager.Clear();
    }
}