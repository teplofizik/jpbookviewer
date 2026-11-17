using System.Globalization;
using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class Book : IDisposable
{
    private readonly IReadOnlyStorage _storage;
    private bool _disposed;

    public Book(string name, string source, IReadOnlyStorage storage, IReadOnlyList<BookPage> pages)
    {
        Name = name;
        Source = source;
        _storage = storage;
        Pages = pages;
    }

    public string Name { get; }

    public string Source { get; }

    public IReadOnlyList<BookPage> Pages { get; }

    public void ProcessToWeb()
    {
        ProcessToWeb(GetDefaultWebDirectory());
    }

    public void ProcessToWeb(string outputDirectory)
    {
        if (Pages.Count == 0)
            return;

        var dataDirectory = Path.Combine(outputDirectory, "data");
        var imagesDirectory = Path.Combine(outputDirectory, "images");

        Directory.CreateDirectory(dataDirectory);
        Directory.CreateDirectory(imagesDirectory);

        var lines = new List<string>();

        foreach (var page in Pages)
        {
            using var bitmap = page.Image.Load();
            if (bitmap == null || bitmap.Width == 0)
                continue;

            const int width = 1000;
            var height = Math.Max(1, (int)Math.Round(bitmap.Height * (width / (double)bitmap.Width)));
            var scale = width / (float)bitmap.Width;
            var fileName = page.Image.Name;

            lines.Add(string.Create(CultureInfo.InvariantCulture,
                $"{fileName}\t{page.Regions.Count}\t{width}\t{height}"));

            SaveImageRects(page, bitmap, Path.Combine(dataDirectory, Path.GetFileNameWithoutExtension(fileName) + ".txt"), scale);
            SaveImage(bitmap, width, height, Path.Combine(imagesDirectory, fileName));
        }

        File.WriteAllLines(Path.Combine(outputDirectory, "list.txt"), lines);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _storage.Dispose();
    }

    private string GetDefaultWebDirectory()
    {
        var root = _storage.LocalRoot
            ?? Path.Combine(Path.GetTempPath(), "CodhRoisReader-web", MakeSafeFileName(Name));

        return Path.Combine(root, "web");
    }

    private static void SaveImageRects(BookPage page, SKBitmap bitmap, string fileName, float scale)
    {
        var lines = new List<string>(page.Regions.Count);

        foreach (var region in page.Regions)
        {
            var rect = region.Bounds.ToPixels(bitmap.Width, bitmap.Height);
            lines.Add(string.Create(CultureInfo.InvariantCulture,
                $"{rect.Left * scale:0}\t{rect.Top * scale:0}\t{rect.Width * scale:0}\t{rect.Height * scale:0}\t{region.DisplayText}"));
        }

        File.WriteAllLines(fileName, lines);
    }

    private static void SaveImage(SKBitmap bitmap, int width, int height, string fileName)
    {
        var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);
        if (surface == null)
            return;

        surface.Canvas.Scale(width / (float)bitmap.Width);
        surface.Canvas.DrawBitmap(bitmap, 0, 0);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);
        using var stream = File.Create(fileName);
        data.SaveTo(stream);
    }

    private static string MakeSafeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var chars = name.Select(c => invalid.Contains(c) ? '_' : c).ToArray();
        var result = new string(chars).Trim();

        return result.Length == 0 ? "book" : result;
    }
}
