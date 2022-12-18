using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace G3D.UI
{
    public class UIInterface
    {
        public List<UIControl> Controls = new List<UIControl>();

        public void AddControl(UIControl Control)
        {
            Controls.Add(Control);
        }

        // TODO : UIDrawer
        public void Draw(IGLDrawer Renderer)
        {
            foreach(var C in Controls)
            {
                C.Draw(Renderer);
            }
        }

        public bool OnClick(MouseEventArgs e)
        {
            foreach(var C in Controls)
            {
                if(C.Rectangle.Contains(e.Location))
                {
                    var ne = new MouseEventArgs(e.Button, e.Clicks, Convert.ToInt32(e.X - C.Rectangle.Left), Convert.ToInt32(e.Y - C.Rectangle.Top), e.Delta);
                    if (C.Click(ne))
                        return true;
                }
            }

            return false;
        }
    }
}
