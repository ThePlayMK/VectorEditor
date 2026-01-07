using System;
using VectorEditor.Core.Structures; 

namespace VectorEditor.Core.Net
{
    public class EditorGrid
    {
        // Rozmiar pojedynczej kratki (np. 20 pikseli)
        public double CellSize { get; set; } = 20.0;

        // Czy siatka jest widoczna i aktywna
        public bool IsEnabled { get; set; } = false;

        // Czy przyciąganie jest włączone (można widzieć siatkę, ale nie przyciągać)
        public bool SnapToGridEnabled { get; set; } = false;

        public EditorGrid(double cellSize = 20.0)
        {
            CellSize = cellSize;
        }

        /// <summary>
        /// Główna logika: bierze punkt i zwraca nowy punkt "przyciągnięty" do siatki
        /// </summary>
        public Point Snap(Point inputPoint)
        {
            if (!SnapToGridEnabled || !IsEnabled)
            {
                // Jeśli siatka wyłączona, zwracamy oryginał
                return inputPoint;
            }

            // Matematyka zaokrąglania do najbliższej wielokrotności CellSize
            double snappedX = Math.Round(inputPoint.X / CellSize) * CellSize;
            double snappedY = Math.Round(inputPoint.Y / CellSize) * CellSize;

            return new Point(snappedX,snappedY);
        }
    }
}