using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G3D.Texture.Generated
{
    public class TextureSolid : BitmapTexture
    {
        public TextureSolid(int Width, int Height, Color C)
        {
            var B = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics G = Graphics.FromImage(B))
            {
                G.FillRectangle(new SolidBrush(C), new Rectangle(0, 0, Width, Height));
            }

            SetBitmap(B);
        }
    }
}
