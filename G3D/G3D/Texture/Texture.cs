using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace G3D.Texture
{
    public class Texture : IDisposable
    {
        private int iId = -1;
        
        protected virtual void TexImage()
        {

        }
       
        protected void Create()
        {
            iId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, iId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);

            TexImage();

            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)0x84FE, 16.0f); //GL_TEXTURE_MAX_ANISOTROPY_EXT
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        public int Id
        {
            get { return iId; }
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, iId);
        }

        public static void UnbindAll()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Release()
        {
            if (iId != -1) GL.DeleteTexture(iId);
        }

        public virtual void Dispose()
        {
            Release();
        }
    }
}
