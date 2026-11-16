using JpBookViewer.Books;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace JpBookViewer.View;

public sealed class BookCanvas : SKControl
{
    private const float MinScale = 0.05f;
    private const float MaxScale = 16f;

    private static readonly SKSamplingOptions Sampling =
        new(SKFilterMode.Linear, SKMipmapMode.Linear);

    private static readonly Lazy<SKTypeface> JapaneseTypeface = new(() =>
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

    private SKBitmap? _bitmap;
    private SKImage? _image;
    private Page? _page;
    private PageRect? _hover;

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

    private bool _hintInCorner;

    public BookCanvas()
    {
        BackColor = Color.Black;
        TabStop = true;
        SetStyle(ControlStyles.Selectable, true);
        PaintSurface += OnPaintSurface;
    }

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public bool HintInCorner
    {
        get => _hintInCorner;
        set
        {
            if (_hintInCorner == value)
                return;

            _hintInCorner = value;
            Invalidate();
        }
    }

    public void SetPage(Page page)
    {
        _page = page;
        _hover = null;

        _image?.Dispose();
        _image = null;
        _bitmap?.Dispose();
        _bitmap = null;

        try
        {
            _bitmap = SKBitmap.Decode(page.Image);
        }
        catch (Exception)
        {
            _bitmap = null;
        }

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
        DrawHoveredRect(canvas);
        canvas.Restore();

        DrawCornerHint(canvas);
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

    private void DrawHoveredRect(SKCanvas canvas)
    {
        if (_hover == null)
            return;

        var rect = _hover.Rect;

        using var fill = new SKPaint
        {
            Color = new SKColor(255, 0, 0, 50),
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };
        canvas.DrawRect(rect, fill);

        if (_hintInCorner)
        {
            using var stroke = new SKPaint
            {
                Color = new SKColor(255, 64, 64, 220),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f / _scale,
                IsAntialias = true,
            };
            canvas.DrawRect(rect, stroke);
        }
        else
        {
            DrawGlyph(canvas, _hover.Kanji, rect, 0.9f);
        }
    }

    private void DrawCornerHint(SKCanvas canvas)
    {
        if (!_hintInCorner || _hover == null || string.IsNullOrEmpty(_hover.Kanji))
            return;

        var ui = _surfaceScaleX > 0 ? _surfaceScaleX : 1f;
        var panel = 96f * ui;
        var margin = 12f * ui;
        var rect = new SKRect(margin, margin, margin + panel, margin + panel);

        using var background = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 235),
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
        };
        using var border = new SKPaint
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f * ui,
            IsAntialias = true,
        };

        canvas.DrawRoundRect(rect, 6f * ui, 6f * ui, background);
        canvas.DrawRoundRect(rect, 6f * ui, 6f * ui, border);

        var inner = new SKRect(
            rect.Left + 6f * ui,
            rect.Top + 6f * ui,
            rect.Right - 6f * ui,
            rect.Bottom - 6f * ui);

        DrawGlyph(canvas, _hover.Kanji, inner, 0.82f);
    }

    private static void DrawGlyph(SKCanvas canvas, string text, SKRect area, float factor)
    {
        if (string.IsNullOrEmpty(text) || area.Width <= 0 || area.Height <= 0)
            return;

        var maxSize = MathF.Min(area.Width, area.Height) * factor;
        if (maxSize <= 0)
            return;

        using var font = new SKFont(JapaneseTypeface.Value, maxSize);
        var bounds = new SKRect();
        var width = font.MeasureText(text, out bounds);

        var maxWidth = area.Width * factor;
        if (width > maxWidth && width > 0)
        {
            font.Size = maxSize * maxWidth / width;
            font.MeasureText(text, out bounds);
        }

        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
        };

        canvas.DrawText(text, area.MidX - bounds.MidX, area.MidY - bounds.MidY, font, paint);
    }

    private void DrawEmptyState(SKCanvas canvas)
    {
        var ui = _surfaceScaleX > 0 ? _surfaceScaleX : 1f;
        using var font = new SKFont(SKTypeface.Default, 16f * ui);
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
        PageRect? hover = null;

        if (_page != null)
        {
            foreach (var rect in _page.Rects)
            {
                if (!rect.Rect.Contains(pagePoint.X, pagePoint.Y))
                    continue;

                hover = rect;
                break;
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
