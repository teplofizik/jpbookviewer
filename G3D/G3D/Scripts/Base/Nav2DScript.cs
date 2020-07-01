using G3D.Navigation;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace G3D.Scripts.Base
{
    public class Nav2DScript : Basic2DScript
    {
        public float Scale => Navigation.Scale;
        public Navigation2D Navigation = new Navigation2D();

        /// <summary>
        /// Инициализация OpenGL
        /// </summary>
        public override void Init()
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Синий фон
            GL.ClearColor(Color.Black);
            GL.Color3(Color.White);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
        }

        protected void UpdatePortMode()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            {
                var Rect = Navigation.ViewRectangle;

                GL.Ortho(Rect.Left, Rect.Right, Rect.Bottom, Rect.Top, fDepth, -fDepth);
            }

            GL.MatrixMode(MatrixMode.Modelview);
        }

        /// <summary>
        /// Инициализация матрицы отображения
        /// </summary>
        public override void InitViewPortMode(int W, int H)
        {
            Navigation.SetScreen(W, H);

            UpdatePortMode();
        }


        /// <summary>
        /// Поменять масштаб относительно текущего относительно заданной точки
        /// </summary>
        /// <param name="Change">Изменение в долях (+1 = +100%), (-0.5 = -50%)</param>
        /// <param name="Org">Опорная точка</param>
        public void UpdateScale(float Change, Point Org)
        {
            Navigation.ChangeScale(Navigation.Scale + Change * Navigation.Scale, Org);
        }

        /// <summary>
        /// Поменять масштаб относительно текущего относительно левого верхнего угла
        /// </summary>
        /// <param name="Change">Изменение в долях (+1 = +100%), (-0.5 = -50%)</param>
        public void UpdateScale(float Change)
        {
            Navigation.ChangeScale(Navigation.Scale + Change * Navigation.Scale);
        }

        protected virtual void DrawBase()
        {

        }

        protected virtual void DrawOverlay()
        {

        }

        /// <summary>
        /// Рисование кадра
        /// </summary>
        public override void Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.PushMatrix();
            {
                DrawBase();
                DrawOverlay();
            }
            GL.PopMatrix();
        }

        public void ProcessClick(PointF Pos)
        {
            var P = Navigation.ConvertToPage(Pos);

            OnMouseClick(P);
        }

        /// <summary>
        /// Обработать нажатие мышки
        /// </summary>
        /// <param name="Location"></param>
        protected virtual void OnMouseClick(PointF Location)
        {

        }
    }
}
