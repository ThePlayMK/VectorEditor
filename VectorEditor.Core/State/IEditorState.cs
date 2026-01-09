namespace VectorEditor.Core.State;

public interface IEditorState
{
    // Wywoływane, gdy użytkownik klika "Save"
    void Save(EditorContext context);

    // Wywoływane, gdy użytkownik coś narysuje/zmieni
    void Modify(EditorContext context);
    
    // Opcjonalnie: Zwraca nazwę stanu dla paska tytułu (np. "Projekt*")
    string GetStatus();
}