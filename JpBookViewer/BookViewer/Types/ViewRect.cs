using G3D.Texture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace JefViewer.SewViewer.Types
{
    class ViewRect : IDisposable
    {
        public string Text;
        public RectangleF Rect;

        public Brush TextBrush;

        public BitmapTexture Texture;

        public ViewRect(Rectangle R, string Text, bool GenTex)
        {
            TextBrush = Brushes.Black;
            Rect = new RectangleF(R.X, R.Y, R.Width, R.Height);
            this.Text = Text;

            if(GenTex)
                GenerateTexture();
        }

        private Font GetFont(float Width)
        {
            return new Font(FontFamily.GenericSansSerif, Width * 90 / 100, GraphicsUnit.Pixel);
        }

        private void RedrawTexture()
        {
            var Size = Math.Min(Rect.Width, Rect.Height);

            using (Graphics G = Texture.BeginUpdate())
            {
                G.Clear(Color.FromArgb(35, Color.Red));

                var F = GetFont(Rect.Width > Rect.Height ? Size * 0.9f : Size);
                var SS = G.MeasureString(Text, F);

                float X = (Rect.Width - SS.Width) / 2;
                float Y = (Rect.Height - SS.Height) / 2;

                G.DrawString(Text, F, TextBrush, new PointF(X, Y));
            }

            Texture.EndUpdate();
        }


        private void GenerateTexture()
        {
            var Size = Math.Min(Rect.Width, Rect.Height);

            var Bitmap = new Bitmap(Convert.ToInt32(Rect.Width), Convert.ToInt32(Rect.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics G = Graphics.FromImage(Bitmap))
            {
                G.Clear(Color.FromArgb(35, Color.Red));

                var F = GetFont(Rect.Width > Rect.Height ? Size * 0.9f : Size);
                var SS = G.MeasureString(Text, F);

                float X = (Bitmap.Width - SS.Width) / 2;
                float Y = (Bitmap.Height - SS.Height) / 2;

                G.DrawString(Text, F, Brushes.Black, new PointF(X, Y));
            }

            Texture = new BitmapTexture(Bitmap);
        }

        public void Dispose()
        {
            if (Texture != null)
            {
                Texture.Release();
                Texture.Dispose();
            }
        }
    }
}
