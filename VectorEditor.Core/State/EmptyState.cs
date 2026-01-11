namespace VectorEditor.Core.State;

public class EmptyState : IEditorState
{
    public async void Save(EditorContext context)
    {
        // Pusty projekt -> Save działa jak "Save As"
        // Przekazujemy null jako ścieżkę, co wymusi otwarcie okna dialogowego w SaveAction
        var success = await context.SaveAction(null);

        if (success)
        {
            context.ChangeState(new SavedState());
        }
    }

    public void Modify(EditorContext context)
    {
        // Jeśli coś narysujemy na pustym, staje się "Zmodyfikowany"
        context.ChangeState(new ModifiedState());
    }

    public string GetStatus() => "Nowy";
}