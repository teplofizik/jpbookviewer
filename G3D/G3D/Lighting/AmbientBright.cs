using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace G3D.Lighting
{
    public class AmbientBright : LightSource
    {
        float[] ambientLight = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
        float[] diffuseLight = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        float[] lightPos = new float[4];

        public AmbientBright(int Index, Vector3 Pos) : base(Index)
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
