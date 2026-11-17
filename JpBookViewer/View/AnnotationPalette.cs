using JpBookViewer.Books;
using SkiaSharp;

namespace JpBookViewer.View;

public static class AnnotationPalette
{
    private static readonly SKColor CharacterColor = new(255, 96, 96);
    private static readonly SKColor LineColor = new(80, 180, 255);
    private static readonly SKColor BlockColor = new(120, 220, 140);
    private static readonly SKColor OtherColor = new(255, 210, 80);

    public static SKColor Resolve(AnnotationRegion region)
    {
        if (region.Color is { } color)
            return color;

        if (!string.IsNullOrWhiteSpace(region.Type))
            return ResolveType(region.Type);

        return ResolveKind(region.Kind);
    }

    private static SKColor ResolveType(string type)
    {
        var normalized = type.Trim().ToLowerInvariant();

        return normalized switch
        {
            "character" or "char" or "glyph" or "symbol" => CharacterColor,
            "line" or "textline" or "text-line" or "text" => LineColor,
            "block" or "paragraph" or "group" or "page" => BlockColor,
            _ => FromTypeName(normalized),
        };
    }

    private static SKColor FromTypeName(string type)
    {
        var hash = 2166136261u;
        foreach (var c in type)
        {
            hash ^= c;
            hash *= 16777619u;
        }

        var hue = (float)(hash % 360u);
        return SKColor.FromHsv(hue, 62f, 92f);
    }

    private static SKColor ResolveKind(AnnotationKind kind)
    {
        return kind switch
        {
            AnnotationKind.Character => CharacterColor,
            AnnotationKind.Line => LineColor,
            AnnotationKind.Block => BlockColor,
            _ => OtherColor,
        };
    }
}
