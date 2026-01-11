using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia.Media;
using VectorEditor.Core.Composite;
using VectorEditor.Core.Structures; // Tu są Twoje kształty (Line, Circle, Rectangle)

namespace VectorEditor.UI.Utils;

public static class SvgExporter
{
    private static string F(double d) => d.ToString("0.##", CultureInfo.InvariantCulture);
    
    // Helper: Jeśli kolor jest w pełni przezroczysty -> "none", inaczej HEX
    private static string ToFill(Color c) => c.A == 0 ? "none" : $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    
    // Helper: Obrys zawsze HEX (przezroczystość sterowana przez opacity)
    private static string ToStroke(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    
    private static string ToOpacity(double op) => F(op);

    public static string GenerateSvg(IEnumerable<ICanvas> shapes, double width, double height)
    {
        var sb = new StringBuilder();
        
        // Zabezpieczenie przed zerowymi wymiarami
        if (width <= 0) width = 800;
        if (height <= 0) height = 600;

        sb.AppendLine($"<svg width=\"{F(width)}\" height=\"{F(height)}\" viewBox=\"0 0 {F(width)} {F(height)}\" xmlns=\"http://www.w3.org/2000/svg\">");
        
        // Opcjonalne białe tło (jeśli chcesz przezroczyste, usuń tę linię)
        sb.AppendLine($"<rect width=\"100%\" height=\"100%\" fill=\"white\" />");

        foreach (var shape in shapes)
        {
            if (shape is Layer layer)
            {
                GenerateSvg(layer.GetChildren(), width, height);
            }
            // --- LINIA ---
            else if (shape is Line line) 
            {
                var s = line.GetStartPoint();
                var e = line.GetEndPoint();
                
                sb.AppendLine($"<line x1=\"{F(s.X)}\" y1=\"{F(s.Y)}\" x2=\"{F(e.X)}\" y2=\"{F(e.Y)}\" " +
                              $"stroke=\"{ToStroke(line.GetContourColor())}\" " +
                              $"stroke-width=\"{F(line.GetWidth())}\" " +
                              $"opacity=\"{ToOpacity(line.GetOpacity())}\" />");
            }
            
            // --- PROSTOKĄT (Poprawione!) ---
            // Musimy obliczyć lewy górny róg i wymiary na podstawie przekątnej
            else if (shape is Rectangle rect)
            {
                var s = rect.GetStartPoint();
                var e = rect.GetOppositePoint(); // Używamy Twojej metody
                
                double x = Math.Min(s.X, e.X);
                double y = Math.Min(s.Y, e.Y);
                double w = Math.Abs(e.X - s.X);
                double h = Math.Abs(e.Y - s.Y);

                sb.AppendLine($"<rect x=\"{F(x)}\" y=\"{F(y)}\" width=\"{F(w)}\" height=\"{F(h)}\" " +
                              $"stroke=\"{ToStroke(rect.GetContourColor())}\" " +
                              $"stroke-width=\"{F(rect.GetWidth())}\" " +
                              $"fill=\"{ToFill(rect.GetContentColor())}\" " +
                              $"opacity=\"{ToOpacity(rect.GetOpacity())}\" />");
            }
            
            // --- KOŁO / ELIPSA ---
            else if (shape is Circle circ)
            {
                var c = circ.GetCenterPoint();
                var rx = circ.GetRadiusX();
                var ry = circ.GetRadiusY();
                
                sb.AppendLine($"<ellipse cx=\"{F(c.X)}\" cy=\"{F(c.Y)}\" rx=\"{F(rx)}\" ry=\"{F(ry)}\" " +
                              $"stroke=\"{ToStroke(circ.GetContourColor())}\" " +
                              $"stroke-width=\"{F(circ.GetWidth())}\" " +
                              $"fill=\"{ToFill(circ.GetContentColor())}\" " +
                              $"opacity=\"{ToOpacity(circ.GetOpacity())}\" />");
            }
            
            // --- INNE WIELOKĄTY (Triangle, CustomShape) ---
            // Dla tych figur GetPoints() powinno zwracać wszystkie wierzchołki (3 lub więcej)
            else if (shape is Triangle triangle)
            {
                var points = shape.GetPoints()?.ToList();
                if (points != null && points.Count >= 2)
                {
                    var ptsString = string.Join(" ", points.Select(p => $"{F(p.X)},{F(p.Y)}"));

                    sb.AppendLine($"<polygon points=\"{ptsString}\" " +
                                  $"stroke=\"{ToStroke(triangle.GetContourColor())}\" " +
                                  $"stroke-width=\"{F(triangle.GetWidth())}\" " +
                                  $"fill=\"{ToFill(triangle.GetContentColor())}\" " +
                                  $"opacity=\"{ToOpacity(triangle.GetOpacity())}\" />");
                }
            }
            else if (shape is CustomShape customShape)
            {
                var points = shape.GetPoints()?.ToList();
                if (points != null && points.Count >= 2)
                {
                    var ptsString = string.Join(" ", points.Select(p => $"{F(p.X)},{F(p.Y)}"));

                    sb.AppendLine($"<polygon points=\"{ptsString}\" " +
                                  $"stroke=\"{ToStroke(customShape.GetContourColor())}\" " +
                                  $"stroke-width=\"{F(customShape.GetWidth())}\" " +
                                  $"fill=\"{ToFill(customShape.GetContentColor())}\" " +
                                  $"opacity=\"{ToOpacity(customShape.GetOpacity())}\" />");
                }
            }
        }

        sb.AppendLine("</svg>");
        return sb.ToString();
    }
}