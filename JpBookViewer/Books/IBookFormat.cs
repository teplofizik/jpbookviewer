namespace JpBookViewer.Books;

public interface IBookFormat
{
    string Id { get; }

    string DisplayName { get; }

    bool CanRead(IReadOnlyStorage storage);

    Book Load(IReadOnlyStorage storage);
}
