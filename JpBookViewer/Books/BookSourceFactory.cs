namespace JpBookViewer.Books;

public static class BookSourceFactory
{
    private static readonly IBookFormat[] Formats =
    [
        new ManyoshuBookFormat(),
        new JsonBookFormat(),
        new CodhRoisBookFormat(),
    ];

    public static bool CanOpen(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return false;

        location = location.Trim();

        if (IsRemote(location))
            return true;

        try
        {
            using var storage = CreateStorage(location);
            return Formats.Any(format => format.CanRead(storage));
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static Book Open(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Не указан путь к книге.", nameof(location));

        var storage = CreateStorage(location.Trim());

        try
        {
            var format = Formats.FirstOrDefault(candidate => candidate.CanRead(storage))
                ?? throw new InvalidDataException(
                    "Не удалось определить формат книги. Ожидается \"book.json\" или \"*_coordinate.csv\".");

            return format.Load(storage);
        }
        catch (Exception)
        {
            storage.Dispose();
            throw;
        }
    }

    private static IReadOnlyStorage CreateStorage(string location)
    {
        if (Directory.Exists(location))
            return new DirectoryStorage(location);

        if (IsRemote(location))
            return new HttpZipStorage(new Uri(location, UriKind.Absolute));

        if (File.Exists(location))
        {
            if (string.Equals(Path.GetExtension(location), ".zip", StringComparison.OrdinalIgnoreCase))
                return new ZipStorage(location);

            throw new NotSupportedException(
                "Поддерживаются папки с книгой и ZIP-архивы (.zip).");
        }

        throw new DirectoryNotFoundException($"Папка или файл не найдены: \"{location}\".");
    }

    private static bool IsRemote(string location)
    {
        return Uri.TryCreate(location, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
