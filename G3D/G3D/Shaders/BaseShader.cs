using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using OpenTK.Graphics.OpenGL;

namespace G3D.Shaders
{
    public abstract class BaseShader
    {
        protected int idProgram = -1;
        protected int idShader = -1;
        protected bool Compiled = false;
        protected string Name = "";

        public BaseShader() { Name = GetType().ToString(); }
        public BaseShader(string Name) { this.Name = Name; }

        protected abstract ShaderType getShaderType();

        /// <summary>
        /// Инициализация шейдера
        /// </summary>
        public virtual void Init(int Program)
        { 
            idProgram = Program;
            idShader = GL.CreateShader(getShaderType());
        }

        /// <summary>
        /// Компиляция исходного кода
        /// </summary>
        /// <param name="Code"></param>
        protected void Compile(string Code)
        {
            GL.ShaderSource(idShader, Code);
            GL.CompileShader(idShader);

            string info;
            GL.GetShaderInfoLog(idShader, out info);
            Debug.WriteLine(info);

            int compileResult;
            GL.GetShader(idShader, ShaderParameter.CompileStatus, out compileResult);
            if (compileResult != 1)
                Debug.WriteLine(Name + ": Compile Error!");
            else
                Compiled = true;
        }

        /// <summary>
        /// Ассоциация шейдера с программой
        /// </summary>
        public virtual void Attach()
        {
            GL.AttachShader(idProgram, idShader);
        }

        /// <summary>
        /// Освобождение шейдера
        /// </summary>
        public virtual void Release()
        {
            if (idShader != -1)
                GL.DeleteShader(idShader);
        }
    }
}
