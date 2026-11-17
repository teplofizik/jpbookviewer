namespace JpBookViewer.View;

public sealed class AnnotationDisplayOptions
{
    public bool ShowAllRegions { get; set; }

    public bool ShowLabels { get; set; }

    public bool ShowConfidence { get; set; } = true;

    public bool ShowMetadata { get; set; } = true;

    public bool HintInCorner { get; set; } = true;
}
