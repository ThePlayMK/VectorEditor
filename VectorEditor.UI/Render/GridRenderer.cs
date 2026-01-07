using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using VectorEditor.Core.Net;

namespace VectorEditor.UI.Render;

public static class GridRenderer
{
    public static void Render(Canvas canvas, EditorGrid grid)
    {
        // Jeśli siatka wyłączona -> tło Canvasa jest przezroczyste (widać biały Border pod spodem)
        if (!grid.IsVisible)
        {
            canvas.Background = Brushes.Transparent;
            return;
        }

        double size = grid.CellSize;

        var geometryDrawing = new GeometryDrawing
        {

            Brush = Brushes.Transparent, 

            
            Pen = new Pen(Brushes.LightGray, 1),
            Geometry = new RectangleGeometry(new Rect(0, 0, size, size))
        };

        var drawingBrush = new DrawingBrush
        {
            Drawing = geometryDrawing,
            TileMode = TileMode.Tile,
            DestinationRect = new RelativeRect(0, 0, size, size, RelativeUnit.Absolute),
            Stretch = Stretch.None
        };

        canvas.Background = drawingBrush;
    }
}