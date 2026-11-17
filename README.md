# JP Book Viewer

Reader for <a href="http://codh.rois.ac.jp/char-shape/book/">codh.rois char-shape books</a>
and other OCR-annotated image collections.

Open a book from a folder, a ZIP archive or a remote ZIP archive (HTTP/HTTPS).

<img src='/assets/example.png'>

## Features

- Smooth zoom with the mouse wheel (zooms at the cursor), drag to pan, double-click to fit the page.
- Page navigation with the buttons, `←` / `→`, `PageUp` / `PageDown`, `Home` / `End`.
- Two hint modes: glyph over the region, or an info box in the corner.
- Character, line and block regions with optional metadata: type, confidence, color and arbitrary key/value pairs.
- Display toggles: all regions, labels, confidence, metadata. Missing metadata is simply not shown.
- Remembers the last book, page and display settings.
- Optional static web export (`Book.ProcessToWeb`).

## Requirements

- Windows 10 version 2004 (build 19041) or newer.
- [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) for framework-dependent builds.

## Build

```powershell
dotnet build JpBookViewer.sln
dotnet run --project JpBookViewer
```

## Publish

Self-contained single-file executable:

```powershell
dotnet publish JpBookViewer -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The executable is produced as `CodhRoisReader.exe`.

## Supported sources

- A folder with the book files.
- A local `.zip` archive.
- A remote `.zip` archive (`http://` or `https://` URL). The archive is downloaded to a temporary
  directory and removed when the book is closed.

## Book formats

### CODROIS coordinate CSV

```text
<book-id>/
  <book-id>_coordinate.csv
  images/
    <book-id>_00001_1.jpg
    ...
```

CSV columns: `Unicode,Image,X,Y,Block ID,Char ID,Width,Height`.
Regions are character boxes; block/char/unicode values are exposed as metadata.

### Generic JSON

Place `book.json` next to an `images` folder (or anywhere inside the archive):

```json
{
  "name": "My book",
  "pages": [
    {
      "image": "images/page1.jpg",
      "regions": [
        {
          "rect": [20, 30, 40, 50],
          "unit": "pixels",
          "kind": "character",
          "text": "も",
          "type": "kana",
          "confidence": 0.97,
          "metadata": { "block": "B1", "char": "C1" }
        },
        {
          "rect": [0.02, 0.12, 0.5, 0.1],
          "unit": "normalized",
          "kind": "line",
          "text": "吾輩は猫である",
          "type": "line",
          "color": "#3399FF"
        }
      ]
    }
  ]
}
```

- `rect` is `[x, y, width, height]`.
- `unit` is `pixels` (default) or `normalized` (`0..1` relative to the image).
- `kind` is `character`, `line`, `block` or any other value (`other`).
- `color` accepts `#RGB`, `#RRGGBB` or `#AARRGGBB`. Without it, the color is chosen by `kind`.
- `confidence`, `type`, `text` and `metadata` are optional and are hidden when the corresponding
  display options are disabled.

`book.json` takes priority over a coordinate CSV when both are present.

## Technology

- .NET 10 / WinForms
- [SkiaSharp](https://github.com/mono/SkiaSharp) for rendering and image decoding
- Storage and annotation formats are separate abstractions:
  `IReadOnlyStorage` (folder / ZIP / HTTP) and `IBookFormat` (CODROIS / JSON)
- JSON configuration in `%LOCALAPPDATA%\CodhRoisReader\config.json`
  (the legacy `config.xml` is imported automatically)
