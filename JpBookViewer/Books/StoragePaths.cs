namespace JpBookViewer.Books;

internal static class StoragePaths
{
    public static string Normalize(string path)
    {
        return path.Replace('\\', '/').TrimStart('/');
    }

    public static string GetDirectory(string path)
    {
        var index = path.LastIndexOf('/');
        return index < 0 ? "" : path[..index];
    }

    public static string GetFileName(string path)
    {
        var index = path.LastIndexOf('/');
        return index < 0 ? path : path[(index + 1)..];
    }

    public static string GetFileNameWithoutExtension(string path)
    {
        var name = GetFileName(path);
        var index = name.LastIndexOf('.');
        return index <= 0 ? name : name[..index];
    }

    public static string GetExtension(string path)
    {
        var name = GetFileName(path);
        var index = name.LastIndexOf('.');
        return index < 0 ? "" : name[index..];
    }

    public static string Combine(string directory, string path)
    {
        path = Normalize(path);
        if (string.IsNullOrEmpty(directory))
            return path;

        return Normalize($"{directory.TrimEnd('/')}/{path}");
    }

    public static bool HasParentTraversal(string path)
    {
        return Normalize(path)
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Any(part => part == "..");
    }
}
