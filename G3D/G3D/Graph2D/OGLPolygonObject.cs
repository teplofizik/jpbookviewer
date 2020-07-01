using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.Graph2D
{
    public class OGLPolygonObject : OGLObject
    {
        PointF[] Points;

        public OGLPolygonObject(PointF[] Points)
        {
            this.Points = Points;
        }

        protected virtual void PutPoints()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                var P = Points[i];
                GL.Vertex3(P.X, P.Y, dZ);
            }
        }

        public override void Draw(Pen Pen)
        {
            Texture.Texture.UnbindAll();
            GL.PushMatrix();
            GL.Color3(Pen.Color);
            GL.LineWidth(Pen.Width);
            GL.Begin(PrimitiveType.Lines);
            PutPoints();
            GL.End();
            GL.PopMatrix();
            IncrementZ();
        }

        public override void Fill(Color C)
        {
            Texture.Texture.UnbindAll();
            GL.PushMatrix();
            GL.Color3(C);
            GL.Begin(PrimitiveType.Polygon);
            PutPoints();
            GL.End();
            GL.PopMatrix();
            IncrementZ();
        }

        public override bool IsContainsPoint(PointF Point)
        {
            var coef = Points.Skip(1).Select((p, i) =>
                                 (Point.Y - Points[i].Y) * (p.X - Points[i].X)
                               - (Point.X - Points[i].X) * (p.Y - Points[i].Y))
                         .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }

        public override void Move(float X, float Y)
        {
            for(int i = 0; i < Points.Length; i++)
            {
                Points[i].X += X;
                Points[i].Y += Y;
            }
        }
    }
}
