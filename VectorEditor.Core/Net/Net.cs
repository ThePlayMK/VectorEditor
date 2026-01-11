using VectorEditor.Core.Structures; // Upewnij się, że to pasuje do Twojego Point

namespace VectorEditor.Core.Net
{
    public class EditorGrid(double cellSize = 20.0)
    {
        public double CellSize { get; } = cellSize;

        // TU BYŁ BŁĄD: Musi się nazywać IsVisible, żeby pasowało do Controllera
        public bool IsVisible { get; set; } = false;

        // TU BYŁ BŁĄD: Musi się nazywać SnapEnabled
        public bool SnapEnabled { get; set; } = false;

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