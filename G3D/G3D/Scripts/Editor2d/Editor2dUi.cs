using G3D.Scripts.Editor2d;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using G3D.UI;
using G3D.Tool;

namespace G3D.Scripts.Editor2d
{
    public class Editor2dUi : BaseEditor2d
    {
        protected UIInterface ui = new UIInterface();

        public Editor2dUi(string Name) : base(Name)
        {

        }

        protected virtual void InitUI()
        {

        }

        protected PointF RealPosition => NavigationObject.ConvertToPage(LastPos);

        protected void BeginDrawUI()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, NavigationObject.ScreenWidth, NavigationObject.ScreenHeight, 0, fDepth, -fDepth);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
        }
        protected void EndDrawUI()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
        }

        private bool IsInRect(RectangleF R, PointF P)
        {
            return (P.X > R.Left) && (P.X < R.Right) && (P.Y > R.Top) && (P.Y < R.Bottom);
        }

        protected void DrawActiveMenu(IGLDrawer drawer)
        {

        }

        protected override void DrawMenu()
        {
            BeginDrawUI();

            var Drawer = GetDrawer();
            Drawer.PushZ(0.9f);
            DrawActiveMenu(Drawer); 
            Drawer.PopZ();

            EndDrawUI();
        }

        public override void SetActualCursor(PointF Point)
        {
            base.SetActualCursor(Point);

            if (ActiveTool != null)
                ActiveTool.OnMouseMove(LastPos, MouseButtons.None);
        }
    }
}
