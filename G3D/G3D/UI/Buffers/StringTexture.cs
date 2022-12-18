using G3D.Texture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.UI.Buffers
{
    class StringTexture : TextureBuffer
    {
        public string Text;
        public Color Clr;

        public Font F;

        public StringTexture(string Text, Font F, Color C, Texture.Texture T) : base(T)
        {
            this.F = F;
            this.Clr = C;
            this.Text = Text;
        }

        protected override void Remove()
        {
            T.Dispose();
        }
    }
}
