using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace G3D.Shaders
{
    // V
    public class TextShader : BaseShader
    {
        string Code;
        ShaderType SType;

        public TextShader(string Text, ShaderType T)
        {
            Code = Text;
            SType = T;
        }

        protected override ShaderType getShaderType() { return SType; }

        public override void Init(int Program)
        {
            base.Init(Program);

            Compile(Code);
        }
    }
}
