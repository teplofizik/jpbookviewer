using JpBookViewer.Books;

namespace JpBookViewer;

public partial class fSelector : Form
{
    private readonly Config _config = Config.Load();

    public fSelector()
    {
        InitializeComponent();
    }

    private void bClose_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void CheckButtonState()
    {
        bOpen.Enabled = BookSourceFactory.CanOpen(tPath.Text);
    }

    private void tPath_TextChanged(object? sender, EventArgs e)
    {
        CheckButtonState();
    }

    private void bSelectDir_Click(object? sender, EventArgs e)
    {
        if (Directory.Exists(tPath.Text))
            dFolder.SelectedPath = tPath.Text;

        if (dFolder.ShowDialog(this) == DialogResult.OK)
        {
            tPath.Text = dFolder.SelectedPath;
            CheckButtonState();
        }
    }

    private void bSelectArchive_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Выберите архив книги",
            Filter = "ZIP-архивы (*.zip)|*.zip|Все файлы (*.*)|*.*",
            CheckFileExists = true,
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            tPath.Text = dialog.FileName;
            CheckButtonState();
        }
    }

    private void fSelector_Load(object? sender, EventArgs e)
    {
        tPath.Text = _config.LastPath;
        cbKanjiViewMode.SelectedIndex = _config.ViewMode is >= 0 and <= 1 ? _config.ViewMode : 0;
        cbShowAllRegions.Checked = _config.ShowAllRegions;
        cbShowLabels.Checked = _config.ShowLabels;
        cbShowConfidence.Checked = _config.ShowConfidence;
        cbShowMetadata.Checked = _config.ShowMetadata;
        CheckButtonState();
    }

    private void bOpen_Click(object? sender, EventArgs e)
    {
        Hide();

        var main = new fMain(tPath.Text)
        {
            C = _config,
            KanjiViewMode = cbKanjiViewMode.SelectedIndex,
        };
        main.FormClosed += (_, _) => Close();

        _config.LastPath = tPath.Text;
        _config.ViewMode = cbKanjiViewMode.SelectedIndex;
        _config.ShowAllRegions = cbShowAllRegions.Checked;
        _config.ShowLabels = cbShowLabels.Checked;
        _config.ShowConfidence = cbShowConfidence.Checked;
        _config.ShowMetadata = cbShowMetadata.Checked;
        _config.Save();

        main.Show();
    }
}
