using System.Text.Json;
using System.Xml.Linq;

namespace JpBookViewer;

public sealed class Config
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public string BookID { get; set; } = "";

    public int PageNumber { get; set; }

    public int ViewMode { get; set; }

    public string LastPath { get; set; } = "";

    public bool Changed { get; set; }

    private static string ConfigDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "CodhRoisReader");

    private static string JsonPath => Path.Combine(ConfigDirectory, "config.json");

    private static string LegacyXmlPath => Path.Combine(ConfigDirectory, "config.xml");

    public void Save()
    {
        Directory.CreateDirectory(ConfigDirectory);
        File.WriteAllText(JsonPath, JsonSerializer.Serialize(this, JsonOptions));
        Changed = false;
    }

    public static Config Load()
    {
        if (File.Exists(JsonPath))
        {
            try
            {
                return JsonSerializer.Deserialize<Config>(File.ReadAllText(JsonPath)) ?? new Config();
            }
            catch (Exception)
            {
                return new Config();
            }
        }

        return LoadLegacyXml() ?? new Config();
    }

    private static Config? LoadLegacyXml()
    {
        if (!File.Exists(LegacyXmlPath))
            return null;

        try
        {
            var root = XDocument.Load(LegacyXmlPath).Root;
            if (root == null)
                return null;

            var config = new Config
            {
                BookID = root.Element(nameof(BookID))?.Value ?? "",
                LastPath = root.Element(nameof(LastPath))?.Value ?? "",
                ViewMode = ParseInt(root.Element(nameof(ViewMode))?.Value),
                PageNumber = ParseInt(root.Element(nameof(PageNumber))?.Value),
            };

            return config;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static int ParseInt(string? value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }
}
