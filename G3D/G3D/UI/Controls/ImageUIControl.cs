using G3D.Texture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.UI.Controls
{
    class ImageUIControl : UIControl
    {
        Texture.Texture TI;

        public ImageUIControl(PointF Pos, Bitmap Bmp) : base(new RectangleF(Pos, Bmp.Size), true)
        {
            TI = new BitmapTexture(Bmp);
            Background = Color.White;
            ActiveBackground = Color.White;
        }

        protected override void onDraw(IGLDrawer R)
        {
            TI.Bind();
            base.onDraw(R);
            Texture.Texture.UnbindAll();
        }
    }
}
