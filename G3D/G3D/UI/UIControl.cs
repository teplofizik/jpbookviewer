using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace G3D.UI
{
    public class UIControl
    {
        public event MouseEventHandler OnMouseClick;
        public event EventHandler OnClick;

        public bool AbsoluteCS = true;
        public RectangleF Rectangle;
        public Color Background = Color.Transparent;
        public Color ActiveBackground = Color.Transparent;

        public DateTime LastClicked = DateTime.MinValue;

        public bool Visible = true;

        public UIControl(RectangleF Rect, bool AbsoluteCS)
        {
            this.AbsoluteCS = AbsoluteCS;
            Rectangle = Rect;
        }

        public void Draw(IGLDrawer Renderer)
        {
            if (Visible)
            {
                Renderer.PushRelative(!AbsoluteCS);
                onDraw(Renderer);
                Renderer.PopRelative();
            }
        }

        public bool Click(MouseEventArgs e)
        {
            LastClicked = DateTime.Now;
            if (onMouseClick(e))
                return true;
            if (onClick())
                return true;
            return false;
        }

        protected Color getBackgroundColor()
        {
            return ((DateTime.Now - LastClicked).TotalMilliseconds < 1000) ? ActiveBackground: Background;
        }

        protected virtual void onDraw(IGLDrawer R)
        {
            R.FillRect(Rectangle, getBackgroundColor());
        }

        protected virtual bool onMouseClick(MouseEventArgs e)
        {
            OnMouseClick?.Invoke(this, e);
            return false;
        }

        protected virtual bool onClick()
        {
            OnClick?.Invoke(this, new EventArgs());
            return true;
        }
    }
}
