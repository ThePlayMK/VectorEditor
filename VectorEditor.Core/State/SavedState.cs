namespace VectorEditor.Core.State;

public class SavedState : IEditorState
{
    public async void Save(EditorContext context)
    {
        //testy
        //System.Diagnostics.Debug.WriteLine("[STATE] Wymuszam zapis w SavedState.");

        // Wywołujemy akcję zapisu (tę samą co w ModifiedState)
        await context.SaveAction(context.CurrentFilePath);
    }

    public void Modify(EditorContext context)
    {
        // Każda, nawet najmniejsza zmiana powoduje wyjście ze stanu Saved
        context.ChangeState(new ModifiedState());
    }

    public string GetStatus() => "Zapisany";
}