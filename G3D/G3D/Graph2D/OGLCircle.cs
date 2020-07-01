using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G3D.Graph2D
{
    public class OGLCircle : OGLArc
    {
        public OGLCircle(float X, float Y, float Radius) : base(X, Y, Radius, 0, 360, true)
        {

        }
    }
}
