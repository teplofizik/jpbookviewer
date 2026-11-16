using System.Globalization;
using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class Book
{
    public List<Page> Pages { get; } = new();

    public string Name { get; private set; } = "";

    public string Directory { get; private set; } = "";

    public void Load(string directory)
    {
        Pages.Clear();

        var source = new CodhRois(directory);
        Directory = directory;
        Name = source.Name;

        foreach (var image in source.Images)
            Pages.Add(new Page(image));

        foreach (var rectangle in source.Rectangles)
        {
            var pageIndex = source.GetPageId(rectangle.Image);
            if (pageIndex >= 0 && pageIndex < Pages.Count)
                Pages[pageIndex].Rects.Add(new PageRect(rectangle.Rect, rectangle.CharView));
        }
    }

    public void ProcessToWeb()
    {
        if (Pages.Count == 0)
            return;

        var webDirectory = Path.Combine(Directory, "web");
        var dataDirectory = Path.Combine(webDirectory, "data");
        var imagesDirectory = Path.Combine(webDirectory, "images");

        System.IO.Directory.CreateDirectory(dataDirectory);
        System.IO.Directory.CreateDirectory(imagesDirectory);

        var lines = new List<string>();

        foreach (var page in Pages)
        {
            using var bitmap = SKBitmap.Decode(page.Image);
            if (bitmap == null || bitmap.Width == 0)
                continue;

            var width = 1000;
            var height = Math.Max(1, (int)Math.Round(bitmap.Height * (width / (double)bitmap.Width)));
            var scale = width / (float)bitmap.Width;

            var fileName = Path.GetFileName(page.Image);
            lines.Add(string.Create(CultureInfo.InvariantCulture,
                $"{fileName}\t{page.Rects.Count}\t{width}\t{height}"));

            SaveImageRects(page, Path.Combine(dataDirectory, Path.GetFileNameWithoutExtension(fileName) + ".txt"), scale);
            SaveImage(bitmap, width, height, Path.Combine(imagesDirectory, fileName));
        }

        File.WriteAllLines(Path.Combine(webDirectory, "list.txt"), lines);
    }

    private static void SaveImageRects(Page page, string fileName, float scale)
    {
        var lines = page.Rects.Select(rectangle => string.Create(CultureInfo.InvariantCulture,
            $"{rectangle.Rect.Left * scale:0}\t{rectangle.Rect.Top * scale:0}\t{rectangle.Rect.Width * scale:0}\t{rectangle.Rect.Height * scale:0}\t{rectangle.Kanji}"));

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
}
