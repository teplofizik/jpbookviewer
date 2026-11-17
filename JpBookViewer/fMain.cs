using JpBookViewer.Books;
using JpBookViewer.View;

namespace JpBookViewer;

public partial class fMain : Form
{
    public bool ConvertToWeb;
    public Config C = new();
    public int KanjiViewMode;

    private readonly string? _initialPath;
    private readonly ContextMenuStrip _viewContextMenu = new();
    private readonly ViewMenuItems _topMenuItems = new();
    private readonly ViewMenuItems _contextMenuItems = new();
    private Book? _book;
    private int _pageIndex;
    private bool _menusBuilt;

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
        BuildMenus();
        await LoadBookAsync(path);
    }

    private void BuildMenus()
    {
        if (_menusBuilt)
            return;

        _menusBuilt = true;

        BuildMenu(miView.DropDownItems, _topMenuItems);
        BuildMenu(_viewContextMenu.Items, _contextMenuItems);
        canvas.ContextMenuStrip = _viewContextMenu;

        SyncViewMenus();
    }

    private void BuildMenu(ToolStripItemCollection items, ViewMenuItems target)
    {
        target.ShowAllRegions = AddCheck(items, "Показывать все области",
            () => C.ShowAllRegions, value => C.ShowAllRegions = value);
        target.ShowLabels = AddCheck(items, "Показывать подписи",
            () => C.ShowLabels, value => C.ShowLabels = value);

        items.Add(new ToolStripSeparator());

        target.ShowConfidence = AddCheck(items, "Показывать уверенность",
            () => C.ShowConfidence, value => C.ShowConfidence = value);
        target.ShowMetadata = AddCheck(items, "Показывать метаданные",
            () => C.ShowMetadata, value => C.ShowMetadata = value);

        items.Add(new ToolStripSeparator());

        target.HintOverlay = new ToolStripMenuItem("Подсказка поверх текста");
        target.HintOverlay.Click += (_, _) => SetHintMode(false);
        target.HintCorner = new ToolStripMenuItem("Подсказка в углу");
        target.HintCorner.Click += (_, _) => SetHintMode(true);
        items.Add(target.HintOverlay);
        items.Add(target.HintCorner);

        items.Add(new ToolStripSeparator());

        var fitPage = new ToolStripMenuItem("Вписать страницу");
        fitPage.Click += (_, _) => canvas.FitToWindow();
        items.Add(fitPage);
    }

    private ToolStripMenuItem AddCheck(
        ToolStripItemCollection items,
        string text,
        Func<bool> getValue,
        Action<bool> setValue)
    {
        var item = new ToolStripMenuItem(text)
        {
            CheckOnClick = true,
            Checked = getValue(),
        };

        item.Click += (_, _) =>
        {
            setValue(item.Checked);
            SyncViewMenus();
            ApplyDisplayOptions();
        };

        items.Add(item);
        return item;
    }

    private void SyncViewMenus()
    {
        if (!_menusBuilt)
            return;

        Sync(_topMenuItems);
        Sync(_contextMenuItems);

        void Sync(ViewMenuItems items)
        {
            items.ShowAllRegions.Checked = C.ShowAllRegions;
            items.ShowLabels.Checked = C.ShowLabels;
            items.ShowConfidence.Checked = C.ShowConfidence;
            items.ShowMetadata.Checked = C.ShowMetadata;
            items.HintOverlay.Checked = KanjiViewMode != 1;
            items.HintCorner.Checked = KanjiViewMode == 1;
        }
    }

    private void SetHintMode(bool inCorner)
    {
        KanjiViewMode = inCorner ? 1 : 0;
        SyncViewMenus();
        ApplyDisplayOptions();
    }

    private void ApplyDisplayOptions()
    {
        canvas.DisplayOptions = CreateDisplayOptions();
        C.Changed = true;
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
        _viewContextMenu.Dispose();
        base.OnFormClosed(e);
    }

    private sealed class ViewMenuItems
    {
        public ToolStripMenuItem ShowAllRegions = null!;
        public ToolStripMenuItem ShowLabels = null!;
        public ToolStripMenuItem ShowConfidence = null!;
        public ToolStripMenuItem ShowMetadata = null!;
        public ToolStripMenuItem HintOverlay = null!;
        public ToolStripMenuItem HintCorner = null!;
    }
}
