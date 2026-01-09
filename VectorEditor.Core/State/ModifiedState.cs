namespace VectorEditor.Core.State;

public class ModifiedState : IEditorState
{
    public async void Save(EditorContext context)
    {
        // Jeśli mamy już ścieżkę -> szybki zapis.
        // Jeśli nie mamy (CurrentFilePath == null) -> Save As.
        bool success = await context.SaveAction(context.CurrentFilePath);

        if (success)
        {
            // Po udanym zapisie przechodzimy w stan Saved
            context.ChangeState(new SavedState());
        }
    }

    public void Modify(EditorContext context)
    {
        // Jesteśmy już zmodyfikowani, kolejna zmiana nic nie zmienia w stanie.
        // Zostajemy tutaj.
    }

    public string GetStatus() => "Niezapisany*";
}