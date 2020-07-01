using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace G3D.Scripts.Base
{
    /// <summary>
    /// Кубик объёмом 100х100х100 (если не указано иное), и вращается по кнопочкам направления
    /// </summary>
    public class Basic3DScript : Script
    {
        protected float fInternalWidth = 0;
        protected float fInternalHeight = 0;

        protected float fRange;
        protected float fAngleX = 50, fAngleY = 50; // Углы обзора
        
        public Basic3DScript()
        {
            fRange = 100.0f;
        }

        public Basic3DScript(float Range)
        {
            fRange = Range;
        }

        public override void Init()
        {
            base.Init();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Multisample);
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        public override void InitViewPort(int W, int H)
        {
            GL.Viewport(0, 0, W, H); // Использовать всю поверхность GLControl под рисование

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            base.InitViewPort(W, H);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            // GL.Rotate(fAngleX, 1, 0, 0);
            // GL.Rotate(fAngleY, 0, 1, 0);
        }

        /// <summary>
        /// Инициализация матрицы отображения
        /// </summary>
        public override void InitViewPortMode(int W, int H)
        {
            if (W <= H)
            {
                fInternalWidth = fRange;
                fInternalHeight = fRange * H / W;
                GL.Ortho(-fRange, fRange, -fRange * H / W, fRange * H / W, -fRange, fRange);
            }
            else
            {
                fInternalWidth = fRange * W / H;
                fInternalHeight = fRange;
                GL.Ortho(-fRange * W / H, fRange * W / H, -fRange, fRange, -fRange, fRange);
            }
        }

        /// <summary>
        /// Ввод со стрелочек
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <param name="Up"></param>
        /// <param name="Down"></param>
        public override void Input(bool Left, bool Right, bool Up, bool Down)
        {
            GL.MatrixMode(MatrixMode.Projection);
            if (Left)
                GL.Rotate(1, 0, 1, 0);
            else if (Right)
                GL.Rotate(-1, 0, 1, 0);

            if (Up)
                GL.Rotate(1, 1, 0, 0);
            else if (Down)
                GL.Rotate(-1, 1, 0, 0);

            GL.MatrixMode(MatrixMode.Modelview);
        }
    }
}
