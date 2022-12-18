using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.UI.Controls
{
    class CheckButtonControl : ButtonControl
    {
        public bool Checked = false;
        public Color CheckedColor = Color.White;

        public CheckButtonControl(PointF Pos, Bitmap Bmp) : base(Pos, Bmp)
        {

        }

        protected override void onDraw(IGLDrawer R)
        {
            base.onDraw(R);

            if (Checked)
            {
                R.IncrementdZ();
                R.LineWidth(3);
                R.DrawRect(Rectangle, CheckedColor);
            }
        }
    }
}
