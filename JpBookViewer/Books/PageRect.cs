using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class PageRect
{
    public PageRect(SKRect rect, string kanji)
    {
        Rect = rect;
        Kanji = kanji;
    }

    public SKRect Rect { get; }

    public string Kanji { get; }
}
