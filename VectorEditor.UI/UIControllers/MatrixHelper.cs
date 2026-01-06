using Avalonia;

namespace VectorEditor.UI.UIControllers;

public static class MatrixHelper
{
    public static Matrix ScaleAt(Matrix matrix, double scaleX, double scaleY, double centerX, double centerY)
    {
        return matrix * Matrix.CreateTranslation(-centerX, -centerY)
                      * Matrix.CreateScale(scaleX, scaleY)
                      * Matrix.CreateTranslation(centerX, centerY);
    }

    public static Matrix Translate(Matrix matrix, double offsetX, double offsetY)
    {
        return matrix * Matrix.CreateTranslation(offsetX, offsetY);
    }
}