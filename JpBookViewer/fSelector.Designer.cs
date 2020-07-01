namespace JefViewer
{
    partial class fSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bClose = new System.Windows.Forms.Button();
            this.bOpen = new System.Windows.Forms.Button();
            this.tPath = new System.Windows.Forms.TextBox();
            this.bSelectDir = new System.Windows.Forms.Button();
            this.dFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.lKanjiViewMode = new System.Windows.Forms.Label();
            this.cbKanjiViewMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // bClose
            // 
            this.bClose.Location = new System.Drawing.Point(12, 63);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(144, 23);
            this.bClose.TabIndex = 0;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // bOpen
            // 
            this.bOpen.Location = new System.Drawing.Point(362, 63);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(144, 23);
            this.bOpen.TabIndex = 1;
            this.bOpen.Text = "Open";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // tPath
            // 
            this.tPath.Location = new System.Drawing.Point(12, 12);
            this.tPath.Name = "tPath";
            this.tPath.Size = new System.Drawing.Size(459, 20);
            this.tPath.TabIndex = 2;
            this.tPath.TextChanged += new System.EventHandler(this.tPath_TextChanged);
            // 
            // bSelectDir
            // 
            this.bSelectDir.Location = new System.Drawing.Point(477, 10);
            this.bSelectDir.Name = "bSelectDir";
            this.bSelectDir.Size = new System.Drawing.Size(29, 23);
            this.bSelectDir.TabIndex = 3;
            this.bSelectDir.Text = "...";
            this.bSelectDir.UseVisualStyleBackColor = true;
            this.bSelectDir.Click += new System.EventHandler(this.bSelectDir_Click);
            // 
            // dFolder
            // 
            this.dFolder.Description = "Select folder with Codh.Rois data";
            // 
            // lKanjiViewMode
            // 
            this.lKanjiViewMode.AutoSize = true;
            this.lKanjiViewMode.Location = new System.Drawing.Point(12, 41);
            this.lKanjiViewMode.Name = "lKanjiViewMode";
            this.lKanjiViewMode.Size = new System.Drawing.Size(115, 13);
            this.lKanjiViewMode.TabIndex = 4;
            this.lKanjiViewMode.Text = "Режим отображения:";
            // 
            // cbKanjiViewMode
            // 
            this.cbKanjiViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbKanjiViewMode.FormattingEnabled = true;
            this.cbKanjiViewMode.Items.AddRange(new object[] {
            "Поверх текста",
            "В квадратике в углу, в тексте отмечается только область "});
            this.cbKanjiViewMode.Location = new System.Drawing.Point(133, 38);
            this.cbKanjiViewMode.Name = "cbKanjiViewMode";
            this.cbKanjiViewMode.Size = new System.Drawing.Size(338, 21);
            this.cbKanjiViewMode.TabIndex = 5;
            // 
            // fSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 98);
            this.Controls.Add(this.cbKanjiViewMode);
            this.Controls.Add(this.lKanjiViewMode);
            this.Controls.Add(this.bSelectDir);
            this.Controls.Add(this.tPath);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.bClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "fSelector";
            this.Text = "Select codh.rois book directory";
            this.Load += new System.EventHandler(this.fSelector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.TextBox tPath;
        private System.Windows.Forms.Button bSelectDir;
        private System.Windows.Forms.FolderBrowserDialog dFolder;
        private System.Windows.Forms.Label lKanjiViewMode;
        private System.Windows.Forms.ComboBox cbKanjiViewMode;
    }
}