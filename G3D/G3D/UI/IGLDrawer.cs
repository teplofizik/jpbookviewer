using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using G3D.Texture;
using G3D.UI.Buffers;
using OpenTK.Graphics.OpenGL;

namespace G3D.UI
{
    public class IGLDrawer
    {
        private static object Locker = new object();

        public int Width;
        public int Height;

        static List<ImageTexture> Images = new List<ImageTexture>();
        static List<StringTexture> Strings = new List<StringTexture>();

        protected Stack<bool> RelativeStack = new Stack<bool>();
        protected Stack<float> ZStack = new Stack<float>();
        protected List<float> dZStack = new List<float>();

        protected float ConvertX(float X) => Relative ? X * Width : X;
        protected float ConvertY(float Y) => Relative ? Y * Height : Y;


        public void PushRelative(bool state) { RelativeStack.Push(state); }
        public void PopRelative() { RelativeStack.Pop(); }
        public bool Relative => (RelativeStack.Count > 0) ? RelativeStack.Last() : false;

        public void IncrementdZ()
        {
            dZStack[dZStack.Count - 1] += 0.000001f;
        }
        public void PushZ(float Z) { ZStack.Push(Z); dZStack.Add(0); }
        public void PopZ() { ZStack.Pop(); dZStack.RemoveAt(dZStack.Count - 1); }
        public float Z => (ZStack.Count > 0) ? ZStack.Last() + dZStack.Last() : 0;

        public IGLDrawer(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            PushZ(0);

            ProcessTextures();
        }

        public static void ProcessTextures()
        {
            lock (Locker)
            {
                Images.ForEach(T => T.Tick());
                Strings.ForEach(T => T.Tick());

                Images.RemoveAll(T => T.ToRemove);
                Strings.RemoveAll(T => T.ToRemove);
            }
            //   Debug.WriteLine($"Images::Count = {Images.Count}, Strings::Count = {Strings.Count}");
        }

        public void LineWidth(float Size)
        {
            GL.LineWidth(Size);
        }

        public void TranslateZ()
        {
            GL.Translate(0, 0, -Z);
        }

        public void DrawPoint(PointF P, Color C)
        {
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(0, 0, -Z);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(P.X, P.Y, 0);
            GL.End();
            GL.PopMatrix();
        }

        internal void Color(Color C)
        {
            GL.Color4(C);
        }

        public void DrawLine(PointF From, PointF To, Color C)
        {
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(0, 0, -Z);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(From.X, From.Y, 0);
            GL.Vertex3(To.X, To.Y, 0);
            GL.End();
            GL.PopMatrix();
        }

        public void DrawLine(PointF[] Points, Color C)
        {
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(0, 0, -Z);
            GL.Begin(PrimitiveType.LineStrip);
            foreach(var P in Points)
                GL.Vertex3(P.X, P.Y, 0);
            GL.End();
            GL.PopMatrix();
        }


        protected void Arc(PointF org, SizeF Size, float fromAngle, float ToAngle)
        {
            GL.Vertex3(org.X, org.Y, 0);
            for (float angle = fromAngle; angle < ToAngle; angle += 3.1415926f / 180)
            {
                float x = Convert.ToSingle(Size.Width * Math.Cos(angle));//calculate the x component 
                float y = Convert.ToSingle(Size.Height * Math.Sin(angle));//calculate the y component 

                GL.Vertex3(org.X + x, org.Y + y, 0);//output vertex 
            }
        }

        public void FillArc(PointF org, SizeF Size, float fromAngle, float ToAngle, Color C)
        {
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(0, 0, -Z);
            GL.Begin(PrimitiveType.Polygon);
            Arc(org, Size, fromAngle, ToAngle);
            GL.End();
            GL.PopMatrix();
        }

        public void DrawArc(PointF org, SizeF Size, float fromAngle, float ToAngle, Color C)
        {
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(0, 0, -Z);
            GL.Begin(PrimitiveType.LineLoop);
            Arc(org, Size, fromAngle, ToAngle);
            GL.End();
            GL.PopMatrix();
        }

