using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class StoragePageImage : IPageImage
{
    private readonly IReadOnlyStorage _storage;
    private readonly string _path;

    public StoragePageImage(IReadOnlyStorage storage, string path)
    {
        _storage = storage;
        _path = path;
    }

    public string Name => StoragePaths.GetFileName(_path);

    public SKBitmap? Load()
    {
        try
        {
            using var stream = _storage.OpenRead(_path);
            return SKBitmap.Decode(stream);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
