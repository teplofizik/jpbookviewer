using SkiaSharp;

namespace JpBookViewer.Books;

public sealed class AnnotationRegion
{
    private static readonly IReadOnlyDictionary<string, string> EmptyMetadata =
        new Dictionary<string, string>();

    public AnnotationRegion(
        RegionBounds bounds,
        AnnotationKind kind = AnnotationKind.Other,
        string? text = null,
        string? type = null,
        double? confidence = null,
        SKColor? color = null,
        IReadOnlyDictionary<string, string>? metadata = null)
    {
        Bounds = bounds;
        Kind = kind;
        Text = text;
        Type = type;
        Confidence = confidence;
        Color = color;
        Metadata = metadata ?? EmptyMetadata;
    }

    public RegionBounds Bounds { get; }

    public AnnotationKind Kind { get; }

    public string? Text { get; }

    public string? Type { get; }

    public double? Confidence { get; }

    public SKColor? Color { get; }

    public IReadOnlyDictionary<string, string> Metadata { get; }

    public string DisplayText => Text ?? "";
}
