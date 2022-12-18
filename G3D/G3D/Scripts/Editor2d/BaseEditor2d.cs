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
using G3D.UI.Editor;
using System.Windows.Forms;
using G3D.UI;
using G3D.Tool;
using System.Diagnostics;

namespace G3D.Scripts.Editor2d
{
    public class BaseEditor2d : Basic2DScript
    {
        public Color BackgroundColor;

        public float Scale => NavigationObject.Scale;
        public Navigation.Navigation NavigationObject = new Navigation.Navigation();

        public bool MouseProcessed = false;

        protected BaseDocument Document = null;
        protected string SceneName = "";

        protected ToolAction ActiveTool = null;

        protected PointF LastPos = PointF.Empty;
        protected PointF MousePos => MouseProcessed ? PointF.Empty : LastPos;

        public PointF MousePosition => LastPos;

        public BaseEditor2d(string Name)
        {
            SceneName = Name;
            BackgroundColor = Color.White; 
        }

        /// <summary>
        /// Инициализация OpenGL
        /// </summary>
        public override void Init()
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);
         //   GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.LineSmooth);
        //    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Синий фон
            GL.ClearColor(Color.Black);
            GL.Color3(Color.White);

            GL.CullFace(CullFaceMode.FrontAndBack);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

            //    SetImage(null);
        }
        
       /* public void SetImage(string FN)
        {
            if(TI != null) TI.Release();
            if ((FN != null) && File.Exists(FN))
                TI = new TextureImage(FN);
            else
                TI = new TextureSolid(20, 20, Color.Blue);

            var BT = TI as BitmapTexture;
            NavigationObject.SetPage(BT.Width, BT.Height);
        }*/

        public void UpdateCS()
        {
        }

        private void UpdatePortMode()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Viewport(0, 0, NavigationObject.ScreenWidth, NavigationObject.ScreenHeight); // Использовать всю поверхность GLControl под рисование

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
            if ((H != 0) && (W != 0))
            {
                NavigationObject.SetScreen(W, H);
            }
        }

        protected void DrawRect(RectangleF R, Color C, float Z = 0)
        {
            GL.Color4(C);
            GL.PushMatrix();
            GL.Translate(R.Left, R.Top, -Z);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(R.Width, 0, 0);
            GL.Vertex3(R.Width, R.Height, 0);
            GL.Vertex3(0, R.Height, 0);
            GL.Vertex3(0, 0, 0);
            GL.End();
            GL.PopMatrix();
        }

        protected void FillRect(RectangleF R, Color C, float Z = 0)
        {
            GL.Color4(C);
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

        protected void FillRect(RectangleF R, Texture.Texture T, float Z = 0)
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
            GL.LoadIdentity();
        //    DrawRect(new Rectangle(0, 0, NavigationObject.PageWidth, NavigationObject.PageHeight), TI);
        }

        protected virtual void DrawOverlay()
        {

        }

        protected virtual void DrawMenu()
        {

        }

        /// <summary>
        /// Рисование кадра
        /// </summary>
        public override void Paint()
        {
            UpdatePortMode();

            MouseProcessed = false;
            GL.GetError();
            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.PushMatrix();
            {
                DrawBase();
                DrawOverlay();
                if(ActiveTool != null)
                {
                    ActiveTool.OnDraw(GetDrawer());
                }
                DrawMenu();
            }
            GL.PopMatrix();
        }
        protected void GlError(string Text)
        {
            var Err = GL.GetError();
            if (Err != ErrorCode.NoError)
            {
                Debug.WriteLine($"GL Error in {Text}: {Err}");
            }
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public override void Release()
        {
        }

        protected IGLDrawer GetDrawer() => new IGLDrawer(NavigationObject.ScreenWidth, NavigationObject.ScreenHeight);

        public BaseDocument GetDocument()
        {
            return Document;
        }

        public virtual void SetDocument(BaseDocument Doc)
        {
            if (Doc != Document)
            {
                Document = Doc;
                NavigationObject.SetObject(Doc.getBoundingBox(SceneName));
                NavigationObject.Autoscale();
                UpdateActiveTool();
            }
        }

        public void CheckBoundingBox()
        {
            var Doc = GetDocument();
            if (Doc != null)
            {
                var BB = Document.getBoundingBox(SceneName);

                if(BB != NavigationObject.GetObject())
                {
                    NavigationObject.SetObject(BB);
                }
            }
        }

        protected void UpdateActiveTool()
        {
            var Doc = GetDocument();

            if (ActiveTool != null)
                ActiveTool.CheckUpdates(ref Doc);
        }

        public void OnDoubleClick(MouseEventArgs mouseEventArgs)
        {
            if (ActiveTool != null)
            {
                if (ActiveTool.OnDoubleClick(mouseEventArgs, NavigationObject.ConvertToPage(new PointF(mouseEventArgs.X, mouseEventArgs.Y))))
                    UpdateActiveTool();
            }
        }

        protected virtual UIInterface GetActiveUI() =>  null;

        public void OnClick(MouseEventArgs e)
        {
            var UI = GetActiveUI();

            if (UI != null)
            {
                if (UI.OnClick(e))
                    return;
            }

            if (ActiveTool != null)
            {
                if (ActiveTool.OnClick(e, NavigationObject.ConvertToPage(new PointF(e.X, e.Y))))
                    UpdateActiveTool();
            }
        }

        public virtual bool OnMouseDown(PointF Pos, MouseButtons Button)
        { 
            if (ActiveTool != null)
            {
                return ActiveTool.OnMouseDown(Pos, Button);
            }
            return false;
        }

        public virtual void OnMouseUp(PointF Pos, MouseButtons Button)
        {
            ActiveTool?.OnMouseUp(Pos, Button);
        }

        public virtual void OnMouseMove(PointF Pos, MouseButtons Button)
        {
            ActiveTool?.OnMouseMove(Pos, Button);
        }

        public bool OnMouseDown(MouseEventArgs e)
        {
           var Pos =  NavigationObject.ConvertToPage(e.Location);
            return OnMouseDown(Pos, e.Button);
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            var Pos = NavigationObject.ConvertToPage(e.Location);
            OnMouseUp(Pos, e.Button);
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            var Pos = NavigationObject.ConvertToPage(e.Location);
            OnMouseMove(Pos, e.Button);
        }
    }
}