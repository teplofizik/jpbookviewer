namespace JpBookViewer.Books;

public sealed class HttpZipStorage : IReadOnlyStorage
{
    private static readonly HttpClient Client = CreateClient();

    private readonly ZipStorage _inner;
    private readonly string _tempPath;

    public HttpZipStorage(Uri uri, string? displayName = null)
    {
        var directory = Path.Combine(Path.GetTempPath(), "CodhRoisReader", "downloads");
        Directory.CreateDirectory(directory);
        _tempPath = Path.Combine(directory, $"{Guid.NewGuid():N}.zip");

        try
        {
            using var response = Client
                .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead)
                .GetAwaiter()
                .GetResult();

            response.EnsureSuccessStatusCode();

            using (var input = response.Content.ReadAsStream())
            using (var output = File.Create(_tempPath))
                input.CopyTo(output);

            var name = string.IsNullOrWhiteSpace(displayName)
                ? Path.GetFileNameWithoutExtension(uri.LocalPath)
                : displayName;

            if (string.IsNullOrWhiteSpace(name))
                name = uri.Host;

            _inner = new ZipStorage(_tempPath, name);
        }
        catch (InvalidDataException ex)
        {
            TryDelete();
            throw new InvalidDataException("Сетевой источник должен быть ZIP-архивом книги.", ex);
        }
        catch (Exception)
        {
            TryDelete();
            throw;
        }
    }

    public string DisplayName => _inner.DisplayName;

    public string? LocalRoot => null;

    public IEnumerable<string> EnumerateFiles() => _inner.EnumerateFiles();

    public bool Exists(string relativePath) => _inner.Exists(relativePath);

    public Stream OpenRead(string relativePath) => _inner.OpenRead(relativePath);

    public void Dispose()
    {
        _inner.Dispose();
        TryDelete();
    }

    private void TryDelete()
    {
        try
        {
            if (File.Exists(_tempPath))
                File.Delete(_tempPath);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("JpBookViewer/2.0");
        return client;
    }
}
