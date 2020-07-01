using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace JefViewer.SewViewer
{
    class Navigation
    {
        public int PageWidth = 0;
        public int PageHeight = 0;

        public int ScreenWidth = 0;
        public int ScreenHeight = 0;

        public void SetScreen(int W, int H)
        {
            ScreenWidth = W;
            ScreenHeight = H;
        }

        public void SetPage(int W, int H)
        {
            PageWidth = W;
            PageHeight = H;
        }

        public float OffsetX = 0; // Смещение в исходных пикселях картинки вне зависимости от масштаба, x
        public float OffsetY = 0; // Смещение в исходных пикселях картинки вне зависимости от масштаба, y

        public float Scale = 1;

        public RectangleF ViewRectangle => new RectangleF(OffsetX * Scale, OffsetY * Scale, ScreenWidth / Scale, ScreenHeight / Scale);

        public PointF ConvertToPage(PointF Point)
        {
            var OrgPartX = Point.X * 1.0f / ScreenWidth;
            var OrgPartY = Point.Y * 1.0f / ScreenHeight;

            var VR = ViewRectangle;

            return new PointF(VR.Left + OrgPartX * VR.Width, VR.Top + OrgPartY * VR.Height);
        }
        
        private float LimitRange(float Value, float Min, float Max)
        {
            if (Value < Min) return Min;
            if (Value > Max) return Max;

            return Value;
        }

        private float MaxXLimit => Math.Max(0, PageWidth * Scale - ScreenWidth);
        private float MaxYLimit => Math.Max(0, PageHeight * Scale - ScreenHeight);

        public void Move(float dX, float dY)
        {
            var OX = OffsetX + dX / Scale / Scale;
            var OY = OffsetY + dY / Scale / Scale;

            OffsetX = OX;// LimitRange(OX, 0, MaxXLimit);
            OffsetY = OY;// LimitRange(OY, 0, MaxYLimit);
        }

        public void ChangeScale(float Value)
        {
            Scale = Value;
        }

        public void ChangeScale(float Value, PointF Org)
        {
            // Координата на листе, её надо будет поместить в точку Org
            var RealOrg = ConvertToPage(Org);
            var PartX = Org.X / ScreenWidth;
            var PartY = Org.Y / ScreenHeight;
            
            Scale = Value;

            var NewWidth = ScreenWidth / Scale;
            var NewHeight = ScreenHeight / Scale;

            var PageOffX = NewWidth * PartX;
            var PageOffY = NewHeight * PartY;

            OffsetX = (RealOrg.X - PageOffX) / Scale;
            OffsetY = (RealOrg.Y - PageOffY) / Scale;
        }
    }
}
