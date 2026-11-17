namespace JpBookViewer.Books;

public sealed class BookPage
{
    public BookPage(IPageImage image, IReadOnlyList<AnnotationRegion> regions)
    {
        Image = image;
        Regions = regions;
    }

    public IPageImage Image { get; }

    public IReadOnlyList<AnnotationRegion> Regions { get; }
}
