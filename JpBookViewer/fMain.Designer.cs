namespace JefViewer
{
    partial class fMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControlS1 = new G3D.Controls.GlControlS();
            this.bNext = new System.Windows.Forms.Button();
            this.bBack = new System.Windows.Forms.Button();
            this.dFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.lPage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // glControlS1
            // 
            this.glControlS1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControlS1.BackColor = System.Drawing.Color.Black;
            this.glControlS1.Location = new System.Drawing.Point(12, 12);
            this.glControlS1.Name = "glControlS1";
            this.glControlS1.Size = new System.Drawing.Size(865, 400);
            this.glControlS1.TabIndex = 0;
            this.glControlS1.VSync = false;
            this.glControlS1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControlS1_Paint);
            this.glControlS1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControlS1_MouseDown);
            this.glControlS1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControlS1_MouseMove);
            this.glControlS1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControlS1_MouseUp);
            this.glControlS1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GlControlS1_MouseWheel);
            this.glControlS1.Resize += new System.EventHandler(this.glControlS1_Resize);
            // 
            // bNext
            // 
            this.bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bNext.Location = new System.Drawing.Point(12, 418);
            this.bNext.Name = "bNext";
            this.bNext.Size = new System.Drawing.Size(364, 23);
            this.bNext.TabIndex = 1;
            this.bNext.Text = "Дальше";
            this.bNext.UseVisualStyleBackColor = true;
            this.bNext.Click += new System.EventHandler(this.bNext_Click);
            // 
            // bBack
            // 
            this.bBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bBack.Location = new System.Drawing.Point(514, 418);
            this.bBack.Name = "bBack";
            this.bBack.Size = new System.Drawing.Size(364, 23);
            this.bBack.TabIndex = 2;
            this.bBack.Text = "Назад";
            this.bBack.UseVisualStyleBackColor = true;
            this.bBack.Click += new System.EventHandler(this.bBack_Click);
            // 
            // dFolder
            // 
            this.dFolder.Description = "Select folder with Codh.Rois data";
            // 
            // lPage
            // 
            this.lPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lPage.Location = new System.Drawing.Point(382, 418);
            this.lPage.Name = "lPage";
            this.lPage.Size = new System.Drawing.Size(126, 23);
            this.lPage.TabIndex = 3;
            this.lPage.Text = "0/0";
            this.lPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 450);
            this.Controls.Add(this.lPage);
            this.Controls.Add(this.bBack);
            this.Controls.Add(this.bNext);
            this.Controls.Add(this.glControlS1);
            this.Name = "fMain";
            this.Text = "Book viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fMain_FormClosing);
            this.Load += new System.EventHandler(this.fMain_Load);
            this.Resize += new System.EventHandler(this.fMain_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private G3D.Controls.GlControlS glControlS1;
        private System.Windows.Forms.Button bNext;
        private System.Windows.Forms.Button bBack;
        private System.Windows.Forms.FolderBrowserDialog dFolder;
        private System.Windows.Forms.Label lPage;
    }
}

