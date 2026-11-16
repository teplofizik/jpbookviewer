using System.Globalization;
using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class CodhRois
{
    private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".bmp"];

    private readonly Dictionary<string, int> _imageIndex;

    public CodhRois(string directory)
    {
        Directory = directory;
        Name = Path.GetFileName(Path.TrimEndingDirectorySeparator(directory));

        Images = LoadImages(Path.Combine(directory, "images"));
        _imageIndex = new Dictionary<string, int>(Images.Count, StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < Images.Count; i++)
            _imageIndex[Path.GetFileNameWithoutExtension(Images[i])] = i;

        Rectangles = LoadRectangles(FindCoordinateFile(directory, Name));
    }

    public string Directory { get; }

    public string Name { get; }

    public IReadOnlyList<string> Images { get; }

    public IReadOnlyList<CodhRoisRectangle> Rectangles { get; }

    public int GetPageId(string image)
    {
        var key = Path.GetFileNameWithoutExtension(Path.GetFileName(image));
        return _imageIndex.TryGetValue(key, out var index) ? index : -1;
    }

    public static bool CheckIsRois(string directory)
    {
        try
        {
            if (!System.IO.Directory.Exists(directory))
                return false;

            var imagesDirectory = Path.Combine(directory, "images");
            if (!System.IO.Directory.Exists(imagesDirectory))
                return false;

            return TryFindCoordinateFile(directory, Path.GetFileName(Path.TrimEndingDirectorySeparator(directory))) != null;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static string FindCoordinateFile(string directory, string bookName)
    {
        return TryFindCoordinateFile(directory, bookName)
            ?? throw new FileNotFoundException(
                $"Файл \"{bookName}_coordinate.csv\" не найден в папке \"{directory}\".",
                Path.Combine(directory, $"{bookName}_coordinate.csv"));
    }

    private static string? TryFindCoordinateFile(string directory, string bookName)
    {
        var expected = Path.Combine(directory, $"{bookName}_coordinate.csv");
        if (File.Exists(expected))
            return expected;

        return System.IO.Directory
            .EnumerateFiles(directory, "*_coordinate.csv", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();
    }

    private static List<string> LoadImages(string imagesDirectory)
    {
        if (!System.IO.Directory.Exists(imagesDirectory))
            throw new DirectoryNotFoundException($"Папка с изображениями не найдена: \"{imagesDirectory}\".");

        return System.IO.Directory
            .EnumerateFiles(imagesDirectory, "*", SearchOption.TopDirectoryOnly)
            .Where(path => ImageExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<CodhRoisRectangle> LoadRectangles(string fileName)
    {
        var result = new List<CodhRoisRectangle>();
        var lines = File.ReadAllLines(fileName);

        for (var i = 1; i < lines.Length; i++)
        {
            var rectangle = CodhRoisRectangle.TryParse(lines[i]);
            if (rectangle != null)
                result.Add(rectangle);
        }

        return result;
    }

    public sealed class CodhRoisRectangle
    {
        private CodhRoisRectangle(int unicode, string image, int x, int y, string blockId, string charId, int width, int height)
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

        public string CharView => Unicode > 0 ? char.ConvertFromUtf32(Unicode) : "";

        public SKRect Rect => new(X, Y, X + Width, Y + Height);

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
                !TryParseInt(parts[7], out var height))
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