        protected void Rect(RectangleF Rect, bool Texture = false)
        {
            GL.Normal3(0, -1, 0);
            if(Texture) GL.TexCoord2(1, 1);
            GL.Vertex3(ConvertX(Rect.Width), ConvertY(Rect.Height), 0);
            if (Texture) GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);
            if (Texture) GL.TexCoord2(0, 1);
            GL.Vertex3(0, ConvertY(Rect.Height), 0);
            if (Texture) GL.TexCoord2(1, 1);
            GL.Vertex3(ConvertX(Rect.Width), ConvertY(Rect.Height), 0);
            if (Texture) GL.TexCoord2(1, 0);
            GL.Vertex3(ConvertX(Rect.Width), 0, 0);
            if (Texture) GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);
        }

        public void PointSize(int v)
        {
            GL.PointSize(v);
        }

        public void DrawRect(RectangleF Rect, Color C)
        {
            Texture.Texture.UnbindAll();
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(ConvertX(Rect.Left), ConvertY(Rect.Top), -Z);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, ConvertY(Rect.Height), 0);
            GL.Vertex3(ConvertX(Rect.Width), ConvertY(Rect.Height), 0);
            GL.Vertex3(ConvertX(Rect.Width), 0, 0);
            GL.End();
            GL.PopMatrix();
        }

        public void FillRect(RectangleF Rect, Color C)
        {
            Texture.Texture.UnbindAll();
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(ConvertX(Rect.Left), ConvertY(Rect.Top), -Z);
            GL.Begin(PrimitiveType.Triangles);
            this.Rect(Rect);
            GL.End();
            GL.PopMatrix();
        }

        public void FillRect(RectangleF Rect, TextureBuffer T)
        {
            T.Bind();

            GL.PushMatrix();
            GL.Translate(ConvertX(Rect.Left), ConvertY(Rect.Top), -Z);
            GL.Begin(PrimitiveType.Triangles);
            this.Rect(Rect, true);
            GL.End();
            GL.PopMatrix();
        }

        public void FillRect(RectangleF Rect, Texture.Texture T)
        {
            T.Bind();

            GL.PushMatrix();
            GL.Translate(ConvertX(Rect.Left), ConvertY(Rect.Top), -Z);
            GL.Begin(PrimitiveType.Triangles);
            this.Rect(Rect, true);
            GL.End();
            GL.PopMatrix();
        }

        public void EnableStipple(bool v)
        {
            if (v)
                GL.Enable(EnableCap.LineStipple);
            else
                GL.Disable(EnableCap.LineStipple);
        }

        public void LineStipple(int Factor, short Pattern)
        {
            GL.LineStipple(Factor, Pattern);
        }

        public SizeF MeasureString(string Text, Font F)
        {
            Bitmap Bmp = new Bitmap(10, 10);
            using (var G = Graphics.FromImage(Bmp))
            {
                return G.MeasureString(Text, F);
            }
        }

        public void DrawString(string Text, Font F, Color C, SizeF Sz, RectangleF layoutRectangle, StringFormat SF)
        {
            var S = GetString(Text, F, C, new RectangleF(0, 0, Sz.Width, Sz.Height), SF);
            FillRect(layoutRectangle, S);
        }

        public void DrawString(string Text, Font F, Color C, PointF org)
        {
            var S = GetString(Text, F, C);
            FillRect(new RectangleF(org.X, org.Y, (S.T as BitmapTexture).Width, (S.T as BitmapTexture).Height), S);
        }

        public void DrawImage(Bitmap B, Point P)
        {
            var T = GetImage(B);
            FillRect(new RectangleF(P.X, P.Y, B.Width, B.Height), T);
        }

        private TextureBuffer GetImage(Bitmap B)
        {
            lock (Locker)
            {
                var limg = Images.ToArray();

                foreach (var Img in limg)
                {
                    // Debug.WriteLine($"Bmp == Img: {B.Equals(Img.Bmp)}");

                    if (Img != null)
                    {
                        if (B.Equals(Img.Bmp)) return Img;
                    }
                }

                var I = new ImageTexture(B);
                Images.Add(I);
                //      Debug.WriteLine($"Count: {Images.Count}");
                return I;
            }
        }

        private TextureBuffer GetString(string S, Font F, Color C)
        {
            lock (Locker)
            {
                var lstr = Strings.ToArray();

                foreach (var Str in lstr)
                {
                    if (Str != null)
                    {
                        if ((Str.Text.CompareTo(S) == 0) && (Str.F == F) && (Str.Clr == C)) return Str;
                    }
                }

                var LF = new Font(F.FontFamily, F.Size, FontStyle.Regular);

                var Sz = MeasureString(S, F);
                var Bmp = new Bitmap(Convert.ToInt32(Sz.Width + 0.99f), Convert.ToInt32(Sz.Height + 0.99f), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics G = Graphics.FromImage(Bmp))
                {
                    G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    G.Clear(System.Drawing.Color.White);
                    G.DrawString(S, F, new SolidBrush(C), 0, 0);
                }
                var T = new BitmapTexture(Bmp);

                var NS = new StringTexture(S, F, C, T);
                Strings.Add(NS);

                return NS;
            }
        }

        private TextureBuffer GetString(string S, Font F, Color C, RectangleF layoutRectangle, StringFormat SF)
        {
            lock (Locker)
            {
                foreach (var Str in Strings)
                {
                    if (Str.Text.CompareTo(S) == 0)
                    {
                        if (Str.F.Equals(F) && (Str.Clr == C))
                            return Str;
                    }
                }

                var Bmp = new Bitmap(Convert.ToInt32(layoutRectangle.Width + 0.99f), Convert.ToInt32(layoutRectangle.Height + 0.99f), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics G = Graphics.FromImage(Bmp))
                {
                    G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    G.Clear(System.Drawing.Color.White);
                    G.DrawString(S, F, new SolidBrush(C), new RectangleF(0, 0, layoutRectangle.Width, layoutRectangle.Height), SF);
                }
                var T = new BitmapTexture(Bmp);

                var NS = new StringTexture(S, F, C, T);
                Strings.Add(NS);

                return NS;
            }
        }
    }
}
