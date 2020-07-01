using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.Graph2D
{
    public abstract class OGLObject
    {
        protected static double dZ = 0;
        public void ResetZ() { dZ = 0; }
        public void IncrementZ() { dZ += 0.00001f; }

        public abstract void Draw(Pen P);
        public abstract void Fill(Color C);

        public abstract void Move(float X, float Y);

        public abstract bool IsContainsPoint(PointF Point);
    }
}
