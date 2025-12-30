using VectorEditor.Core.Composite;

namespace VectorEditor.Core.Strategy;

public interface IModificationStrategy
{
    // Zwraca obiekt reprezentujący stan przed modyfikacją
    object? Apply(ICanvas target);
    // Przywraca stan na podstawie zapisanego obiektu
    void Undo(ICanvas target, object? memento);
}