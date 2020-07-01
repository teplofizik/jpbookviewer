using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;
using System.Drawing;

namespace G3D.Shaders
{
    public class GLProgram
    {
        List<BaseShader> Shaders = new List<BaseShader>();
        int idProgram = -1;
        bool Linked = false;

        public GLProgram()
        {
            Init();
        }

        /// <summary>
        /// Идентификатор программы
        /// </summary>
        public int Program {  get { return idProgram; } }

        /// <summary>
        /// Инициализация шейдера
        /// </summary>
        public virtual void Init()
        {
            idProgram = GL.CreateProgram();
        }

        /// <summary>
        /// Добавить шейдер к программе
        /// </summary>
        /// <param name="S"></param>
        public void AttachShader(BaseShader S)
        {
            Shaders.Add(S);
            S.Init(idProgram);
            S.Attach();
        }

        /// <summary>
        /// Подключить к конвееру
        /// </summary>
        public void Link()
        {
            GL.LinkProgram(idProgram);
            
            int LinkStatus;
            GL.GetProgram(idProgram, GetProgramParameterName.LinkStatus, out LinkStatus);
            if (LinkStatus != 1)
            {
                System.Diagnostics.Debug.WriteLine("Link failed!");
                System.Diagnostics.Debug.WriteLine(GL.GetProgramInfoLog(idProgram));
                Linked = false;
            }
            else
                Linked = true;
        }

        /// <summary>
        /// Собрана программа?
        /// </summary>
        /// <returns></returns>
        public bool isLinked()
        {
            return Linked;
        }

        /// <summary>
        /// Объявить активно используемым
        /// </summary>
        public void Bind()
        {
            GL.UseProgram(idProgram);
        }

        /// <summary>
        /// Освобождение программы и шейдеров
        /// </summary>
        public virtual void Release()
        {
            if (idProgram != -1)
                GL.DeleteProgram(idProgram);

            foreach (var S in Shaders) S.Release();
        }

        public void Uniform(string Name, float Value)
        {
            if (!isLinked()) return;

            int Loc = GL.GetUniformLocation(idProgram, Name);

            if (Loc == -1)
                Debug.WriteLine("Unknown location for '" + Name + "'");
            else
                GL.Uniform1(Loc, Value);
        }

        public void Uniform(string Name, Vector3 Value)
        {
            if (!isLinked()) return;

            int Loc = GL.GetUniformLocation(idProgram, Name);

            if (Loc == -1)
                Debug.WriteLine("Unknown location for '" + Name + "'");
            else
                GL.Uniform3(Loc, Value.X, Value.Y, Value.Z);
        }

        public void Uniform(string Name, Color Value)
        {
            if (!isLinked()) return;

            int Loc = GL.GetUniformLocation(idProgram, Name);

            if (Loc == -1)
                Debug.WriteLine("Unknown location for '" + Name + "'");
            else
                GL.Uniform4(Loc, Value.R / 255.0, Value.G / 255.0, Value.B / 255.0, Value.A / 255.0);
        }
    }
}
