namespace JpBookViewer.Books;

public interface IReadOnlyStorage : IDisposable
{
    string DisplayName { get; }

    string? LocalRoot { get; }

    IEnumerable<string> EnumerateFiles();

    bool Exists(string relativePath);

    Stream OpenRead(string relativePath);
}
