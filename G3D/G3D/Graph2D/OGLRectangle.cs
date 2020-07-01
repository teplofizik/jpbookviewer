using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.Graph2D
{
    public class OGLRectangle : OGLPolygonObject
    {
        public OGLRectangle(float X, float Y, float Width, float Height) : base(GeneratePoints(X, Y, Width, Height))
        {

        }

        private static PointF[] GeneratePoints(float X, float Y, float W, float H)
        {
            return new PointF[]
            {
                new PointF(X, Y),
                new PointF(X + W, Y),
                new PointF(X + W, Y + H),
                new PointF(X, Y + H),
                new PointF(X, Y)
            };
        }
    }
}
