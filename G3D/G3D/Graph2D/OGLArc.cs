using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.Graph2D
{
    public class OGLArc : OGLPolygonObject
    {
        public OGLArc(float X, float Y, float Radius, float AngleFrom, float AngleTo, bool Dir) : base(GeneratePoints(X, Y, Radius, AngleFrom, AngleTo, Dir))
        {

        }

        private static PointF[] GeneratePoints(float X, float Y, float Radius, float AngleFrom, float AngleTo, bool Dir)
        {
            var Res = new List<PointF>();

            float A = Convert.ToSingle(AngleFrom / 180 * Math.PI);
            float AT = Convert.ToSingle(AngleTo / 180 * Math.PI);

            float A360 = Convert.ToSingle(360 / 180 * Math.PI);
            bool Closed = (AT - A == A360);
            float Step = A360 / 360;

            if(!Closed) Res.Add(new PointF(X, Y));
            if (Dir)
            {
                // ++
                while(A <= AT)
                {
                    Res.Add(new PointF(Convert.ToSingle(X + Math.Cos(A) * Radius), Convert.ToSingle(Y - Math.Sin(A) * Radius)));
                    A += Step;
                }
            }
            else
            {
                while (A <= AT)
                {
                    Res.Add(new PointF(Convert.ToSingle(X + Math.Cos(A) * Radius), Convert.ToSingle(Y - Math.Sin(A) * Radius)));
                    A -= Step;
                }
            }
            if (!Closed) Res.Add(new PointF(X, Y));
            
            return Res.ToArray();
        }
    }
}
