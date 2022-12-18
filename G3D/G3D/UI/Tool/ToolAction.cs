using G3D.UI;
using G3D.UI.Editor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace G3D.Tool
{
    public class ToolAction
    {
        protected PointF MousePosition;

        public virtual void OnInit()
        {

        }

        public virtual void OnDeinit()
        {

        }

        public virtual void CheckUpdates(ref BaseDocument Doc)
        {

        }

        public virtual bool OnMouseDown(PointF Pos, MouseButtons Button)
        {
            return false;
        }

        public virtual void OnMouseUp(PointF Pos, MouseButtons Button)
        {

        }

        public virtual void OnMouseMove(PointF Pos, MouseButtons Button)
        {

        }

        public virtual bool OnClick(MouseEventArgs mouseEventArgs, PointF Pos)
        {
            return false;
        }

        public virtual void OnDraw(IGLDrawer Drawer)
        {

        }

        public virtual bool OnDoubleClick(MouseEventArgs mouseEventArgs, PointF pointF)
        {
            return false;
        }
    }
}
