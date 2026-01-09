using System;
using System.Threading.Tasks;

namespace VectorEditor.Core.State;

public class EditorContext
{
    // Aktualny stan
    public IEditorState CurrentState { get; private set; }

    // Ścieżka do aktualnego pliku (może być null, jeśli to nowy projekt)
    public string? CurrentFilePath { get; set; }

    // Delegat/Akcja do wywoływania zapisu fizycznego (wstrzykniemy tu logikę z UI)
    // Func<string?, Task<bool>>: przyjmuje ścieżkę (opcjonalną), zwraca czy się udało
    public Func<string?, Task<bool>> SaveAction { get; set; } = _ => Task.FromResult(false);
    public EditorContext()
    {
        // Stan początkowy: Pusty (nowy projekt)
        ChangeState(new EmptyState());
    }

    public void ChangeState(IEditorState newState)
    {
        CurrentState = newState;
        // Tu możesz np. odświeżyć tytuł okna (dodać gwiazdkę *)
        //System.Diagnostics.Debug.WriteLine($"[STATE] Zmiana stanu na: {newState.GetType().Name}");
    }

    // Metody wywoływane przez UI / CommandManager
    public void Save() 
    {
        //System.Diagnostics.Debug.WriteLine($"[STATE] Próba zapisu w stanie: {CurrentState.GetType().Name}");
        CurrentState.Save(this);
    }

    public void Modify()
    {
        // To woła CommandManager
        CurrentState.Modify(this);
    }
}