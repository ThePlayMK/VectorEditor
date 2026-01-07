using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Net;

namespace VectorEditor.UI.Render;

public static class GridRenderer
{
    public static void Render(Canvas canvas, EditorGrid grid)
    {
        if (!grid.IsVisible)
        {
            canvas.Background = Brushes.Transparent;
            return;
        }

        double size = grid.CellSize;

        // 1. Rysunek pojedynczej kratki
        var geometryDrawing = new GeometryDrawing
        {
            Brush = Brushes.Transparent,
            Pen = new Pen(Brushes.LightGray, 1),
            Geometry = new RectangleGeometry(new Rect(0, 0, size, size))
        };

        // 2. Pędzel powtarzający (DrawingBrush)
        var drawingBrush = new DrawingBrush
        {
            Drawing = geometryDrawing,
            TileMode = TileMode.Tile,
            
            // --- POPRAWKA DLA AVALONIA 11 ---
            // Zamiast Viewport i ViewportUnits używamy DestinationRect.
            // RelativeUnit.Absolute oznacza, że podajemy wymiary w pikselach (nie w procentach).
            DestinationRect = new RelativeRect(0, 0, size, size, RelativeUnit.Absolute),
            // --------------------------------
            
            Stretch = Stretch.None
        };

        canvas.Background = drawingBrush;
    }
}