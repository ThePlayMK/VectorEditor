using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using VectorEditor.Core.Composite;

namespace VectorEditor.UI.Utils;

public class ProjectExporter(Canvas canvas)
{
    public async Task ExportToSvg(IEnumerable<IShape> shapes, IStorageFile file)
    {
        try
        {
            var w = canvas.Bounds.Width > 0 ? canvas.Bounds.Width : 800;
            var h = canvas.Bounds.Height > 0 ? canvas.Bounds.Height : 600;

            var svgContent = SvgExporter.GenerateSvg(shapes, w, h);

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(svgContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXPORT BŁĄD]: {ex.Message}");
        }
    }

    public async Task ExportToRaster(IStorageFile file, Action<bool> toggleGrid)
    {
        // 1. Przygotowanie (ukrycie siatki przez callback)
        var originalTransform = canvas.RenderTransform;
        toggleGrid(false);

        double finalWidth = double.IsNaN(canvas.Width) ? canvas.Bounds.Width : canvas.Width;
        double finalHeight = double.IsNaN(canvas.Height) ? canvas.Bounds.Height : canvas.Height;
        if (finalWidth <= 0) finalWidth = 800;
        if (finalHeight <= 0) finalHeight = 600;

        try
        {
            canvas.RenderTransform = null;
            canvas.UpdateLayout();

            var pixelSize = new PixelSize((int)finalWidth, (int)finalHeight);
            var bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
            bitmap.Render(canvas);

            await using var stream = await file.OpenWriteAsync();
            bitmap.Save(stream);
        }
        finally
        {
            // Przywracanie stanu
            canvas.RenderTransform = originalTransform;
            toggleGrid(true);
            canvas.UpdateLayout();
        }
    }

    public IEnumerable<IShape> GetAllShapes(Layer rootLayer)
    {
        var allShapes = new List<IShape>();
        foreach (var child in rootLayer.GetChildren())
        {
            if (child is Layer childLayer)
            {
                if (childLayer.IsVisible)
                    allShapes.AddRange(GetAllShapes(childLayer));
            }
            else if (child is IShape shape)
            {
                allShapes.Add(shape);
            }
        }

        return allShapes;
    }
}