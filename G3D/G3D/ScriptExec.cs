using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using G3D.Scripts.Interface;
using System.Drawing;

namespace G3D
{
    public class ScriptExec
    {
        string Status = "";
        public string Text = "";

        private bool loaded = false;
        private Scripts.Script S;
        private int[] Selected = null;

        public void Init(Scripts.Script S)
        {
            this.S = S;
            loaded = true;

            S.Init();
        }

        public void Release()
        {
            S.Release();
        }

        public void Paint()
        {
            if (!loaded) return;

            S.Paint();
            S.PaintOverlay();
        }

        public void SetupViewport(int Width, int Height)
        {
            S.InitViewPort(Width, Height);
        }

        public void CheckKeyboard()
        {
            var KS = Keyboard.GetState();
            S.Input(KS.IsKeyDown(Key.Left), KS.IsKeyDown(Key.Right), KS.IsKeyDown(Key.Up), KS.IsKeyDown(Key.Down));
        }

        public void Tick(int dT)
        {
            S.Tick(dT);

            // Обработка ошибок
            if (true)
            {
                var E = GL.GetError();
                while (E != ErrorCode.NoError)
                {
                    Debug.WriteLine(E.ToString());
                    E = GL.GetError();
                }
            }

            Text = (S.GetType().ToString()) + ": " + S.GetStatus();
        }

        public void Click(Scripts.Script.MouseButton B, int X, int Y)
        {
            //S.Click(B, X, Y);
            if(B == Scripts.Script.MouseButton.Left) ProcessClick(X, Y);
        }

        /// <summary>
        /// Вращение камеры от горизонтального положения до положения сверху-вниз
        /// </summary>
        /// <param name="Angle">0 - 90</param>
        public void RotateVertical(float Angle)
        {
            if(S is ICameraControl)
            {
                ICameraControl CC = S as ICameraControl;
                
                CC.RotateVertical(Angle);
            }
        }

        /// <summary>
        /// Вращение горизонтальной плоскости компланарно
        /// </summary>
        /// <param name="Angle">-80 - 80</param>
        public void RotateHorizontal(float Angle)
        {
            if (S is ICameraControl)
            {
                ICameraControl CC = S as ICameraControl;

                CC.RotateHorizontal(Angle);
            }
        }

        #region Picking and selection

        /// <summary>
        /// Матрица для выбора объекта
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="deltax"></param>
        /// <param name="deltay"></param>
        /// <param name="viewport"></param>
        void PickMatrix(double x, double y, double deltax, double deltay, int[] viewport)
        {
            if (deltax <= 0 || deltay <= 0)
            {
                return;
            }

            /* Translate and scale the picked region to the entire window */
            GL.Translate((viewport[2] - 2 * (x - viewport[0])) / deltax, (viewport[3] - 2 * (y - viewport[1])) / deltay, 0);
            GL.Scale(viewport[2] / deltax, viewport[3] / deltay, 1.0);
        }
        
        /// <summary>
        /// Обработать выделение [для получения дерева зависимостей]
        /// </summary>
        /// <param name="selectBuff"></param>
        private void ProcessSelectionTree(int[] selectBuff, int Hits)
        {
            if (selectBuff != null)
            {
                int Start = 0;
                for (int h = 0; h < Hits; h++)
                {
                    int count = selectBuff[Start];
                    if (count > 0)
                    {
                        var Sel = new int[count];
                        for (int i = 0; i < count; i++)
                            Sel[i] = selectBuff[Start + 3 + i];

                        Selected = Sel;
                        Start += 3 + count;
                    }
                    else
                        Selected = null;
                }
            }
            else
                Selected = null;

            S.ProcessSelection(Selected);
        }

        /// <summary>
        /// Обработать выделение [для получения дерева зависимостей]
        /// </summary>
        /// <param name="selectBuff"></param>
        private void ProcessSelection(int[] selectBuff, int Hits)
        {
            List<int> Res = new List<int>();
            if (selectBuff != null)
            {
                int Start = 0;
                for (int h = 0; h < Hits; h++)
                {
                    int count = selectBuff[Start];
                    if (count > 0)
                    {
                        Res.Add(selectBuff[Start + 3]);
                        
                        Start += 3 + count;
                    }
                }
                if (Res.Count > 0)
                    Selected = Res.ToArray();
                else
                    Selected = null;
            }
            else
                Selected = null;

            S.ProcessSelection(Selected);
        }

        // Обработка щелчка
        private void ProcessClick(int X, int Y)
        {
            int[] selectBuff = new int[64];
            int[] viewport = new int[4];

            GL.SelectBuffer(selectBuff.Length, selectBuff);
            GL.GetInteger(GetPName.Viewport, viewport);
            GL.RenderMode(RenderingMode.Select);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            {
                
                GL.LoadIdentity();
                PickMatrix(X, viewport[3] - Y, 2, 2, viewport);
                S.InitViewPortMode(viewport[2], viewport[3]);
                S.Paint();

                // Collect the hits
                int hits = GL.RenderMode(RenderingMode.Render);

                if (hits > 0)
                    ProcessSelection(selectBuff, hits);
                else
                    ProcessSelection(null, 0);

                Status = (Selected != null) ? String.Join(", ", Selected) : "None";

                GL.MatrixMode(MatrixMode.Projection);
            }
            GL.PopMatrix();
        }
        #endregion
    }
}
