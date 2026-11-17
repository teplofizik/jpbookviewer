using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class FilePageImage : IPageImage
{
    private readonly string _path;

    public FilePageImage(string path)
    {
        _path = path;
    }

    public string Name => Path.GetFileName(_path);

    public SKBitmap? Load()
    {
        try
        {
            return File.Exists(_path) ? SKBitmap.Decode(File.ReadAllBytes(_path)) : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
