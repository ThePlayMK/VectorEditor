using System;
using System.Collections.Generic;
using System.Linq;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures;

namespace VectorEditor.UI.Select;

public class SelectionManager
{
    private readonly List<ICanvas> _currentSelection = [];

    public IReadOnlyList<ICanvas> Selected => _currentSelection;
    public event Action? OnChanged;

    public void Clear()
    {
        if (_currentSelection.Count == 0)
            return;

        _currentSelection.Clear();
        OnChanged?.Invoke();
    }

    public void SelectSingle(ICanvas element)
    {
        if (!element.IsVisible)
        {
            return;
        }

        _currentSelection.Clear();
        _currentSelection.Add(element);
        OnChanged?.Invoke();
    }

    public void AddRange(IEnumerable<ICanvas> elements)
    {
        _currentSelection.AddRange(elements);
        OnChanged?.Invoke();
    }

    public void SelectArea(Layer targetLayer, Point p1, Point p2)
    {
        var topLeft = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
        var bottomRight = new Point(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));

        var foundElements = targetLayer.GetChildren()
            .Where(child => child.IsWithinBounds(topLeft, bottomRight))
            .ToList();

        Clear();
        AddRange(foundElements);
    }
}