using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace VectorEditor.UI.UIControllers;

public class CanvasController
{
    private bool _isPanning;
    private Control? _capturedControl;
    private Point _initialMousePosition;

    // -------------------------
    // ZOOM (scroll wheel)
    // -------------------------
    public static void OnPointerWheel(Canvas canvas, PointerWheelEventArgs e)
    {
        if (canvas.RenderTransform is not MatrixTransform transform)
            return;

        var matrix = transform.Matrix;
        var scaleFactor = e.Delta.Y > 0 ? 1.1 : 0.9;
        var point = e.GetPosition(canvas);

        matrix = MatrixHelper.ScaleAt(matrix, scaleFactor, scaleFactor, point.X, point.Y);
        transform.Matrix = matrix;

        e.Handled = true;
    }

    // -------------------------
    // PAN START (Hand tool)
    // -------------------------
    public void OnPointerPressed(Control sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(sender).Properties;

        if (!properties.IsLeftButtonPressed)
            return;

        _isPanning = true;
        _initialMousePosition = e.GetPosition(sender);
        e.Pointer.Capture(sender);
        _capturedControl = sender;
    }

    // -------------------------
    // PAN MOVE
    // -------------------------
    public void OnPointerMoved(Canvas canvas, Control sender, PointerEventArgs e)
    {
        if (!_isPanning)
            return;

        if (canvas.RenderTransform is not MatrixTransform transform)
            return;

        var currentPosition = e.GetPosition(sender);
        var offset = currentPosition - _initialMousePosition;

        var matrix = transform.Matrix;
        matrix = MatrixHelper.Translate(matrix, offset.X, offset.Y);
        transform.Matrix = matrix;

        _initialMousePosition = currentPosition;
    }

    // -------------------------
    // PAN END
    // -------------------------
    public void OnPointerReleased(PointerReleasedEventArgs e)
    {
        _isPanning = false;

        if (_capturedControl == null)
        {
            return;
        }
        e.Pointer.Capture(null);
        _capturedControl = null;
    }

    // -------------------------
    // CENTER CANVAS
    // -------------------------
    public static void CenterCanvas(Canvas canvas)
    {
        if (canvas.RenderTransform is not MatrixTransform transform)
            return;

        if (canvas.Parent is not Control container)
            return;

        var canvasBounds = canvas.Bounds;
        var containerBounds = container.Bounds;

        var targetX = (containerBounds.Width - canvasBounds.Width) / 2;
        var targetY = (containerBounds.Height - canvasBounds.Height) / 2;

        var offsetX = targetX - canvasBounds.X;
        var offsetY = targetY - canvasBounds.Y;

        transform.Matrix = Matrix.CreateTranslation(offsetX, offsetY);
    }
}