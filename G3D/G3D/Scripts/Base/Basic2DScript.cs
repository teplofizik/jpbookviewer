using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace G3D.Scripts.Base
{
    /// <summary>
    /// Плоскость, вид сверху, ограничена по z от -1 до 1. Размеры от -100 до 100 по обеим осям,
    /// плюс за неквадратность к большей стороне
    /// </summary>
    public class Basic2DScript : Script
    {
        protected float fInternalWidth = 0;
        protected float fInternalHeight = 0;

        protected float fFieldSize = 100;
        protected float fDepth = 1;

        protected int Width = 0;
        protected int Height = 0;

        public Basic2DScript() { }
        public Basic2DScript(float Size) { fFieldSize = Size; }
        public Basic2DScript(float Size, float Depth) { fFieldSize = Size; fDepth = Depth; }

        /// <summary>
        /// Инициализация
        /// </summary>
        public override void InitViewPort(int W, int H)
        {
            GL.Viewport(0, 0, W, H); // Использовать всю поверхность GLControl под рисование

            base.InitViewPort(W, H);
        }

        /// <summary>
        /// Инициализация матрицы отображения
        /// </summary>
        public override void InitViewPortMode(int W, int H)
        {
            Width = W;
            Height = H; 
            ReinitPortMode();
        }

        protected void ReinitPortMode()
        {
            if ((Width != 0) && (Height != 0))
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                var aspectRatio = 1.0f * Width / Height;
                if (Width <= Height)
                {
                    fInternalWidth = fFieldSize;
                    fInternalHeight = fFieldSize / aspectRatio;
                }
                else
                {
                    fInternalWidth = fFieldSize * aspectRatio;
                    fInternalHeight = fFieldSize;
                }
                GL.Ortho(-fInternalWidth, fInternalWidth, -fInternalHeight, fInternalHeight, fDepth, -fDepth);
                GL.MatrixMode(MatrixMode.Modelview);
            }
        }
    }
}
