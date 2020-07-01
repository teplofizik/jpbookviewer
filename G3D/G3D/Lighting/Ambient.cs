using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace G3D.Lighting
{
    public class Ambient : LightSource
    {
        float[] ambientLight = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
        float[] diffuseLight = new float[] { 0.6f, 0.6f, 0.6f, 1.0f };
        float[] lightPos = new float[4];

        public Ambient(int Index, Vector3 Pos) : base(Index)
        {
            Init(Pos);
        }

        public Ambient(int Index, Vector3 Pos, float[] diffuse) : base(Index)
        {
            diffuseLight = diffuse;
            Init(Pos);
        }

        private void Init(Vector3 Pos)
        {
            Light(LightParameter.Ambient, ambientLight);
            Light(LightParameter.Diffuse, diffuseLight);
            {
                lightPos[0] = Pos.X;
                lightPos[1] = Pos.Y;
                lightPos[2] = Pos.Z;
                lightPos[3] = 1.0f;

                Light(LightParameter.Position, lightPos);
            }

            Enable();
        }

        /// <summary>
        /// Поместить на сцене
        /// </summary>
        public override void Put()
        {
            Light(LightParameter.Position, lightPos);
        }
    }
}
