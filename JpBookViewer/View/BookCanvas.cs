using System.Globalization;
using JpBookViewer.Books;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace JpBookViewer.View;

public sealed class BookCanvas : SKControl
{
    private const float MinScale = 0.05f;
    private const float MaxScale = 16f;
    private const int MaxMetadataLines = 4;

    private static readonly SKSamplingOptions Sampling =
        new(SKFilterMode.Linear, SKMipmapMode.Linear);

    private static readonly Lazy<SKTypeface> TextTypeface = new(() =>
    {
        try
        {
            return SKFontManager.Default.MatchCharacter('あ')
                ?? SKTypeface.FromFamilyName("Yu Gothic UI")
                ?? SKTypeface.FromFamilyName("Meiryo")
                ?? SKTypeface.Default;
        }
        catch (Exception)
        {
            return SKTypeface.Default;
        }
    });

    private static readonly Lazy<SKTypeface> InterfaceTypeface = new(() =>
        SKTypeface.FromFamilyName("Segoe UI")
        ?? SKTypeface.FromFamilyName("Tahoma")
        ?? SKTypeface.Default);

    private SKBitmap? _bitmap;
    private SKImage? _image;
    private BookPage? _page;
    private AnnotationRegion? _hover;

    private AnnotationDisplayOptions _displayOptions = new();

    private float _scale = 1f;
    private float _offsetX;
    private float _offsetY;
    private bool _fitted = true;

    private float _surfaceWidth;
    private float _surfaceHeight;
    private float _surfaceScaleX = 1f;
    private float _surfaceScaleY = 1f;

    private bool _panning;
    private Point _lastMouse;

    public BookCanvas()
    {
        BackColor = Color.Black;
        TabStop = true;
        SetStyle(ControlStyles.Selectable, true);
        PaintSurface += OnPaintSurface;
    }

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public AnnotationDisplayOptions DisplayOptions
    {
        get => _displayOptions;
        set
        {
            _displayOptions = value ?? new AnnotationDisplayOptions();
            Invalidate();
        }
    }

    public void SetPage(BookPage page)
    {
        _page = page;
        _hover = null;

        _image?.Dispose();
        _image = null;
        _bitmap?.Dispose();
        _bitmap = null;

        _bitmap = page.Image.Load();

        if (_bitmap != null)
            _image = SKImage.FromBitmap(_bitmap);

        _fitted = true;
        Invalidate();
    }

    public void FitToWindow()
    {
        _fitted = true;
        Invalidate();
    }

    private void ZoomAtSurface(SKPoint point, int delta)
    {
        if (_image == null || delta == 0)
            return;

        var factor = MathF.Pow(1.15f, delta / 120f);
        var nextScale = Math.Clamp(_scale * factor, MinScale, MaxScale);
        if (MathF.Abs(nextScale - _scale) < 0.0001f)
            return;

        var pagePoint = ToPage(point);

        _scale = nextScale;
        _offsetX = point.X - pagePoint.X * _scale;
        _offsetY = point.Y - pagePoint.Y * _scale;
        _fitted = false;

        Invalidate();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        _surfaceWidth = info.Width;
        _surfaceHeight = info.Height;
        _surfaceScaleX = info.Width / Math.Max(1f, ClientSize.Width);
        _surfaceScaleY = info.Height / Math.Max(1f, ClientSize.Height);

        canvas.Clear(SKColors.Black);

        if (_fitted)
            FitToView();

        if (_image == null)
        {
            DrawEmptyState(canvas);
            return;
        }

        canvas.Save();
        canvas.Translate(_offsetX, _offsetY);
        canvas.Scale(_scale);
        canvas.DrawImage(_image, 0, 0, Sampling);
        DrawRegions(canvas);
        canvas.Restore();

        DrawHoverInfo(canvas);
    }

    private void FitToView()
    {
        if (_image == null || _surfaceWidth <= 0 || _surfaceHeight <= 0)
            return;

        var imageWidth = _image.Width;
        var imageHeight = _image.Height;
        if (imageWidth <= 0 || imageHeight <= 0)
            return;

        _scale = MathF.Min(_surfaceWidth / imageWidth, _surfaceHeight / imageHeight);
        _scale = Math.Clamp(MathF.Min(_scale, 1f), MinScale, MaxScale);

        _offsetX = (_surfaceWidth - imageWidth * _scale) / 2f;
        _offsetY = (_surfaceHeight - imageHeight * _scale) / 2f;
    }

