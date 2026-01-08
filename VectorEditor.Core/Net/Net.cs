using System;
using VectorEditor.Core.Structures; // Upewnij się, że to pasuje do Twojego Point

namespace VectorEditor.Core.Net
{
    public class EditorGrid
    {
        public double CellSize { get; set; } = 20.0;

        // TU BYŁ BŁĄD: Musi się nazywać IsVisible, żeby pasowało do Controllera
        public bool IsVisible { get; set; } = false;

        // TU BYŁ BŁĄD: Musi się nazywać SnapEnabled
        public bool SnapEnabled { get; set; } = false;

        public EditorGrid(double cellSize = 20.0)
        {
            CellSize = cellSize;
        }

        public Point Snap(Point inputPoint)
        {
            if (!SnapEnabled || !IsVisible)
            {
                return inputPoint;
            }

            double snappedX = Math.Round(inputPoint.X / CellSize) * CellSize;
            double snappedY = Math.Round(inputPoint.Y / CellSize) * CellSize;

            return new Point(snappedX, snappedY);
        }
    }
}