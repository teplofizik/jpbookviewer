using G3D.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.UI.Editor
{
    public class BaseDocument
    {
        public Color BackgroundColor = Color.Gray;
        public Color AxisColor = Color.Black;
        public Color MainAxisColor = Color.Blue;

        protected Dictionary<string, RectangleF> Boxes = new Dictionary<string, RectangleF>();

        public virtual RectangleF getBoundingBox(string Name)
        {
            lock (Boxes)
            {
                return Boxes.ContainsKey(Name) ? Boxes[Name] : new RectangleF(-1, -1, 2, 2);
            }
        }

        public void SetBoundingBox(string Name, RectangleF BB)
        {
            lock (Boxes)
            {
                Boxes[Name] = BB;
            }
        }

        public virtual void Draw(IGLDrawer Drawer, Navigation.Navigation Nav)
        {

        }

        protected float GetStep(float Range)
        {
            float[] Steps = new float[] { 100, 10, 1, 0.1f, 0.01f };

            foreach (var S in Steps)
            {
                var StepCount = Range / S;

                if ((StepCount >= 5) && (StepCount <= 50))
                    return S;
            }

            return 0.01f;
        }

        protected float CalcStep(RectangleF VR)
        {
            var dX = Math.Abs(VR.Width);
            var dY = Math.Abs(VR.Height);

            return Math.Min(GetStep(dX), GetStep(dY));
        }

        protected void DrawCS(IGLDrawer Drawer, RectangleF Rect)
        {
            var Step = CalcStep(Rect);

            var StartX = Convert.ToInt32((Rect.Left - Step * 3) / Step) * Step;
            var StartY = Convert.ToInt32((Rect.Top - Step * 3) / Step) * Step;

            Drawer.PushZ(-0.9f);
            for (float X = StartX; X < Rect.Right; X += Step)
            {
                int Crat = Convert.ToInt32(X / Step);
                bool Bold = ((Crat % 5) == 0);

                Drawer.LineWidth(Bold ? 3 : 1.5f);
                Drawer.DrawLine(new PointF(X, Rect.Top), new PointF(X, Rect.Bottom), (Math.Abs(X) < 0.001) ? MainAxisColor : AxisColor);
            }

            for (float Y = StartY; Y < Rect.Bottom; Y += Step)
            {
                int Crat = Convert.ToInt32((Y + 0.001) / Step);
                bool Bold = ((Crat % 5) == 0);

                Drawer.LineWidth(Bold ? 3 : 1.5f);
                Drawer.DrawLine(new PointF(Rect.Left, Y), new PointF(Rect.Right, Y), (Math.Abs(Y) < 0.001) ? MainAxisColor : AxisColor);
            }
            Drawer.PopZ();
        }
    }
}