    private void DrawRegions(SKCanvas canvas)
    {
        if (_page == null || _image == null)
            return;

        var imageWidth = _image.Width;
        var imageHeight = _image.Height;

        foreach (var region in _page.Regions)
        {
            if (ReferenceEquals(region, _hover))
                continue;

            var rect = region.Bounds.ToPixels(imageWidth, imageHeight);
            var color = AnnotationPalette.Resolve(region);

            if (_displayOptions.ShowAllRegions)
            {
                using var stroke = new SKPaint
                {
                    Color = color.WithAlpha(150),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1.5f / _scale,
                    IsAntialias = true,
                };
                canvas.DrawRect(rect, stroke);
            }

            if (_displayOptions.ShowLabels && !string.IsNullOrEmpty(region.Text))
                DrawTextFitted(canvas, region.Text, rect, 0.8f, color.WithAlpha(230));
        }

        DrawHoveredRegion(canvas, imageWidth, imageHeight);
    }

    private void DrawHoveredRegion(SKCanvas canvas, int imageWidth, int imageHeight)
    {
        if (_hover == null)
            return;

        var rect = _hover.Bounds.ToPixels(imageWidth, imageHeight);
        var color = AnnotationPalette.Resolve(_hover);

        using (var fill = new SKPaint
               {
                   Color = color.WithAlpha(60),
                   Style = SKPaintStyle.Fill,
                   IsAntialias = true,
               })
        {
            canvas.DrawRect(rect, fill);
        }

        using (var stroke = new SKPaint
               {
                   Color = color.WithAlpha(230),
                   Style = SKPaintStyle.Stroke,
                   StrokeWidth = 2f / _scale,
                   IsAntialias = true,
               })
        {
            canvas.DrawRect(rect, stroke);
        }

        if (!_displayOptions.HintInCorner && !string.IsNullOrEmpty(_hover.Text))
            DrawTextFitted(canvas, _hover.Text, rect, 0.9f, SKColors.Black);
    }

    private void DrawHoverInfo(SKCanvas canvas)
    {
        if (_hover == null)
            return;

        var details = BuildDetails(_hover);

        if (_displayOptions.HintInCorner)
        {
            DrawInfoPanel(canvas, _hover.DisplayText, details);
            return;
        }

        if (details.Count > 0)
            DrawInfoPanel(canvas, null, details);
    }

    private List<string> BuildDetails(AnnotationRegion region)
    {
        var details = new List<string>();

        if (_displayOptions.ShowMetadata && !string.IsNullOrWhiteSpace(region.Type))
            details.Add($"Тип: {region.Type}");

        if (_displayOptions.ShowConfidence && region.Confidence is { } confidence)
            details.Add($"Уверенность: {FormatConfidence(confidence)}");

        if (_displayOptions.ShowMetadata)
        {
            var metadata = region.Metadata
                .Where(pair => !(region.Type != null && pair.Key.Equals("type", StringComparison.OrdinalIgnoreCase)))
                .Where(pair => !(region.Confidence != null && pair.Key.Equals("confidence", StringComparison.OrdinalIgnoreCase)))
                .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .Take(MaxMetadataLines);

            foreach (var pair in metadata)
                details.Add($"{pair.Key}: {pair.Value}");
        }

        return details;
    }

    private void DrawInfoPanel(SKCanvas canvas, string? title, IReadOnlyList<string> details)
    {
        var hasTitle = !string.IsNullOrEmpty(title);
        if (!hasTitle && details.Count == 0)
            return;

        var ui = _surfaceScaleX > 0 ? _surfaceScaleX : 1f;
        var margin = 12f * ui;
        var padding = 10f * ui;
        var detailSize = 12f * ui;
        var detailLineHeight = detailSize * 1.45f;
        var titleHeight = hasTitle ? 64f * ui : 0f;

        using var detailFont = new SKFont(InterfaceTypeface.Value, detailSize);
        var detailsWidth = details.Count == 0 ? 0f : details.Max(line => detailFont.MeasureText(line));
        var panelWidth = Math.Clamp(MathF.Max(120f * ui, detailsWidth + padding * 2), 120f * ui, 420f * ui);
        var panelHeight = padding * 2 + titleHeight + details.Count * detailLineHeight;

        var panel = new SKRect(margin, margin, margin + panelWidth, margin + panelHeight);

        using (var background = new SKPaint
               {
                   Color = new SKColor(255, 255, 255, 238),
                   Style = SKPaintStyle.Fill,
                   IsAntialias = true,
               })
        {
            canvas.DrawRoundRect(panel, 6f * ui, 6f * ui, background);
        }

        using (var border = new SKPaint
               {
                   Color = SKColors.Black,
                   Style = SKPaintStyle.Stroke,
                   StrokeWidth = 2f * ui,
                   IsAntialias = true,
               })
        {
            canvas.DrawRoundRect(panel, 6f * ui, 6f * ui, border);
        }

        var contentLeft = panel.Left + padding;
        var contentRight = panel.Right - padding;

        if (hasTitle)
        {
            var titleArea = new SKRect(contentLeft, panel.Top + padding, contentRight, panel.Top + padding + titleHeight);
            DrawTextFitted(canvas, title!, titleArea, 0.82f, SKColors.Black);
        }

        if (details.Count > 0)
        {
            using var paint = new SKPaint { Color = new SKColor(30, 30, 30), IsAntialias = true };
            canvas.Save();
            canvas.ClipRect(panel);

            var y = panel.Top + padding + titleHeight + detailSize;
            foreach (var line in details)
            {
                canvas.DrawText(line, contentLeft, y, detailFont, paint);
                y += detailLineHeight;
            }

            canvas.Restore();
        }
    }

