using JpBookViewer.Books;
using JpBookViewer.View;

namespace JpBookViewer;

public partial class fMain : Form
{
    public bool ConvertToWeb;
    public Config C = new();
    public int KanjiViewMode;

    private readonly string? _initialPath;
    private Book? _book;
    private int _pageIndex;

    public fMain() : this(null)
    {
    }

    public fMain(string? path)
    {
        InitializeComponent();
        _initialPath = path;
    }

    private string? GetPath()
    {
        if (_initialPath != null)
            return _initialPath;

        return dFolder.ShowDialog(this) == DialogResult.OK ? dFolder.SelectedPath : null;
    }

    private async void fMain_Load(object? sender, EventArgs e)
    {
        var path = GetPath();
        if (string.IsNullOrWhiteSpace(path))
        {
            Close();
            return;
        }

        canvas.DisplayOptions = CreateDisplayOptions();
        await LoadBookAsync(path);
    }

    private AnnotationDisplayOptions CreateDisplayOptions()
    {
        return new AnnotationDisplayOptions
        {
            ShowAllRegions = C.ShowAllRegions,
            ShowLabels = C.ShowLabels,
            ShowConfidence = C.ShowConfidence,
            ShowMetadata = C.ShowMetadata,
            HintInCorner = KanjiViewMode == 1,
        };
    }

    private async Task LoadBookAsync(string path)
    {
        UseWaitCursor = true;
        Text = "Загрузка…";

        try
        {
            var book = await Task.Run(() => BookSourceFactory.Open(path));

            if (IsDisposed)
            {
                book.Dispose();
                return;
            }

            _book = book;

            if (_book.Pages.Count == 0)
            {
                MessageBox.Show(this, "В книге не найдено ни одной страницы.",
                    "Не удалось открыть книгу", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (C.BookID == _book.Name && C.PageNumber >= 0 && C.PageNumber < _book.Pages.Count)
                _pageIndex = C.PageNumber;

            Text = string.Equals(_book.Name, _book.Source, StringComparison.OrdinalIgnoreCase)
                ? _book.Name
                : $"{_book.Name} — {_book.Source}";

            if (ConvertToWeb)
                _book.ProcessToWeb();

            SetPage(_pageIndex);

            if (C.BookID != _book.Name)
            {
                C.BookID = _book.Name;
                C.Changed = true;
            }

            canvas.Focus();
        }
        catch (Exception ex)
        {
            if (IsDisposed)
                return;

            MessageBox.Show(this, ex.Message, "Не удалось открыть книгу",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
        finally
        {
            if (!IsDisposed)
                UseWaitCursor = false;
        }
    }

    private void SetPage(int index)
    {
        if (_book == null || index < 0 || index >= _book.Pages.Count)
            return;

        _pageIndex = index;

        if (C.PageNumber != index)
        {
            C.PageNumber = index;
            C.Changed = true;
        }

        canvas.SetPage(_book.Pages[index]);
        lPage.Text = $"{index + 1}/{_book.Pages.Count}";
        bNext.Enabled = index < _book.Pages.Count - 1;
        bBack.Enabled = index > 0;
        canvas.Focus();
    }

    private void bBack_Click(object? sender, EventArgs e)
    {
        SetPage(_pageIndex - 1);
    }

    private void bNext_Click(object? sender, EventArgs e)
    {
        SetPage(_pageIndex + 1);
    }

    private void fMain_KeyDown(object? sender, KeyEventArgs e)
    {
        if (_book == null)
            return;

        switch (e.KeyCode)
        {
            case Keys.Left:
            case Keys.PageUp:
                SetPage(_pageIndex - 1);
                e.Handled = true;
                break;
            case Keys.Right:
            case Keys.PageDown:
                SetPage(_pageIndex + 1);
                e.Handled = true;
                break;
            case Keys.Home:
                SetPage(0);
                e.Handled = true;
                break;
            case Keys.End:
                SetPage(_book.Pages.Count - 1);
                e.Handled = true;
                break;
        }
    }

    private void fMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (C.Changed)
            C.Save();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _book?.Dispose();
        _book = null;
        base.OnFormClosed(e);
    }
}
