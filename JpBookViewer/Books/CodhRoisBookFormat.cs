using System.Globalization;
using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class CodhRoisBookFormat : IBookFormat
{
    private const string CoordinateSuffix = "_coordinate.csv";

    private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".bmp"];

    public string Id => "codhrois";

    public string DisplayName => "CODROIS coordinate CSV";

    public bool CanRead(IReadOnlyStorage storage)
    {
        var files = storage.EnumerateFiles().ToList();
        var coordinatePath = FindCoordinateFile(files, storage.DisplayName);
        if (coordinatePath == null)
            return false;

        var imagesPrefix = StoragePaths.Combine(StoragePaths.GetDirectory(coordinatePath), "images") + "/";
        return files.Any(path =>
            path.StartsWith(imagesPrefix, StringComparison.OrdinalIgnoreCase) &&
            ImageExtensions.Contains(StoragePaths.GetExtension(path), StringComparer.OrdinalIgnoreCase));
    }

    public Book Load(IReadOnlyStorage storage)
    {
        var files = storage.EnumerateFiles().ToList();
        var coordinatePath = FindCoordinateFile(files, storage.DisplayName)
            ?? throw new InvalidDataException("В книге не найден файл \"*_coordinate.csv\".");

        var coordinateName = StoragePaths.GetFileName(coordinatePath);
        var bookName = coordinateName.EndsWith(CoordinateSuffix, StringComparison.OrdinalIgnoreCase)
            ? coordinateName[..^CoordinateSuffix.Length]
            : storage.DisplayName;

        if (string.IsNullOrWhiteSpace(bookName))
            bookName = storage.DisplayName;

        var root = StoragePaths.GetDirectory(coordinatePath);
        var imagesPrefix = StoragePaths.Combine(root, "images") + "/";

        var imageFiles = files
            .Where(path => path.StartsWith(imagesPrefix, StringComparison.OrdinalIgnoreCase))
            .Where(path => ImageExtensions.Contains(StoragePaths.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .OrderBy(StoragePaths.GetFileName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (imageFiles.Count == 0)
            throw new InvalidDataException("В книге не найдено ни одного изображения.");

        var imagesIndex = new Dictionary<string, int>(imageFiles.Count, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < imageFiles.Count; i++)
            imagesIndex[StoragePaths.GetFileNameWithoutExtension(imageFiles[i])] = i;

        var regions = new List<AnnotationRegion>[imageFiles.Count];
        for (var i = 0; i < regions.Length; i++)
            regions[i] = new List<AnnotationRegion>();

        foreach (var rectangle in ReadRectangles(storage, coordinatePath))
        {
            var imageKey = StoragePaths.GetFileNameWithoutExtension(StoragePaths.Normalize(rectangle.Image));
            if (imagesIndex.TryGetValue(imageKey, out var pageIndex))
                regions[pageIndex].Add(rectangle.ToRegion());
        }

        var pages = new List<BookPage>(imageFiles.Count);
        for (var i = 0; i < imageFiles.Count; i++)
            pages.Add(new BookPage(new StoragePageImage(storage, imageFiles[i]), regions[i]));

        return new Book(bookName, storage.DisplayName, storage, pages);
    }

    private static string? FindCoordinateFile(IReadOnlyList<string> files, string displayName)
    {
        var candidates = files
            .Where(path => StoragePaths.GetFileName(path).EndsWith(CoordinateSuffix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (candidates.Count == 0)
            return null;

        var expected = $"{displayName}{CoordinateSuffix}";
        return candidates
            .OrderBy(path => string.Equals(StoragePaths.GetFileName(path), expected, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(path => path.Count(c => c == '/'))
            .ThenBy(path => path, StringComparer.OrdinalIgnoreCase)
            .First();
    }

    private static IEnumerable<CodhRoisRectangle> ReadRectangles(IReadOnlyStorage storage, string path)
    {
        using var stream = storage.OpenRead(path);
        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);

        var lineNumber = 0;
        while (reader.ReadLine() is { } line)
        {
            lineNumber++;
            if (lineNumber == 1)
                continue;

            var rectangle = CodhRoisRectangle.TryParse(line);
            if (rectangle != null)
                yield return rectangle;
        }
    }

    private sealed class CodhRoisRectangle
    {
        private CodhRoisRectangle(
            int unicode,
            string image,
            int x,
            int y,
            string blockId,
            string charId,
            int width,
            int height)
        {
            Unicode = unicode;
            Image = image;
            X = x;
            Y = y;
            BlockId = blockId;
            CharId = charId;
            Width = width;
            Height = height;
        }

        public int Unicode { get; }

        public string Image { get; }

        public int X { get; }

        public int Y { get; }

        public string BlockId { get; }

        public string CharId { get; }

        public int Width { get; }

        public int Height { get; }

        public AnnotationRegion ToRegion()
        {
            var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(BlockId))
                metadata["block"] = BlockId;
            if (!string.IsNullOrWhiteSpace(CharId))
                metadata["char"] = CharId;
            if (Unicode > 0)
                metadata["unicode"] = $"U+{Unicode:X4}";

            return new AnnotationRegion(
                RegionBounds.Pixels(new SKRect(X, Y, X + Width, Y + Height)),
                AnnotationKind.Character,
                Unicode > 0 ? char.ConvertFromUtf32(Unicode) : "",
                metadata: metadata);
        }

        internal static CodhRoisRectangle? TryParse(string line)
        {
            var parts = line.Split(',');
            if (parts.Length < 8)
                return null;

            var unicode = ParseUnicode(parts[0]);
            var image = parts[1].Trim();
            if (image.Length == 0)
                return null;

            if (!TryParseInt(parts[2], out var x) ||
                !TryParseInt(parts[3], out var y) ||
                !TryParseInt(parts[6], out var width) ||
                !TryParseInt(parts[7], out var height) ||
                width <= 0 ||
                height <= 0)
                return null;

            return new CodhRoisRectangle(unicode, image, x, y, parts[4].Trim(), parts[5].Trim(), width, height);
        }

        private static int ParseUnicode(string value)
        {
            value = value.Trim();
            if (!value.StartsWith("U+", StringComparison.OrdinalIgnoreCase))
                return 0;

            return int.TryParse(value.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var code)
                ? code
                : 0;
        }

        private static bool TryParseInt(string value, out int result)
        {
            return int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }
    }
}
