using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace G3D.Controls
{
    abstract class Editor2dControl : GlControlS
    {
        Point LastPos;
        public Color Background
        {
            get { return S?.BackgroundColor ?? Color.Gray; }
            set { if (S != null) { S.BackgroundColor = value; } }
        }

        bool designMode = false;
        bool ActiveMove = false;
        bool BlockClick = false;
        public Scripts.Editor2d.BaseEditor2d S;


        public Editor2dControl()
        {
            designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        }

        protected abstract Scripts.Editor2d.BaseEditor2d InitEditorScript();

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!designMode)
            {
                S = InitEditorScript();
                S.Init();
                SetupViewport();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            SetupViewport();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            S?.Paint();
            SwapBuffers();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            LastPos = e.Location;
            ActiveMove = true;
            Cursor = Cursors.SizeAll;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            ActiveMove = false;
            Cursor = Cursors.Default;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!designMode)
            {
                const int WHEEL_DELTA = 120;
                int Ticks = Math.Abs(e.Delta / WHEEL_DELTA);
                bool Dir = e.Delta < 0;

                // Todo: Scale from org point
                for (int i = 0; i < Ticks; i++)
                {
                    if (!Dir)
                    {
                        if (S.Scale < 10.0f) S.UpdateScale(S.Scale * 1.1f, e.Location);
                    }
                    else
                    {
                        if (S.Scale > 0.21f) S.UpdateScale(S.Scale * 0.9f, e.Location);
                    }
                }

                if (Ticks > 0)
                {
                    S.UpdateCS();
                    Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (ActiveMove)
            {
                int dX = LastPos.X - e.Location.X;
                int dY = LastPos.Y - e.Location.Y;
                LastPos = e.Location;
                BlockClick = true;

                S.Move(dX, dY);
                S.UpdateCS();
                Invalidate();
            }
            else
            {
                S.SetActualCursor(new PointF(e.Location.X, e.Location.Y));
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!BlockClick)
            {
                S?.OnClick(e);
            }

            BlockClick = false;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            S?.OnDoubleClick(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            S?.Release();

            base.OnHandleDestroyed(e);
        }

        private void SetupViewport()
        {
            S?.InitViewPort(Width, Height);
        }

    }
}
