using JpBookViewer.Books;

namespace JpBookViewer;

public partial class fMain : Form
{
    public bool ConvertToWeb;
    public Config C = new();
    public int KanjiViewMode;

    private readonly string? _initialPath;
    private readonly Book _book = new();
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

    private void fMain_Load(object? sender, EventArgs e)
    {
        var path = GetPath();
        if (string.IsNullOrWhiteSpace(path))
        {
            Close();
            return;
        }

        try
        {
            _book.Load(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Не удалось открыть книгу",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        if (_book.Pages.Count == 0)
        {
            MessageBox.Show(this, "В папке книги не найдено ни одной страницы.",
                "Не удалось открыть книгу", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        canvas.HintInCorner = KanjiViewMode == 1;

        if (C.BookID == _book.Name && C.PageNumber >= 0 && C.PageNumber < _book.Pages.Count)
            _pageIndex = C.PageNumber;

        Text = _book.Name;

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

    private void SetPage(int index)
    {
        if (index < 0 || index >= _book.Pages.Count)
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
}
