using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using G3D.Lighting;
using G3D.Scripts.Base;
using System.Drawing;
using G3D.Texture;
using G3D.Texture.Format;
using G3D.Texture.Generated;
using System.IO;

namespace JefViewer.SewViewer
{
    class BookRenderer : Basic2DScript
    {
        public float Scale => NavigationObject.Scale;
        public Navigation NavigationObject = new Navigation();

        public bool MouseProcessed = false;

        Texture TI;
        protected PointF LastPos = PointF.Empty;
        protected PointF MousePos => MouseProcessed ? PointF.Empty : LastPos;
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

            SetImage(null);
        }
        
        public void SetImage(string FN)
        {
            if(TI != null) TI.Release();
            if ((FN != null) && File.Exists(FN))
                TI = new TextureImage(FN);
            else
                TI = new TextureSolid(20, 20, Color.Blue);

            var BT = TI as BitmapTexture;
            NavigationObject.SetPage(BT.Width, BT.Height);
         //   NavigationObject.OffsetX = 0;
         //   NavigationObject.OffsetY = 0;
         //   NavigationObject.Scale = (NavigationObject.ScreenWidth == 0) ? 1 : NavigationObject.ScreenWidth * 1.0f /  BT.Width;

        }

        public void UpdateCS()
        {
            UpdatePortMode();
        }

        private void UpdatePortMode()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            
            {
                var Rect = NavigationObject.ViewRectangle;

                GL.Ortho(Rect.Left, Rect.Right, Rect.Bottom, Rect.Top, fDepth, -fDepth);
            }

            GL.MatrixMode(MatrixMode.Modelview);
        }
        
        public void UpdateScale(float Value, Point Org)
        {
            NavigationObject.ChangeScale(Value, Org);
        }

        public void UpdateScale(float Value)
        {
            NavigationObject.ChangeScale(Value);
        }

        
        public void Move(int dX, int dY)
        {
            NavigationObject.Move(dX, dY);
        }

        public virtual void SetActualCursor(PointF Point)
        {
            LastPos = NavigationObject.ConvertToPage(Point);
        }

        /// <summary>
        /// Инициализация матрицы отображения
        /// </summary>
        public override void InitViewPortMode(int W, int H)
        {
            NavigationObject.SetScreen(W, H);

            UpdatePortMode();
        }

        protected void DrawRect(RectangleF R, Texture T, float Z = 0)
        {
            T.Bind();

            GL.PushMatrix();
            GL.Translate(R.Left, R.Top, -Z);
            GL.Begin(PrimitiveType.Triangles);
            GL.Normal3(0, -1, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex3(R.Width, R.Height, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex3(0, R.Height, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex3(R.Width, R.Height, 0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(R.Width, 0, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, 0, 0);
            GL.End();
            GL.PopMatrix();
        }

        protected virtual void DrawBase()
        {
            DrawRect(new Rectangle(0, 0, NavigationObject.PageWidth, NavigationObject.PageHeight), TI);
        }

        protected virtual void DrawOverlay()
        {

        }

        /// <summary>
        /// Рисование кадра
        /// </summary>
        public override void Paint()
        {
            MouseProcessed = false;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.PushMatrix();
            {
                DrawBase();
                DrawOverlay();
            }
            GL.PopMatrix();
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public override void Release()
        {
            TI.Release();
        }

    }
}