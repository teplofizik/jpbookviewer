using System.Globalization;
using System.Text.Json;
using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class JsonBookFormat : IBookFormat
{
    private const string BookFileName = "book.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public string Id => "json";

    public string DisplayName => "Generic JSON book";

    public bool CanRead(IReadOnlyStorage storage)
    {
        return FindBookFile(storage.EnumerateFiles().ToList()) != null;
    }

    public Book Load(IReadOnlyStorage storage)
    {
        var bookPath = FindBookFile(storage.EnumerateFiles().ToList())
            ?? throw new InvalidDataException($"В книге не найден файл \"{BookFileName}\".");

        using var stream = storage.OpenRead(bookPath);
        var dto = JsonSerializer.Deserialize<JsonBookDto>(stream, JsonOptions)
            ?? throw new InvalidDataException($"Не удалось прочитать \"{bookPath}\".");

        if (dto.Pages == null || dto.Pages.Count == 0)
            throw new InvalidDataException($"В \"{bookPath}\" нет ни одной страницы.");

        var baseDirectory = StoragePaths.GetDirectory(bookPath);
        var pages = new List<BookPage>(dto.Pages.Count);

        foreach (var pageDto in dto.Pages)
        {
            if (string.IsNullOrWhiteSpace(pageDto.Image))
                throw new InvalidDataException($"В \"{bookPath}\" у страницы не указано изображение.");

            var imagePath = ResolvePath(baseDirectory, pageDto.Image);
            if (!storage.Exists(imagePath))
                throw new FileNotFoundException($"Изображение не найдено в книге: \"{pageDto.Image}\".", imagePath);

            var regions = new List<AnnotationRegion>();
            if (pageDto.Regions != null)
            {
                foreach (var regionDto in pageDto.Regions)
                {
                    var region = ParseRegion(regionDto);
                    if (region != null)
                        regions.Add(region);
                }
            }

            pages.Add(new BookPage(new StoragePageImage(storage, imagePath), regions));
        }

        var name = string.IsNullOrWhiteSpace(dto.Name) ? storage.DisplayName : dto.Name.Trim();
        return new Book(name, storage.DisplayName, storage, pages);
    }

    private static string? FindBookFile(IReadOnlyList<string> files)
    {
        return files
            .Where(path => string.Equals(StoragePaths.GetFileName(path), BookFileName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path.Count(c => c == '/'))
            .ThenBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
    }

    private static string ResolvePath(string baseDirectory, string path)
    {
        var normalized = StoragePaths.Normalize(path);
        if (Path.IsPathRooted(path) || StoragePaths.HasParentTraversal(normalized))
            throw new InvalidDataException($"Недопустимый путь к изображению: \"{path}\".");

        return StoragePaths.Combine(baseDirectory, normalized);
    }

    private static AnnotationRegion? ParseRegion(JsonRegionDto dto)
    {
        if (dto.Rect == null || dto.Rect.Length != 4)
            return null;

        var x = dto.Rect[0];
        var y = dto.Rect[1];
        var width = dto.Rect[2];
        var height = dto.Rect[3];

        if (width <= 0 || height <= 0)
            return null;

        var rect = new SKRect(x, y, x + width, y + height);
        var bounds = IsNormalized(dto.Unit)
            ? RegionBounds.FromNormalized(rect)
            : RegionBounds.Pixels(rect);

        return new AnnotationRegion(
            bounds,
            ParseKind(dto.Kind),
            string.IsNullOrEmpty(dto.Text) ? null : dto.Text,
            string.IsNullOrWhiteSpace(dto.Type) ? null : dto.Type.Trim(),
            dto.Confidence,
            ParseColor(dto.Color),
            ParseMetadata(dto.Metadata));
    }

    private static bool IsNormalized(string? unit)
    {
        return unit != null &&
               (unit.Equals("normalized", StringComparison.OrdinalIgnoreCase) ||
                unit.Equals("relative", StringComparison.OrdinalIgnoreCase) ||
                unit.Equals("fraction", StringComparison.OrdinalIgnoreCase));
    }

    private static AnnotationKind ParseKind(string? kind)
    {
        if (string.IsNullOrWhiteSpace(kind))
            return AnnotationKind.Other;

        return kind.Trim().ToLowerInvariant() switch
        {
            "character" or "char" or "glyph" or "symbol" => AnnotationKind.Character,
            "line" or "textline" or "text-line" or "text" => AnnotationKind.Line,
            "block" or "paragraph" or "group" => AnnotationKind.Block,
            _ => AnnotationKind.Other,
        };
    }

    private static IReadOnlyDictionary<string, string>? ParseMetadata(Dictionary<string, JsonElement>? metadata)
    {
        if (metadata == null || metadata.Count == 0)
            return null;

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in metadata)
        {
            var value = ElementToString(pair.Value);
            if (!string.IsNullOrWhiteSpace(value))
                result[pair.Key] = value;
        }

        return result.Count == 0 ? null : result;
    }

    private static string ElementToString(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "",
            _ => element.GetRawText(),
        };
    }

    private static SKColor? ParseColor(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var text = value.Trim();
        if (text.StartsWith('#'))
            text = text[1..];

        if (text.Length == 3)
            text = string.Concat(text.Select(c => new string(c, 2)));

        if (text.Length == 6)
        {
            return TryParseByte(text[..2], out var r) &&
                   TryParseByte(text[2..4], out var g) &&
                   TryParseByte(text[4..6], out var b)
                ? new SKColor(r, g, b)
                : null;
        }

        if (text.Length == 8)
        {
            return TryParseByte(text[..2], out var a) &&
                   TryParseByte(text[2..4], out var r) &&
                   TryParseByte(text[4..6], out var g) &&
                   TryParseByte(text[6..8], out var b)
                ? new SKColor(r, g, b, a)
                : null;
        }

        return null;
    }

    private static bool TryParseByte(string value, out byte result)
    {
        return byte.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
    }

    private sealed class JsonBookDto
    {
        public string? Name { get; set; }

        public List<JsonPageDto>? Pages { get; set; }
    }

    private sealed class JsonPageDto
    {
        public string? Image { get; set; }

        public List<JsonRegionDto>? Regions { get; set; }
    }

    private sealed class JsonRegionDto
    {
        public float[]? Rect { get; set; }

        public string? Unit { get; set; }

        public string? Kind { get; set; }

        public string? Text { get; set; }

        public string? Type { get; set; }

        public double? Confidence { get; set; }

        public string? Color { get; set; }

        public Dictionary<string, JsonElement>? Metadata { get; set; }
    }
}
