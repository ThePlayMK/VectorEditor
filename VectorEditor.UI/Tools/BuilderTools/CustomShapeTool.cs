using System.Collections.Generic;
using System.Linq;
using System;
using Avalonia; // Do Avalonia.Point
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using VectorEditor.Core.Builder;
using VectorEditor.Core.Command;
using VectorEditor.UI.Tools.BuilderTools;
using VectorEditor.UI.UIControllers;

// Alias dla Twojej struktury
using CorePoint = VectorEditor.Core.Structures.Point;

namespace VectorEditor.UI.Tools.BuilderTools;

public class CustomShapeTool : ITool
{
    private CustomShapeBuilder _builder = new();
    private readonly List<CorePoint> _points = new();
    
    private Polyline? _previewPolyline;
    private const double PreviewOpacity = 0.5;
    
    // Ustawienia "Magnesu"
    private const double CloseThreshold = 15.0; // Jak blisko (w pikselach) trzeba być, żeby zamknąć
    private bool _isHoveringStart = false;      // Czy myszka jest nad punktem startowym?

    public void PointerPressed(MainWindow window, ToolController controller, PointerPressedEventArgs e)
    {
    // 1. Obsługa podwójnego kliknięcia (koniec rysowania)
        if (e.ClickCount == 2 && _points.Count > 1)
        {
            Finish(window);
            return;
        }

    // 2. Pobierz punkt z siatki
        var snappedPoint = controller.GetSnappedPoint(e, window.CanvasCanvas);
        var newPoint = new CorePoint(snappedPoint.X, snappedPoint.Y);

        // 3. Sprawdź Magnes (zamykanie kształtu)
        if (_points.Count > 2 && _isHoveringStart)
        {
            Finish(window);
            return;
        }

    // 4. BEZPIECZNIK (Try-Catch) - To naprawia crash!
        try
        {
        // Najpierw próbujemy dodać punkt do logiki (Buildera)
            _builder.AddPoint(newPoint);
        }
        catch (Exception)
        {
        // Jeśli Builder rzucił błąd (np. "Nieprawidłowa geometria", "Przecięcie linii"),
        // to po prostu IGNORUJEMY to kliknięcie.
        // Możesz tu dodać np. System.Media.SystemSounds.Beep.Play(); żeby dać znać użytkownikowi.
            return; 
        }      

    // 5. Dopiero jeśli Builder nie zgłosił sprzeciwu, dodajemy punkt do listy wizualnej
        _points.Add(newPoint);

    // Aktualizujemy podgląd
        UpdatePreview(window, e.GetPosition(window.CanvasCanvas));
    }

    public void PointerMoved(MainWindow window, ToolController controller, PointerEventArgs e)
    {
        if (_points.Count == 0) return;

        // Pobieramy pozycję do podglądu (tu też używamy siatki dla spójności)
        var snappedCurrent = controller.GetSnappedPoint(e, window.CanvasCanvas);
        
        // --- LOGIKA MAGNESU (Smart Closing) ---
        var start = _points[0];
        
        // Obliczamy dystans (w pikselach ekranowych) między kursorem a startem
        // Używamy surowej pozycji myszy do sprawdzania odległości, żeby było płynniej
        var rawPos = e.GetPosition(window.CanvasCanvas);
        double dist = Math.Sqrt(Math.Pow(rawPos.X - start.X, 2) + Math.Pow(rawPos.Y - start.Y, 2));

        if (_points.Count > 2 && dist < CloseThreshold)
        {
            // JESTEŚMY BLISKO STARTU -> "Przyklej" podgląd do startu
            _isHoveringStart = true;
            
            // Wizualna sztuczka: podgląd "skacze" do punktu startowego
            var magnetPoint = new CorePoint(start.X, start.Y);
            UpdatePreview(window, magnetPoint);
            
            // Opcjonalnie: Zmień kursor na "Hand", żeby zasygnalizować zamknięcie
            window.Cursor = new Cursor(StandardCursorType.Hand);
        }
        else
        {
            // NORMALNY RUCH
            _isHoveringStart = false;
            
            var hoverPoint = new CorePoint(snappedCurrent.X, snappedCurrent.Y);
            UpdatePreview(window, hoverPoint);
            
            window.Cursor = Cursor.Default;
        }
    }

    private void UpdatePreview(MainWindow window, CorePoint hoverPoint) // Przeciążenie dla CorePoint
    {
        UpdatePreview(window, new Avalonia.Point(hoverPoint.X, hoverPoint.Y));
    }

    private void UpdatePreview(MainWindow window, Avalonia.Point hoverPoint)
    {
        if (_previewPolyline == null)
        {
            _previewPolyline = new Polyline
            {
                Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100),
                StrokeThickness = window.Settings.StrokeWidth,
                Fill = Brushes.Transparent, // Na razie bez wypełnienia, żeby nie zasłaniać
                IsHitTestVisible = false
            };
            window.CanvasCanvas.Children.Add(_previewPolyline);
        }

        var pts = new Avalonia.Collections.AvaloniaList<Avalonia.Point>();
        
        // Dodaj wszystkie zatwierdzone punkty
        foreach (var p in _points)
        {
            pts.Add(new Avalonia.Point(p.X, p.Y));
        }

        // Dodaj punkt "latający" (kursora)
        pts.Add(hoverPoint);

        _previewPolyline.Points = pts;
        
        // Opcjonalnie: Zmień kolor linii na zielony, jeśli zamykamy
        if (_isHoveringStart)
        {
            _previewPolyline.Stroke = Brushes.Green; // Sygnał dla użytkownika: "Tu zamkniesz"
            _previewPolyline.StrokeThickness = window.Settings.StrokeWidth * 1.5;
        }
        else
        {
            _previewPolyline.Stroke = new SolidColorBrush(window.Settings.ContourColor, window.Settings.Opacity * PreviewOpacity / 100);
            _previewPolyline.StrokeThickness = window.Settings.StrokeWidth;
        }
    }

    public void PointerReleased(MainWindow window, ToolController controller, PointerReleasedEventArgs e) { }

    private void Finish(MainWindow window)
    {
        // Sprzątanie podglądu
        if (_previewPolyline != null)
        {
            window.CanvasCanvas.Children.Remove(_previewPolyline);
            _previewPolyline = null;
        }
        window.Cursor = Cursor.Default;

        // Jeśli mamy za mało punktów -> anuluj
        if (_points.Count < 2)
        {
            ResetTool();
            return;
        }

        // Konfiguracja buildera (punkty były dodawane na bieżąco w PointerPressed)
        _builder
            .SetContentColor(window.Settings.ContentColor)
            .SetContourColor(window.Settings.ContourColor)
            .SetOpacity(window.Settings.Opacity / 100)
            .SetWidth(window.Settings.StrokeWidth);
        
        // Wykonanie komendy
        var cmd = new AddShapeCommand(_builder, window.SelectedLayerModel);
        window.CommandManager.Execute(cmd);

        ResetTool();
    }

    private void ResetTool()
    {
        _points.Clear();
        _isHoveringStart = false;
        _builder = new CustomShapeBuilder();
    }
}