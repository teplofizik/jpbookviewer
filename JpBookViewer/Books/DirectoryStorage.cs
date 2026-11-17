namespace JpBookViewer.Books;

public sealed class DirectoryStorage : IReadOnlyStorage
{
    private readonly string _root;

    public DirectoryStorage(string directory)
    {
        _root = Path.GetFullPath(directory);
        if (!Directory.Exists(_root))
            throw new DirectoryNotFoundException($"Папка не найдена: \"{directory}\".");

        DisplayName = new DirectoryInfo(_root).Name;
        if (string.IsNullOrWhiteSpace(DisplayName))
            DisplayName = _root;
    }

    public string DisplayName { get; }

    public string? LocalRoot => _root;

    public IEnumerable<string> EnumerateFiles()
    {
        return Directory
            .EnumerateFiles(_root, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(_root, path).Replace('\\', '/'));
    }

    public bool Exists(string relativePath)
    {
        var path = Resolve(relativePath);
        return path != null && File.Exists(path);
    }

    public Stream OpenRead(string relativePath)
    {
        var path = Resolve(relativePath)
            ?? throw new FileNotFoundException($"Файл не найден в книге: \"{relativePath}\".");

        return File.OpenRead(path);
    }

    public void Dispose()
    {
    }

    private string? Resolve(string relativePath)
    {
        var normalized = StoragePaths.Normalize(relativePath);
        if (normalized.Length == 0 || StoragePaths.HasParentTraversal(normalized))
            return null;

        var fullPath = Path.GetFullPath(Path.Combine(_root, normalized.Replace('/', Path.DirectorySeparatorChar)));
        var rootWithSeparator = _root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

        return fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase) ? fullPath : null;
    }
}
