using JefViewer.SewViewer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JefViewer
{
    public partial class fSelector : Form
    {
        Config C = Config.Load();

        public fSelector()
        {
            InitializeComponent();
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CheckButtonState()
        {
            bool Exists = Directory.Exists(tPath.Text);

            if (Exists)
            {
                Exists &= CodhRois.CheckIsRois(tPath.Text);
            }

            bOpen.Enabled = Exists;
        }

        private void tPath_TextChanged(object sender, EventArgs e)
        {
            CheckButtonState();
        }

        private void bSelectDir_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(tPath.Text))
                dFolder.SelectedPath = tPath.Text;

            if (dFolder.ShowDialog() == DialogResult.OK)
            {
                tPath.Text = dFolder.SelectedPath;
                CheckButtonState();
            }
        }

        private void fSelector_Load(object sender, EventArgs e)
        {
            tPath.Text = C.LastPath;
            cbKanjiViewMode.SelectedIndex = C.ViewMode;
            CheckButtonState();
        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            Hide();

            var M = new fMain(tPath.Text);
            M.Closed += (s, args) => this.Close();
            M.C = C;
            M.KanjiViewMode = cbKanjiViewMode.SelectedIndex;

            C.LastPath = tPath.Text;
            C.ViewMode = cbKanjiViewMode.SelectedIndex;
            C.Save();

            M.Show();
        }
    }
}
