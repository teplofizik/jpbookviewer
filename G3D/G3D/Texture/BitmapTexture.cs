using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace G3D.Texture
{
    public class BitmapTexture : Texture
    {
        protected Bitmap B;

        public BitmapTexture()
        {

        }

        public BitmapTexture(Bitmap B)
        {
            SetBitmap(B);
        }

        protected void SetBitmap(Bitmap _B)
        {
            B = _B;
            Create();
        }

        public Graphics BeginUpdate()
        {
            return Graphics.FromImage(B);
        }

        public void EndUpdate()
        {
            Bind();

            BitmapData bmp_data = B.LockBits(new Rectangle(0, 0, B.Width, B.Height),
                                             ImageLockMode.ReadOnly,
                                             System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexSubImage2D(TextureTarget.Texture2D,
                             0,
                             0,
                             0,
                             bmp_data.Width,
                             bmp_data.Height,
                             OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                             PixelType.UnsignedByte,
                             bmp_data.Scan0);

            B.UnlockBits(bmp_data);
        }

        protected override void TexImage()
        {
            BitmapData bmp_data = B.LockBits(new Rectangle(0, 0, B.Width, B.Height),
                                             ImageLockMode.ReadOnly,
                                             System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D,
                          0,
                          PixelInternalFormat.Rgba,
                          bmp_data.Width,
                          bmp_data.Height,
                          0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                          PixelType.UnsignedByte,
                          bmp_data.Scan0);

            B.UnlockBits(bmp_data);
        }

        public override void Dispose()
        {
            base.Dispose();

            B.Dispose();
        }

        /// <summary>
        /// Ширина
        /// </summary>
        public int Width
        {
            get { return B.Width; }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height
        {
            get { return B.Height; }
        }
    }
}
