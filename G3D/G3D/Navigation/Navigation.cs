using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.Navigation
{
    public class Navigation
    {
        public float PageWidth = 0;
        public float PageHeight = 0;

        public int ScreenWidth = 0;
        public int ScreenHeight = 0;

        RectangleF Object = RectangleF.Empty;

        public float OffsetX = 0; // Смещение в исходных пикселях картинки вне зависимости от масштаба, x
        public float OffsetY = 0; // Смещение в исходных пикселях картинки вне зависимости от масштаба, y

        public float Scale = 1;

        protected RefNavigation RefObject = new RefNavigation(1, 0, 0, 0, 0);

        public Navigation()
        {

        }

        public void SetScreen(int W, int H)
        {
            if ((ScreenWidth != 0) && (ScreenHeight != 0))
            {
                // Определим центр отображения
                double cX = OffsetX + PageWidth / 2;
                double cY = OffsetY + PageHeight / 2;

                var newAspectRatio = 1.0f * W / H;

                if(RefObject.AspectRatio > newAspectRatio)
                {
                    // берём ширину как константу
                    PageWidth = RefObject.PageWidth;
                    PageHeight = RefObject.PageWidth / newAspectRatio;
                }
                else
                {
                    // берём высоту как константу
                    PageHeight = RefObject.PageHeight;
                    PageWidth = RefObject.PageHeight * newAspectRatio;
                }
            }
            else
            {
                UpdateScale();
            }
            ScreenWidth = W;
            ScreenHeight = H;

            if (this.Object != RectangleF.Empty)
            {
            //    UpdateScale();
            }
        }

        public void SetObject(RectangleF Object)
        {
            this.Object = Object;
            Scale = 1;
            var aspectRatio = 1.0f * ScreenWidth / ScreenHeight;
            if (ScreenWidth <= ScreenHeight)
            {
                // Для вертикальной ориентации окна W < H
                PageWidth = Object.Height * aspectRatio;
                PageHeight = Object.Height;
                OffsetX = Object.Left;
                OffsetY = Object.Top;

                // Center:
                var VR = ViewRectangle;
                Scale = Math.Min(VR.Width / Object.Width, VR.Height / Object.Height);

            }
            else
            {
                PageWidth = Object.Width;
                PageHeight = Object.Width / aspectRatio;
                OffsetX = (Object.Left + Object.Right - PageWidth) / 2;
                OffsetY = Object.Top;

                // Center:
                var VR = ViewRectangle;
                Scale = Math.Min(VR.Width / Object.Width, VR.Height / Object.Height);

                var Diff = (PageWidth / Scale - Object.Width) / 2;

                OffsetX = -Diff;
            }

            RefObject = new RefNavigation(1.0f * ScreenWidth / ScreenHeight, OffsetX, OffsetY, PageWidth, PageHeight);
        }

        public RectangleF GetObject() => Object;

        protected void UpdateScale()
        {
            var aspectRatio = 1.0f * ScreenWidth / ScreenHeight;
            if (ScreenWidth <= ScreenHeight)
            {
                // Для вертикальной ориентации окна W < H
                PageWidth = Object.Height / aspectRatio;
                PageHeight = Object.Height;
            }
            else
            {
                PageWidth = Object.Width;
                PageHeight = Object.Width / aspectRatio;
            }

            RefObject = new RefNavigation(1.0f * ScreenWidth / ScreenHeight, OffsetX, OffsetY, PageWidth, PageHeight);
        }

        public RectangleF ViewRectangle => new RectangleF(OffsetX * Scale, OffsetY * Scale, PageWidth / Scale, PageHeight / Scale);

        /// <summary>
        /// Преобразование из экранных координат в сантиметры
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public PointF ConvertToPage(PointF Point)
        {
            var OrgPartX = Point.X * 1.0f / ScreenWidth;
            var OrgPartY = Point.Y * 1.0f / ScreenHeight;

            var VR = ViewRectangle;

            return new PointF(VR.Left + OrgPartX * VR.Width, VR.Top + OrgPartY * VR.Height);
        }

        /// <summary>
        /// Преобразование из сантиметров в  экранные координаты
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public PointF ConvertFromPage(PointF Point)
        {
            var VR = ViewRectangle;

            var OrgPartX = (Point.X - VR.Left) / VR.Width;
            var OrgPartY = (Point.Y - VR.Top) / VR.Height;

            return new PointF(OrgPartX * ScreenWidth, OrgPartY * ScreenHeight);
        }

        private float LimitRange(float Value, float Min, float Max)
        {
            if (Value < Min) return Min;
            if (Value > Max) return Max;

            return Value;
        }

        private float MaxXLimit => Math.Max(0, PageWidth * Scale - ScreenWidth);
        private float MaxYLimit => Math.Max(0, PageHeight * Scale - ScreenHeight);

        public SizeF GetSizeByPixels(SizeF PixelSize)
        {
            var VR = ViewRectangle;

            return new SizeF(PixelSize.Width * VR.Width / ScreenWidth, PixelSize.Height * VR.Height / ScreenHeight);
        }

        public void Move(float dX, float dY)
        {
            var VR = ViewRectangle;

            dX = dX * VR.Width / ScreenWidth;
            dY = dY * VR.Height / ScreenHeight;

            var OX = OffsetX + dX / Scale;
            var OY = OffsetY + dY / Scale;

            OffsetX = OX;// LimitRange(OX, 0, MaxXLimit);
            OffsetY = OY;// LimitRange(OY, 0, MaxYLimit);
        }

        public void Autoscale()
        {
            /*var BBW = 1.0f;
            var BBH = PageHeight / PageWidth;

            var SW = 1.0f;
            var SH = 1.0f * ScreenWidth / ScreenHeight;
            if((BBH >= 1.0f))
            {
                if (SH >= 1.0f)
                {
                    // Работаем по высоте
  //                  Scale = BBH / SH;
                }
                else
                {
 //                   Scale = BBH / SH;
                }
            }
            else
            {
                if (SH <= 1.0f)
                {
                    // Работаем по высоте
 //                   Scale = BBH / SH;
                }
                else
                { // S:0.7289999  OX:-0.3988379 OY:-0.102931023
                  //                    Scale = SH / BBH;
                }
            }

            float NewW = PageWidth * Scale;
            float NewH = PageHeight * Scale;

            OffsetX = (ScreenWidth - NewW) / Scale;
            OffsetY = (ScreenHeight - NewH) / Scale;*/
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

            var NewWidth = PageWidth / Scale;
            var NewHeight = PageHeight / Scale;

            var PageOffX = NewWidth * PartX;
            var PageOffY = NewHeight * PartY;

            OffsetX = (RealOrg.X - PageOffX) / Scale;
            OffsetY = (RealOrg.Y - PageOffY) / Scale;
        }

        protected class RefNavigation
        {
            public float PageWidth;
            public float PageHeight;

            public float OffsetX;
            public float OffsetY;

            public float AspectRatio;

            public RefNavigation(float AR, float OX, float OY, float PW, float PH)
            {
                this.AspectRatio = AR;

                OffsetX = OX;
                OffsetY = OY;

                PageWidth = PW;
                PageHeight = PH;
            }
        }
    }
}
