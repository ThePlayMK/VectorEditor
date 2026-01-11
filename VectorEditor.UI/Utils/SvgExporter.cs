using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures; 

namespace VectorEditor.UI.Utils;

public static class SvgExporter
{
    private static string F(double d) => d.ToString("0.##", CultureInfo.InvariantCulture);
    private static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    private static string ToOpacity(double op) => F(op);

    public static string GenerateSvg(IEnumerable<IShape> shapes, double width, double height)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"<svg width=\"{width}\" height=\"{height}\" xmlns=\"http://www.w3.org/2000/svg\">");
        sb.AppendLine($"<rect width=\"100%\" height=\"100%\" fill=\"white\" />");

        foreach (var shape in shapes)
        {
            // --- LINIA ---
            if (shape is Line line) 
            {
                var start = line.GetStartPoint();
                var end = line.GetEndPoint();
                
                sb.AppendLine($"<line x1=\"{F(start.X)}\" y1=\"{F(start.Y)}\" " +
                              $"x2=\"{F(end.X)}\" y2=\"{F(end.Y)}\" " +
                              $"stroke=\"{ToHex(line.GetContourColor())}\" " +
                              $"stroke-width=\"{F(line.GetWidth())}\" " +
                              $"opacity=\"{ToOpacity(line.GetOpacity())}\" />");
            }
            
            // --- KOŁO / ELIPSA ---
            else if (shape is Circle circ)
            {
                var center = circ.GetCenterPoint();
                var rx = circ.GetRadiusX();
                var ry = circ.GetRadiusY();
                
                // Sprawdzamy, czy to idealne koło
                if (Math.Abs(rx - ry) < 0.001)
                {
                    sb.AppendLine($"<circle cx=\"{F(center.X)}\" cy=\"{F(center.Y)}\" " +
                                  $"r=\"{F(rx)}\" " +
                                  $"stroke=\"{ToHex(circ.GetContourColor())}\" " +
                                  $"stroke-width=\"{F(circ.GetWidth())}\" " +
                                  $"fill=\"{ToHex(circ.GetContentColor())}\" " +
                                  $"opacity=\"{ToOpacity(circ.GetOpacity())}\" />");
                }
                else
                {
                    sb.AppendLine($"<ellipse cx=\"{F(center.X)}\" cy=\"{F(center.Y)}\" " +
                                  $"rx=\"{F(rx)}\" ry=\"{F(ry)}\" " +
                                  $"stroke=\"{ToHex(circ.GetContourColor())}\" " +
                                  $"stroke-width=\"{F(circ.GetWidth())}\" " +
                                  $"fill=\"{ToHex(circ.GetContentColor())}\" " +
                                  $"opacity=\"{ToOpacity(circ.GetOpacity())}\" />");
                }
            }
            
            // --- WIELOKĄTY (Triangle, CustomShape, Rectangle) ---
            // Używamy metody GetPoints(), która zwraca listę wierzchołków.
            // To zadziała dla Trójkąta, Prostokąta i Wielokąta.
            else
            {
                // Próba pobrania punktów
                var points = shape.GetPoints()?.ToList();

                if (points != null && points.Count >= 2)
                {
                    // Budujemy ciąg punktów "x1,y1 x2,y2 ..."
                    var ptsString = string.Join(" ", points.Select(p => $"{F(p.X)},{F(p.Y)}"));

                    // Używamy <polygon> dla zamkniętych kształtów
                    // Jeśli to Rectangle, Triangle lub CustomShape, <polygon> jest idealny.
                    sb.AppendLine($"<polygon points=\"{ptsString}\" " +
                                  $"stroke=\"{ToHex(shape.GetContourColor())}\" " +
                                  $"stroke-width=\"{F(shape.GetWidth())}\" " +
                                  $"fill=\"{ToHex(shape.GetContentColor())}\" " +
                                  $"opacity=\"{ToOpacity(shape.GetOpacity())}\" />");
                }
            }
        }

        sb.AppendLine("</svg>");
        return sb.ToString();
    }
}