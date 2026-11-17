#nullable disable

namespace JpBookViewer
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
            this.bSelectArchive = new System.Windows.Forms.Button();
            this.dFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.lKanjiViewMode = new System.Windows.Forms.Label();
            this.cbKanjiViewMode = new System.Windows.Forms.ComboBox();
            this.gbDisplay = new System.Windows.Forms.GroupBox();
            this.cbShowAllRegions = new System.Windows.Forms.CheckBox();
            this.cbShowLabels = new System.Windows.Forms.CheckBox();
            this.cbShowConfidence = new System.Windows.Forms.CheckBox();
            this.cbShowMetadata = new System.Windows.Forms.CheckBox();
            this.gbDisplay.SuspendLayout();
            this.SuspendLayout();
            //
            // bClose
            //
            this.bClose.Location = new System.Drawing.Point(12, 170);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(144, 30);
            this.bClose.TabIndex = 0;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            //
            // bOpen
            //
            this.bOpen.Location = new System.Drawing.Point(362, 170);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(144, 30);
            this.bOpen.TabIndex = 1;
            this.bOpen.Text = "Open";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            //
            // tPath
            //
            this.tPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tPath.Location = new System.Drawing.Point(12, 12);
            this.tPath.Name = "tPath";
            this.tPath.Size = new System.Drawing.Size(429, 23);
            this.tPath.TabIndex = 2;
            this.tPath.TextChanged += new System.EventHandler(this.tPath_TextChanged);
            //
            // bSelectDir
            //
            this.bSelectDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSelectDir.Location = new System.Drawing.Point(447, 11);
            this.bSelectDir.Name = "bSelectDir";
            this.bSelectDir.Size = new System.Drawing.Size(29, 25);
            this.bSelectDir.TabIndex = 3;
            this.bSelectDir.Text = "...";
            this.bSelectDir.UseVisualStyleBackColor = true;
            this.bSelectDir.Click += new System.EventHandler(this.bSelectDir_Click);
            //
            // bSelectArchive
            //
            this.bSelectArchive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSelectArchive.Location = new System.Drawing.Point(477, 11);
            this.bSelectArchive.Name = "bSelectArchive";
            this.bSelectArchive.Size = new System.Drawing.Size(29, 25);
            this.bSelectArchive.TabIndex = 4;
            this.bSelectArchive.Text = "ZIP";
            this.bSelectArchive.UseVisualStyleBackColor = true;
            this.bSelectArchive.Click += new System.EventHandler(this.bSelectArchive_Click);
            //
            // dFolder
            //
            this.dFolder.Description = "Select folder with a book";
            this.dFolder.UseDescriptionForTitle = true;
            //
            // lKanjiViewMode
            //
            this.lKanjiViewMode.AutoSize = true;
            this.lKanjiViewMode.Location = new System.Drawing.Point(12, 44);
            this.lKanjiViewMode.Name = "lKanjiViewMode";
            this.lKanjiViewMode.Size = new System.Drawing.Size(112, 15);
            this.lKanjiViewMode.TabIndex = 4;
            this.lKanjiViewMode.Text = "Режим подсказки:";
            //
            // cbKanjiViewMode
            //
            this.cbKanjiViewMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbKanjiViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbKanjiViewMode.FormattingEnabled = true;
            this.cbKanjiViewMode.Items.AddRange(new object[] {
            "Поверх текста",
            "В квадратике в углу"});
            this.cbKanjiViewMode.Location = new System.Drawing.Point(133, 41);
            this.cbKanjiViewMode.Name = "cbKanjiViewMode";
            this.cbKanjiViewMode.Size = new System.Drawing.Size(373, 23);
            this.cbKanjiViewMode.TabIndex = 5;
            //
            // gbDisplay
            //
            this.gbDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDisplay.Controls.Add(this.cbShowAllRegions);
            this.gbDisplay.Controls.Add(this.cbShowLabels);
            this.gbDisplay.Controls.Add(this.cbShowConfidence);
            this.gbDisplay.Controls.Add(this.cbShowMetadata);
            this.gbDisplay.Location = new System.Drawing.Point(12, 78);
            this.gbDisplay.Name = "gbDisplay";
            this.gbDisplay.Size = new System.Drawing.Size(494, 80);
            this.gbDisplay.TabIndex = 6;
            this.gbDisplay.TabStop = false;
            this.gbDisplay.Text = "Отображение";
            //
            // cbShowAllRegions
            //
            this.cbShowAllRegions.AutoSize = true;
            this.cbShowAllRegions.Location = new System.Drawing.Point(12, 26);
            this.cbShowAllRegions.Name = "cbShowAllRegions";
            this.cbShowAllRegions.Size = new System.Drawing.Size(181, 19);
            this.cbShowAllRegions.TabIndex = 0;
            this.cbShowAllRegions.Text = "Показывать все области";
            this.cbShowAllRegions.UseVisualStyleBackColor = true;
            //
            // cbShowLabels
            //
            this.cbShowLabels.AutoSize = true;
            this.cbShowLabels.Location = new System.Drawing.Point(12, 50);
            this.cbShowLabels.Name = "cbShowLabels";
            this.cbShowLabels.Size = new System.Drawing.Size(216, 19);
            this.cbShowLabels.TabIndex = 1;
            this.cbShowLabels.Text = "Показывать текст во всех областях";
            this.cbShowLabels.UseVisualStyleBackColor = true;
            //
            // cbShowConfidence
            //
            this.cbShowConfidence.AutoSize = true;
            this.cbShowConfidence.Checked = true;
            this.cbShowConfidence.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowConfidence.Location = new System.Drawing.Point(250, 26);
            this.cbShowConfidence.Name = "cbShowConfidence";
            this.cbShowConfidence.Size = new System.Drawing.Size(172, 19);
            this.cbShowConfidence.TabIndex = 2;
            this.cbShowConfidence.Text = "Показывать уверенность";
            this.cbShowConfidence.UseVisualStyleBackColor = true;
            //
            // cbShowMetadata
            //
            this.cbShowMetadata.AutoSize = true;
            this.cbShowMetadata.Checked = true;
            this.cbShowMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowMetadata.Location = new System.Drawing.Point(250, 50);
            this.cbShowMetadata.Name = "cbShowMetadata";
            this.cbShowMetadata.Size = new System.Drawing.Size(168, 19);
            this.cbShowMetadata.TabIndex = 3;
            this.cbShowMetadata.Text = "Показывать метаданные";
            this.cbShowMetadata.UseVisualStyleBackColor = true;
            //
            // fSelector
            //
            this.AcceptButton = this.bOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 212);
            this.Controls.Add(this.gbDisplay);
            this.Controls.Add(this.cbKanjiViewMode);
            this.Controls.Add(this.lKanjiViewMode);
            this.Controls.Add(this.bSelectArchive);
            this.Controls.Add(this.bSelectDir);
            this.Controls.Add(this.tPath);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.bClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "fSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JP Book Viewer";
            this.Load += new System.EventHandler(this.fSelector_Load);
            this.gbDisplay.ResumeLayout(false);
            this.gbDisplay.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.TextBox tPath;
        private System.Windows.Forms.Button bSelectDir;
        private System.Windows.Forms.Button bSelectArchive;
        private System.Windows.Forms.FolderBrowserDialog dFolder;
        private System.Windows.Forms.Label lKanjiViewMode;
        private System.Windows.Forms.ComboBox cbKanjiViewMode;
        private System.Windows.Forms.GroupBox gbDisplay;
        private System.Windows.Forms.CheckBox cbShowAllRegions;
        private System.Windows.Forms.CheckBox cbShowLabels;
        private System.Windows.Forms.CheckBox cbShowConfidence;
        private System.Windows.Forms.CheckBox cbShowMetadata;
    }
}
