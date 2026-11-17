using System.IO.Compression;

namespace JpBookViewer.Books;

public sealed class ZipStorage : IReadOnlyStorage
{
    private readonly FileStream _stream;
    private readonly ZipArchive _archive;
    private readonly Dictionary<string, ZipArchiveEntry> _entries;

    public ZipStorage(string fileName, string? displayName = null)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException($"Архив не найден: \"{fileName}\".", fileName);

        _stream = File.OpenRead(fileName);
        _archive = new ZipArchive(_stream, ZipArchiveMode.Read);
        _entries = new Dictionary<string, ZipArchiveEntry>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in _archive.Entries)
        {
            if (entry.FullName.EndsWith('/') || entry.FullName.EndsWith('\\'))
                continue;

            var path = StoragePaths.Normalize(entry.FullName);
            if (path.Length > 0)
                _entries[path] = entry;
        }

        DisplayName = string.IsNullOrWhiteSpace(displayName)
            ? Path.GetFileNameWithoutExtension(fileName)
            : displayName;
    }

    public string DisplayName { get; }

    public string? LocalRoot => null;

    public IEnumerable<string> EnumerateFiles() => _entries.Keys;

    public bool Exists(string relativePath) => _entries.ContainsKey(StoragePaths.Normalize(relativePath));

    public Stream OpenRead(string relativePath)
    {
        var path = StoragePaths.Normalize(relativePath);
        if (!_entries.TryGetValue(path, out var entry))
            throw new FileNotFoundException($"Файл не найден в архиве: \"{relativePath}\".", relativePath);

        var memory = new MemoryStream();
        using (var stream = entry.Open())
            stream.CopyTo(memory);

        memory.Position = 0;
        return memory;
    }

    public void Dispose()
    {
        _archive.Dispose();
        _stream.Dispose();
    }
}
