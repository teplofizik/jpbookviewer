using G3D.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace G3D.UI.Buffers
{
    public class TextureBuffer
    {
        int Timeout = 0;
        public Texture.Texture T;

        public TextureBuffer(Texture.Texture T)
        {
            this.T = T;
            Timeout = 20;
        }

        public void ResetTimeout()
        {
            Timeout = 20;
        }

        public bool ToRemove => (Timeout == 0);

        protected virtual void Remove()
        {

        }

        public void Bind()
        {
            ResetTimeout();
            if(T != null) T.Bind();
        }

        public void Tick()
        {
            if (Timeout > 0)
            {
                Timeout--;
                if (Timeout == 0)
                {
                    T.Release();
                    Remove();
                    T = null;
                }
            }
        }
    }
}
