using G3D.Scripts;
using G3D.Scripts.Interface;
using JefViewer.SewViewer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JefViewer
{
    public partial class fMain : Form
    {
        public bool ConvertToWeb = false;

        public Config C;
        public int KanjiViewMode = 0;
        
        bool ActiveMove = false;
        Point LastPos;
        bool loaded = false;

        int Page = 0;
        string TestPath = null; //
        Book MyBook = new Book();

        SewViewer.BookWithMenuRenderer S;

        public fMain()
        {
            InitializeComponent();
        }

        public fMain(string Path)
        {
            InitializeComponent();
            TestPath = Path;
        }

        private string GetPath()
        {
            if (TestPath != null) return TestPath;
            if (dFolder.ShowDialog() == DialogResult.OK)
            {
                return dFolder.SelectedPath;
            }
            return null;
        }

        private void SetupOptions()
        {
            switch(KanjiViewMode)
            {
                case 0: S.ShowHint = false; break;
                case 1: S.ShowHint = true; break;
            }
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            loaded = true;
            S = new SewViewer.BookWithMenuRenderer();
            S.Init();
            SetupViewport();

            SetupOptions();

            var Path = GetPath();
            if(Path != null)
            { 
                // @"C:\Users\HPw\Documents\teplofizik\Android\Manyougana\Data\Meshihyaku\100249476"
                MyBook.Load(Path);
                if(ConvertToWeb) MyBook.ProcessToWeb();

                if (C.BookID.CompareTo(MyBook.Name) == 0)
                {
                    if((C.PageNumber >= 0) && (C.PageNumber < MyBook.Pages.Count))
                        Page = C.PageNumber;
                }
                if (MyBook.Pages.Count > 0)
                {
                    Text = MyBook.Name;
                    SetPage(Page);
                    C.BookID = MyBook.Name;
                }
            }
            else
                Close();

            CheckButtons();
        }

        private void fMain_Resize(object sender, EventArgs e)
        {
            SetupViewport();
        }

        private void glControlS1_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;
        }

        private void glControlS1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;

            S.Paint();
            glControlS1.SwapBuffers();
        }

        private void SetupViewport()
        {
            S.InitViewPort(glControlS1.Width, glControlS1.Height);
        }

        private void glControlS1_MouseDown(object sender, MouseEventArgs e)
        {
            LastPos = e.Location;
            ActiveMove = true;
            Cursor = Cursors.SizeAll;
        }

        private void glControlS1_MouseUp(object sender, MouseEventArgs e)
        {
            ActiveMove = false;
            Cursor = Cursors.Default;
        }

        private void glControlS1_MouseMove(object sender, MouseEventArgs e)
        {
            if (ActiveMove)
            {
                int dX = LastPos.X - e.Location.X;
                int dY = LastPos.Y - e.Location.Y;
                LastPos = e.Location;

                S.Move(dX, dY);
                S.UpdateCS();
                glControlS1.Invalidate();
            }
            else
            {
                S.SetActualCursor(new PointF(e.Location.X, e.Location.Y));
                glControlS1.Invalidate();
            }

           // var Pos = S.NavigationObject.ConvertToPage(new PointF(e.Location.X, e.Location.Y));
           // lPos.Text = $"{Pos.X}, {Pos.Y}";
        }
        
        private void GlControlS1_MouseWheel(object sender,MouseEventArgs e)
        {
            const int WHEEL_DELTA = 120;
            int Ticks = Math.Abs(e.Delta / WHEEL_DELTA);
            bool Dir = e.Delta < 0;
            
            // Todo: Scale from org point
            for(int i = 0; i < Ticks; i++) { 
                if(!Dir)
                {
                    if (S.Scale < 0.99f) S.UpdateScale(S.Scale + 0.1f, e.Location);
                }
                else
                {
                    if (S.Scale > 0.21f) S.UpdateScale(S.Scale - 0.1f, e.Location);
                }
            }

            if(Ticks > 0)
            {
                S.UpdateCS();
                glControlS1.Invalidate();
            }
        }

        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            S.Release();

            if (C.Changed) C.Save();
        }

        private void bTest_Click(object sender, EventArgs e)
        {
            //S.SetImage();
        }

        private void SetPage(int Page)
        {
            if ((Page >= 0) && (Page < MyBook.Pages.Count))
            {
                this.Page = Page;

                C.Changed |= Page != C.PageNumber;
                C.PageNumber = Page;
                S.SetPage(MyBook.Pages[Page]);
                S.UpdateCS();
                glControlS1.Invalidate();

                lPage.Text = $"{Page+1}/{MyBook.Pages.Count}";
            }

            CheckButtons();
            glControlS1.Focus();
        }

        private void CheckButtons()
        {
            bNext.Enabled = (Page < MyBook.Pages.Count - 1);
            bBack.Enabled = (Page > 0);
        }

        private void bBack_Click(object sender, EventArgs e)
        {
            SetPage(Page - 1);
        }

        private void bNext_Click(object sender, EventArgs e)
        {
            SetPage(Page + 1);
        }
    }
}
