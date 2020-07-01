using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G3D.Text
{
    public class TextGenerator
    {
        private Font mFont;

        public TextGenerator(Font F)
        {
            mFont = F;
        }

        /// <summary>
        /// Измерить текст
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public Size getTextSize(string Text)
        {
            using (var image = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(image))
                {
                    var S = g.MeasureString(Text, mFont);
                    return new Size(Convert.ToInt32(S.Width), Convert.ToInt32(S.Height));
                }
            }
        }

        /// <summary>
        /// Получить картинку с нарисованным текстом
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public Bitmap getTextBitmap(string Text, Color C)
        {
            var S = getTextSize(Text);

            var B = new Bitmap(S.Width, S.Height);
            using (Graphics G = Graphics.FromImage(B))
            {
                G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                G.FillRectangle(new SolidBrush(Color.FromArgb(0, C)), new Rectangle(0, 0, S.Width, S.Height));
                G.DrawString(Text, mFont, new SolidBrush(C), 0, 0);
            }
            return B;
        }

        /// <summary>
        /// Получить картинку с нарисованным текстом
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public Bitmap getTextBitmap(string Text, Color C, Color O, float OutlineWidth)
        {
            var S = getTextSize(Text);

            var B = new Bitmap(Convert.ToInt32(S.Width + OutlineWidth*2), S.Height);
            using (Graphics G = Graphics.FromImage(B))
            {
                G.SmoothingMode = SmoothingMode.AntiAlias;
                G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                G.InterpolationMode = InterpolationMode.HighQualityBilinear;

                G.CompositingMode = CompositingMode.SourceCopy;
                G.FillRectangle(new SolidBrush(Color.FromArgb(0, O)), 0, 0, B.Width, B.Height);

                var P = new GraphicsPath();
                P.AddString(Text, mFont.FontFamily, (int)FontStyle.Regular, mFont.Height*0.91f, new Point(0, 0), new StringFormat());

                G.CompositingMode = CompositingMode.SourceOver;
                if(OutlineWidth > 0) G.DrawPath(new Pen(O, OutlineWidth), P);
                G.FillPath(new SolidBrush(C), P);
                P.Dispose();
            }
            return B;
        }
    }
}
