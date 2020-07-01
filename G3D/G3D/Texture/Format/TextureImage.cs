using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace G3D.Texture.Format
{
    public class TextureImage : BitmapTexture
    {
        public TextureImage(string FileName)
        {
            var B = new Bitmap(FileName);

            SetBitmap(B);
        }
    }
}
