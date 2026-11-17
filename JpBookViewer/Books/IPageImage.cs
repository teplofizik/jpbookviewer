using SkiaSharp;

namespace JpBookViewer.Books;

public interface IPageImage
{
    string Name { get; }

    SKBitmap? Load();
}
