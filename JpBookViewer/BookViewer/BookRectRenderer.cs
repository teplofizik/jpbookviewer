using G3D.Texture;
using G3D.Texture.Generated;
using JefViewer.SewViewer.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace JefViewer.SewViewer
{
    class BookRectRenderer : BookRenderer
    {
        public List<ViewRect> Rects = new List<ViewRect>();
        public Page ActualPage = new Page();

        public Texture RectT = new TextureSolid(20, 20, Color.FromArgb(0, Color.White));
        public Texture HintT = new TextureSolid(20, 20, Color.FromArgb(50, Color.Red));
        //     public Texture RectA = new TextureSolid(50, 50, Color.FromArgb(200, Color.Blue));

        public TextureHint HintImage = new TextureHint(Color.White);

        public bool ShowHint = true;

        public string LastActiveText = "";

        public BookRectRenderer()
        {

        }

        public void SetPage(Page P)
        {
            ActualPage = P;
            SetImage(P.Image);

            foreach (var R in Rects)
            {
                R.Dispose();
            }

            Rects.Clear();
            foreach(var R in P.Rects)
            {
                Rects.Add(new ViewRect(R.Rect, R.Kanji, !ShowHint));
            }
        }

        private bool IsInRect(RectangleF R, PointF P)
        {
            return (P.X > R.Left) && (P.X < R.Right) && (P.Y > R.Top) && (P.Y < R.Bottom);
        }

        private void UpdateHint(string Text)
        {
            HintImage.SetText(Text);
        }

        private void DrawHint(string Text)
        {
            var VR = NavigationObject.ViewRectangle;

            var X = VR.Left + VR.Width * 0.02f;
            var Y = VR.Top + VR.Height * 0.02f;
            var W = 80 / NavigationObject.Scale;
            var H = W;
            var Rect = new RectangleF(X, Y, W, H);

            HintImage.SetText(Text);
            DrawRect(Rect, HintImage, 0.2f);
        }

        protected override void DrawOverlay()
        {
            base.DrawOverlay();

            string Last = "";
            foreach(var R in Rects)
            {
                if(IsInRect(R.Rect, MousePos))
                {
                    Last = R.Text;
                    MouseProcessed = true;
                    if(!ShowHint)
                        DrawRect(R.Rect, R.Texture, 0.4f);
                    else
                        DrawRect(R.Rect, HintT, 0.4f);
                }
                else
                {
                    DrawRect(R.Rect, RectT, 0.2f);
                }
            }
            LastActiveText = Last;

            if (Last.Length > 0)
            {
                if (ShowHint) DrawHint(Last);
            }
        }
    }
}
