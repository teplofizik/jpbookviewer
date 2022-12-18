using G3D.Texture.Generated;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.Texture
{
    class TextureHint : TextureSolid
    {
        const int BorderSize = 3;

        string LastText = "";

        Color Background;
        Brush TextBrush = Brushes.Black;
        Brush BorderBrush = Brushes.Black;

        public TextureHint(Color C) : base(100, 100, Color.FromArgb(127, C))
        {
            Background = C;
        }

        private Font GetFont(int Width)
        {
            return new Font(FontFamily.GenericSansSerif, Width * 70 / 100, GraphicsUnit.Pixel);
        }

        private void DrawBorder(Graphics G)
        {
            G.FillRectangle(BorderBrush, new RectangleF(0, 0, Width, BorderSize));
            G.FillRectangle(BorderBrush, new RectangleF(0, Height - BorderSize, Width, BorderSize));
            G.FillRectangle(BorderBrush, new RectangleF(0, BorderSize, BorderSize, Height - BorderSize * 2));
            G.FillRectangle(BorderBrush, new RectangleF(Width - BorderSize, BorderSize, BorderSize, Height - BorderSize * 2));
        }
        
        public void SetText(string Text)
        {
            if (LastText.CompareTo(Text) != 0)
            {
                LastText = Text;
                var G = BeginUpdate();
                {
                    G.Clear(Background);
                    DrawBorder(G);

                    var F = GetFont(Height);
                    var SS = G.MeasureString(Text, F);

                    float X = (Width - SS.Width) / 2;
                    float Y = 2 + (Height - SS.Height) / 2;

                    G.DrawString(Text, F, TextBrush, new PointF(X, Y));
                }
                EndUpdate();
            }
        }
    }
}
