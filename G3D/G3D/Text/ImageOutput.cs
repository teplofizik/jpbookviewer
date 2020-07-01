using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using G3D.Texture;

namespace G3D.Text
{
    public class ImageOutput
    {
        int Width;
        int Height;

        public ImageOutput(int W, int H)
        {
            Width = W;
            Height = H;
        }

        /// <summary>
        /// Перейти в режим отображения текста
        /// </summary>
        public void Setup()
        {
            GL.MatrixMode(MatrixMode.Projection);

            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, Width, Height, 0, -100, 100);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.PushAttrib(AttribMask.TextureBit);
            GL.PushAttrib(AttribMask.LightingBit);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
        }

        /// <summary>
        /// Выйти из режима отображения текста
        /// </summary>
        public void Release()
        {
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.PopAttrib();
            GL.PopAttrib();

            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void Draw(int X, int Y, int Width, int Height, int TextureId)
        {
            Setup();
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.Color3(Color.White);
            
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex2(X, Y + Height);
            GL.TexCoord2(1, 1); GL.Vertex2(X + Width, Y + Height);
            GL.TexCoord2(1, 0); GL.Vertex2(X + Width, Y);
            GL.TexCoord2(0, 0); GL.Vertex2(X, Y);
            GL.End();
            Release();
        }

        public void Draw(int X, int Y, BitmapTexture Texture)
        {
            Draw(X, Y, Texture.Width, Texture.Height, Texture.Id);
        }

        public void Draw(int X, int Y, Bitmap Img)
        {
            var T = new Texture.BitmapTexture(Img);

            Draw(X, Y, Img.Width, Img.Height, T.Id);

            T.Release();
        }


        public void Draw(float X, float Y, int Width, int Height, int TextureId)
        {
            Setup();
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.Color3(Color.White);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 1); GL.Vertex2(X, Y + Height);
            GL.TexCoord2(1, 1); GL.Vertex2(X + Width, Y + Height);
            GL.TexCoord2(1, 0); GL.Vertex2(X + Width, Y);
            GL.TexCoord2(0, 0); GL.Vertex2(X, Y);
            GL.End();
            Release();
        }

        public void Draw(float X, float Y, Bitmap Img)
        {
            var T = new Texture.BitmapTexture(Img);

            Draw(X, Y, Img.Width, Img.Height, T.Id);

            T.Release();
        }

        public void Draw(float X, float Y, BitmapTexture Texture)
        {
            Draw(X, Y, Texture.Width, Texture.Height, Texture.Id);
        }
    }
}
