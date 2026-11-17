using System.Globalization;
using System.Text.Json;
using SkiaSharp;

namespace JpBookViewer.Books;

/// <summary>
/// Reads the Manyoshu OCR line dataset produced by NyaKobunReader.
/// The folder must contain manyoshu_lines_roles.jsonl; image paths are absolute.
/// </summary>
public sealed class ManyoshuBookFormat : IBookFormat
{
    private const string RolesFileName = "manyoshu_lines_roles.jsonl";
    private const string ManifestFileName = "manyoshu.book.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
    };

    public string Id => "manyoshu";

    public string DisplayName => "Manyoshu OCR lines";

    public bool CanRead(IReadOnlyStorage storage)
    {
        return FindManifestFile(storage) != null || FindRolesFile(storage) != null;
    }

    public Book Load(IReadOnlyStorage storage)
    {
        var manifestPath = FindManifestFile(storage);
        var manifest = manifestPath != null ? ReadManifest(storage, manifestPath) : null;
        var rolesPath = manifest?.Lines ?? FindRolesFile(storage)
            ?? throw new InvalidDataException($"No {RolesFileName} or {ManifestFileName} found.");
        var baseDirectory = Path.GetDirectoryName(rolesPath) ?? "";
        var imageRoot = manifest?.ImageRoot;
        if (!string.IsNullOrWhiteSpace(imageRoot) && !Path.IsPathRooted(imageRoot))
            imageRoot = Path.Combine(baseDirectory, imageRoot);

        var lines = ReadLines(storage, rolesPath);
        var pages = new List<BookPage>();

        foreach (var group in lines
                     .Where(line => !string.IsNullOrWhiteSpace(line.Image))
                     .GroupBy(line => (line.Volume, line.Page))
                     .OrderBy(group => ParseVolume(group.Key.Volume))
                     .ThenBy(group => ParsePage(group.Key.Page)))
        {
            var first = group.First();
            var imagePath = ResolveImagePath(first.Image!, imageRoot ?? baseDirectory);
            var regions = new List<AnnotationRegion>();

            foreach (var line in group.OrderBy(line => ParseLineId(line.LineId)))
            {
                var region = ToRegion(line);
                if (region != null)
                    regions.Add(region);
            }

            pages.Add(new BookPage(new FilePageImage(imagePath), regions));
        }

        if (pages.Count == 0)
            throw new InvalidDataException("No pages found in Manyoshu roles file.");

        var name = string.IsNullOrWhiteSpace(manifest?.Name) ? "Manyoshu OCR" : manifest!.Name!;
        return new Book(name, storage.DisplayName, storage, pages);
    }

    private static ManyoshuManifest? ReadManifest(IReadOnlyStorage storage, string path)
    {
        using var stream = storage.OpenRead(path);
        return JsonSerializer.Deserialize<ManyoshuManifest>(stream, JsonOptions);
    }

    private static List<ManyoshuLine> ReadLines(IReadOnlyStorage storage, string path)
    {
        var result = new List<ManyoshuLine>();
        using var stream = File.Exists(path)
            ? File.OpenRead(path)
            : storage.OpenRead(path);
        using var reader = new StreamReader(stream);
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var dto = JsonSerializer.Deserialize<ManyoshuLine>(line, JsonOptions);
            if (dto != null)
                result.Add(dto);
        }

        return result;
    }

    private static AnnotationRegion? ToRegion(ManyoshuLine line)
    {
        if (line.Bbox is not { Length: 4 } box)
            return null;

        var rect = new SKRect(box[0], box[1], box[2], box[3]);
        if (rect.Width <= 0 || rect.Height <= 0)
            return null;

        var role = line.Role ?? "comment";
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Add(metadata, "role", role);
        Add(metadata, "subrole", line.Subrole);
        Add(metadata, "status", line.Status);
        Add(metadata, "poem_no", line.PoemNo);
        Add(metadata, "global_line", line.GlobalLineId);
        Add(metadata, "block", line.RefBlockId);
        Add(metadata, "block_hint", line.BlockHint);
        Add(metadata, "block_assignment", line.BlockAssignment);
        Add(metadata, "comment_kind", line.CommentKind);
        Add(metadata, "ocr", line.Text);
        if (line.RefSegmentIds is { Length: > 0 })
            Add(metadata, "segments", string.Join(",", line.RefSegmentIds));

        var display = !string.IsNullOrWhiteSpace(line.TextClean) ? line.TextClean! : line.Text ?? "";

        return new AnnotationRegion(
            RegionBounds.Pixels(rect),
            AnnotationKind.Line,
            display,
            role,
            line.Similarity,
            RoleColor(role),
            metadata);
    }

    private static SKColor RoleColor(string role)
    {
        return role.ToLowerInvariant() switch
        {
            "poem" => new SKColor(0x18, 0x9c, 0x35),
            "header" or "header_continuation" => new SKColor(0x14, 0x58, 0xa0),
            "comment" => new SKColor(0xb0, 0x4a, 0x4a),
            "reading" => new SKColor(0x7a, 0x8a, 0xa0),
            "small" => new SKColor(0xa0, 0xa0, 0xa0),
            "label" => new SKColor(0x86, 0x46, 0xc8),
            "artifact" => new SKColor(0x33, 0x33, 0x33),
            _ => new SKColor(0xff, 0xd2, 0x50),
        };
    }

    private static string ResolveImagePath(string image, string baseDirectory)
    {
        if (Path.IsPathRooted(image))
            return image;

        var combined = Path.Combine(baseDirectory, image);
        return File.Exists(combined) ? combined : image;
    }

    private static string? FindManifestFile(IReadOnlyStorage storage)
    {
        return storage.EnumerateFiles()
            .FirstOrDefault(path => string.Equals(
                StoragePaths.GetFileName(path),
                ManifestFileName,
                StringComparison.OrdinalIgnoreCase));
    }

    private static string? FindRolesFile(IReadOnlyStorage storage)
    {
        return storage.EnumerateFiles()
            .FirstOrDefault(path => string.Equals(
                StoragePaths.GetFileName(path),
                RolesFileName,
                StringComparison.OrdinalIgnoreCase));
    }

    private static int ParseVolume(string? value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : 0;
    }

    private static int ParsePage(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        var digits = value.StartsWith("page", StringComparison.OrdinalIgnoreCase)
            ? value[4..]
            : value;
        return int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : 0;
    }

    private static int ParseLineId(int? value) => value ?? int.MaxValue;

    private static void Add(IDictionary<string, string> target, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            target[key] = value;
    }

    private sealed class ManyoshuManifest
    {
        public string? Name { get; set; }

        public string? Lines { get; set; }

        public string? ImageRoot { get; set; }
    }

    private sealed class ManyoshuLine
    {
        public string? Volume { get; set; }

        public string? Page { get; set; }

        public int? LineId { get; set; }

        public string? Image { get; set; }

        public float[]? Bbox { get; set; }

        public string? Text { get; set; }

        public string? TextClean { get; set; }

        public string? Role { get; set; }

        public string? Subrole { get; set; }

        public string? Status { get; set; }

        public string? PoemNo { get; set; }

        public double? Similarity { get; set; }

        public string? GlobalLineId { get; set; }

        public string? RefBlockId { get; set; }

        public string? BlockHint { get; set; }

        public string? BlockAssignment { get; set; }

        public string? CommentKind { get; set; }

        public int[]? RefSegmentIds { get; set; }
    }
}
