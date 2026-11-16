namespace JpBookViewer.Books;

public sealed class Page
{
    public Page(string image)
    {
        Image = image;
    }

    public string Image { get; }

    public List<PageRect> Rects { get; } = new();
}
