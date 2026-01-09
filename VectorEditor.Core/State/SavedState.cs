namespace VectorEditor.Core.State;

public class SavedState : IEditorState
{
    public void Save(EditorContext context)
    {
        //nic nie robi
    }

    public void Modify(EditorContext context)
    {
        // Każda, nawet najmniejsza zmiana powoduje wyjście ze stanu Saved
        context.ChangeState(new ModifiedState());
    }

    public string GetStatus() => "Zapisany";
}