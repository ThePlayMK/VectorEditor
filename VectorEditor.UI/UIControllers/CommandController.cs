using System;
using Avalonia.Input;
using VectorEditor.Core.Command;
using VectorEditor.Core.Command.Select;
using VectorEditor.Core.Composite;

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
                commandManager.Undo();
                return;
            // CTRL + Y → Redo
            case KeyModifiers.Control when e.Key == Key.Y:
                commandManager.Redo();
                return;
            // CTRL + C → Copy
            case KeyModifiers.Control when e.Key == Key.C:
            {
                if (selectionManager.Selected.Count == 0)
                    return;

                var copy = new CopyCommand(selectionManager.Selected);
                copy.Execute(); 
                return;
            }
            // CTRL + V → Paste
            case KeyModifiers.Control when e.Key == Key.V:
            {
                var cmd = new PasteCommand(getSelectedLayer(), selectionManager);
                commandManager.Execute(cmd);
                return;
            }
            default:
                return;
        }
    }

    public void Undo() => commandManager.Undo();
    public void Redo() => commandManager.Redo();
}


/*
        private void Undo_Click(object? sender, RoutedEventArgs e) => _commandController.Undo();

        private void Redo_Click(object? sender, RoutedEventArgs e) => _commandController.Redo();
*/