using G3D.Texture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.UI.Buffers
{
    class ImageTexture : TextureBuffer
    {
        public object Bmp;

        public ImageTexture(Bitmap Img) : base(new BitmapTexture(Img))
        {
            this.Bmp = Img;
        }

    }
}