    private static void DrawTextFitted(SKCanvas canvas, string text, SKRect area, float factor, SKColor color)
    {
        if (string.IsNullOrEmpty(text) || area.Width <= 0 || area.Height <= 0)
            return;

        var maxSize = MathF.Min(area.Width, area.Height) * factor;
        if (maxSize <= 0)
            return;

        using var font = new SKFont(TextTypeface.Value, maxSize);
        var bounds = new SKRect();
        var width = font.MeasureText(text, out bounds);

        var maxWidth = area.Width * factor;
        if (width > maxWidth && width > 0)
        {
            font.Size = maxSize * maxWidth / width;
            font.MeasureText(text, out bounds);
        }

        var maxHeight = area.Height * factor;
        if (bounds.Height > maxHeight && bounds.Height > 0)
        {
            font.Size *= maxHeight / bounds.Height;
            font.MeasureText(text, out bounds);
        }

        using var paint = new SKPaint { Color = color, IsAntialias = true };
        canvas.DrawText(text, area.MidX - bounds.MidX, area.MidY - bounds.MidY, font, paint);
    }

    private static string FormatConfidence(double value)
    {
        var percent = value <= 1.0 ? value * 100.0 : value;
        return percent.ToString("0.#", CultureInfo.InvariantCulture) + "%";
    }

    private void DrawEmptyState(SKCanvas canvas)
    {
        var ui = _surfaceScaleX > 0 ? _surfaceScaleX : 1f;
        using var font = new SKFont(InterfaceTypeface.Value, 16f * ui);
        using var paint = new SKPaint
        {
            Color = new SKColor(160, 160, 160),
            IsAntialias = true,
        };

        const string text = "Нет страницы";
        var width = font.MeasureText(text);
        canvas.DrawText(text, (_surfaceWidth - width) / 2f, _surfaceHeight / 2f, font, paint);
    }

    private SKPoint ToSurface(Point clientPoint)
    {
        return new SKPoint(clientPoint.X * _surfaceScaleX, clientPoint.Y * _surfaceScaleY);
    }

    private SKPoint ToPage(SKPoint surfacePoint)
    {
        return new SKPoint(
            (surfacePoint.X - _offsetX) / _scale,
            (surfacePoint.Y - _offsetY) / _scale);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button != MouseButtons.Left || _image == null)
            return;

        Focus();
        _panning = true;
        _lastMouse = e.Location;
        Capture = true;
        Cursor = Cursors.SizeAll;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (!_panning)
            return;

        _panning = false;
        Capture = false;
        Cursor = Cursors.Default;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_image == null)
            return;

        if (_panning)
        {
            _offsetX += (e.X - _lastMouse.X) * _surfaceScaleX;
            _offsetY += (e.Y - _lastMouse.Y) * _surfaceScaleY;
            _lastMouse = e.Location;
            _fitted = false;
            Invalidate();
            return;
        }

        UpdateHover(e.Location);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        ZoomAtSurface(ToSurface(e.Location), e.Delta);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        Focus();
    }

    protected override void OnDoubleClick(EventArgs e)
    {
        base.OnDoubleClick(e);
        FitToWindow();
    }

    private void UpdateHover(Point location)
    {
        var pagePoint = ToPage(ToSurface(location));
        AnnotationRegion? hover = null;

        if (_page != null && _image != null)
        {
            var imageWidth = _image.Width;
            var imageHeight = _image.Height;
            var bestArea = float.MaxValue;

            foreach (var region in _page.Regions)
            {
                var rect = region.Bounds.ToPixels(imageWidth, imageHeight);
                if (!rect.Contains(pagePoint.X, pagePoint.Y))
                    continue;

                var area = rect.Width * rect.Height;
                if (area >= bestArea)
                    continue;

                hover = region;
                bestArea = area;
            }
        }

        if (ReferenceEquals(hover, _hover))
            return;

        _hover = hover;
        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _image?.Dispose();
            _bitmap?.Dispose();
        }

        base.Dispose(disposing);
    }
}
