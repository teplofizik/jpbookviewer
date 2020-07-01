using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using G3D.Scripts.Interface;

namespace G3D.Scripts.Base
{
    /// <summary>
    /// Перспективная проекция
    /// </summary>
    public class Basic3DScriptP : Script, ICameraControl
    {
        protected float fInternalWidth = 0;
        protected float fInternalHeight = 0;

        protected GLFrame Camera = new GLFrame();
        private float fDepth = 400;
        private float fSpeed = 0.6f;

        public Basic3DScriptP()
        {
            fDepth = 400;
        }

        public Basic3DScriptP(float Range)
        {
            fDepth = Range;
        }

        public Basic3DScriptP(float Range, float Speed)
        {
            fDepth = Range;
            fSpeed = Speed;
        }

        public override void Init()
        {
            base.Init();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Multisample);

            Camera.MoveFrameForward(-50f);
          //  Camera.MoveFrameUp(20f);
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
        }

        /// <summary>
        /// Инициализация матрицы отображения
        /// </summary>
        public override void InitViewPortMode(int W, int H)
        {
            if (H == 0) H = 1;
            float fAspect = W * 1.0f / H;
            Geometry.Perspective(1.0f, fAspect, 1, fDepth);
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
            if (Up)
                Camera.MoveFrameForward(fSpeed);
            if (Down)
                Camera.MoveFrameForward(-fSpeed);

            if (Left)
                Camera.MoveFrameRight(1f);
            if (Right)
                Camera.MoveFrameRight(-1f);
        }

        public void RotateVertical(float Angle)
        {
            Camera.RotateFrameLocalX(Angle);
        }

        public void RotateHorizontal(float Angle)
        {
            Camera.RotateFrameLocalY(Angle);
        }
    }
}
