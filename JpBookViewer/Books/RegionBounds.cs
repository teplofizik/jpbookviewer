using SkiaSharp;

namespace JpBookViewer.Books;

public readonly record struct RegionBounds(SKRect Rect, bool Normalized)
{
    public static RegionBounds Pixels(SKRect rect) => new(rect, false);

    public static RegionBounds FromNormalized(SKRect rect) => new(rect, true);

    public SKRect ToPixels(int imageWidth, int imageHeight)
    {
        if (!Normalized)
            return Rect;

        return new SKRect(
            Rect.Left * imageWidth,
            Rect.Top * imageHeight,
            Rect.Right * imageWidth,
            Rect.Bottom * imageHeight);
    }
}
