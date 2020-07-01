using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace G3D.Scripts
{
    public class Script
    {
        protected const float PI = (float)Math.PI;
        protected int fWidth = 0;
        protected int fHeight = 0;

        /// <summary>
        /// Инициализация OpenGL
        /// </summary>
        public virtual void Init()
        {
            GL.ClearColor(Color.SkyBlue);
        }

        /// <summary>
        /// Инициализация видеоокошка
        /// </summary>
        public virtual void InitViewPort(int W, int H)
        {
            fWidth = W;
            fHeight = H;

            InitViewPortMode(W, H);
        }

        /// <summary>
        /// Обработать выделение
        /// </summary>
        /// <param name="Stack"></param>
        public virtual void ProcessSelection(int[] Stack)
        {

        }

        /// <summary>
        /// Инициализация матрицы отображения
        /// </summary>
        public virtual void InitViewPortMode(int W, int H)
        {

        }

        public void ReinitViewPort()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            InitViewPortMode(fWidth, fHeight);
        }

        /// <summary>
        /// Рисование кадра
        /// </summary>
        public virtual void Paint()
        {

        }

        /// <summary>
        /// Рисование оверлея
        /// </summary>
        public virtual void PaintOverlay()
        {

        }

        /// <summary>
        /// Для анимации
        /// </summary>
        /// <param name="dT"></param>
        public virtual void Tick(int dT)
        {

        }
        
        /// <summary>
        /// Ввод со стрелочек
        /// </summary>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <param name="Up"></param>
        /// <param name="Down"></param>
        public virtual void Input(bool Left, bool Right, bool Up, bool Down)
        {

        }

        /// <summary>
        /// Щелчок мышкой
        /// </summary>
        /// <param name="B"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public virtual void Click(MouseButton B, int X, int Y)
        {

        }

        /// <summary>
        /// Текстовое описание статуса, если вдруг что
        /// </summary>
        /// <returns></returns>
        public virtual string GetStatus()
        {
            return "";
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public virtual void Release()
        {

        }

        /// <summary>
        /// Нарисовать оси координат
        /// </summary>
        protected void DrawAxis(float Length)
        {
            GL.Begin(PrimitiveType.Lines);
            {
                // Axis X
                GL.Color3(Color.Red);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(Length, 0, 0);

                // Axis Y
                GL.Color3(Color.Green);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, Length, 0);

                // Axis Z
                GL.Color3(Color.Blue);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, Length);
            }
            GL.End();
        }

        #region Определение координат видимости
        protected PointF[] GetFeedbackPoints(int[] Ids)
        {
            const int FeedbackBufferSize = 32768;
            int Count = Ids.Length;
            PointF[] Res = new PointF[Count];
            float[] feedbackBuff = new float[FeedbackBufferSize];
            
            GL.FeedbackBuffer(feedbackBuff.Length, FeedbackType.Gl2D, feedbackBuff);
            GL.RenderMode(RenderingMode.Feedback);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            Paint();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            int Size = GL.RenderMode(RenderingMode.Render);
            // Анализируем буфер обратной связи
            // Получаем минимальные-максимальные координаты окна
            for (int i = 0; i < Size;)
            {
                // Ловим токен метки
                if ((FeedBackToken)feedbackBuff[i++] == FeedBackToken.PassThroughToken)
                {
                    int Index = -1;
                    int Id = Convert.ToInt32(feedbackBuff[i++]);
                    for (int idx = 0; idx < Ids.Length; idx++)
                        if (Ids[idx] == Id)
                        {
                            Index = idx;
                            break;
                        }

                    // Дальше идёт идетификатор метки
                    if (Index >= 0)
                    {
                        // Проходим цикл, пока не дойдём до следующего токена
                        while ((i < Size) && ((FeedBackToken)feedbackBuff[i] != FeedBackToken.PassThroughToken))
                        {
                            var T = (FeedBackToken)feedbackBuff[i];
                            
                            if (T == FeedBackToken.PointToken)
                            {
                                float x = feedbackBuff[++i];
                                float y = feedbackBuff[++i];

                                Res[Index] = new PointF(x, fHeight - y);
                                i++;
                            }
                            else
                                i++; // Получаем сл. индекс и продолжаем поиск
                        }
                    }
                }
            }

            return Res;
        }

        protected RectangleF[] GetFeedbackRect(int[] Ids)
        {
            const int FeedbackBufferSize = 32768;
            int Count = Ids.Length;
            RectangleF[] Res = new RectangleF[Count];
            float[] feedbackBuff = new float[FeedbackBufferSize];

            float[] top = new float[Count],
                    left = new float[Count],
                    bottom = new float[Count],
                    right = new float[Count];

            for (int i = 0; i < Count; i++)
            {
                top[i] = 999999;
                left[i] = 999999;
                bottom[i] = -999999;
                right[i] = -999999;
            }

            GL.FeedbackBuffer(feedbackBuff.Length, FeedbackType.Gl2D, feedbackBuff);
            GL.RenderMode(RenderingMode.Feedback);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            Paint();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            int Size = GL.RenderMode(RenderingMode.Render);
            // Анализируем буфер обратной связи
            // Получаем минимальные-максимальные координаты окна
            for (int i = 0; i < Size;)
            {
                // Ловим токен метки
                if ((FeedBackToken)feedbackBuff[i++] == FeedBackToken.PassThroughToken)
                {
                    int Index = -1;
                    int Id = Convert.ToInt32(feedbackBuff[i++]);
                    for (int idx = 0; idx < Ids.Length; idx++)
                        if (Ids[idx] == Id)
                        {
                            Index = idx;
                            break;
                        }

                    // Дальше идёт идетификатор метки
                    if (Index >= 0)
                    {
                        // Проходим цикл, пока не дойдём до следующего токена
                        while ((i < Size) && ((FeedBackToken)feedbackBuff[i] != FeedBackToken.PassThroughToken))
                        {
                            var T = (FeedBackToken)feedbackBuff[i];

                            // Получаем многоугольники
                            if (T == FeedBackToken.PolygonToken)
                            {
                                // Получаем количество точек в многоугольнике
                                int count = Convert.ToInt16(feedbackBuff[++i]);

                                // По две координаты на точку, т.к. режим 2D
                                for (int j = 0; j < count; j++)
                                {
                                    float x = feedbackBuff[++i];
                                    if (x > right[Index]) right[Index] = x;
                                    if (x < left[Index]) left[Index] = x;

                                    float y = feedbackBuff[++i];
                                    if (y > bottom[Index]) bottom[Index] = y;
                                    if (y < top[Index]) top[Index] = y;
                                }
                            }
                            else if (T == FeedBackToken.PointToken)
                            {
                                float x = feedbackBuff[++i];
                                if (x > right[Index]) right[Index] = x;
                                if (x < left[Index]) left[Index] = x;

                                float y = feedbackBuff[++i];
                                if (y > bottom[Index]) bottom[Index] = y;
                                if (y < top[Index]) top[Index] = y;
                                i++;
                            }
                            else
                                i++; // Получаем сл. индекс и продолжаем поиск
                        }
                    }
                }
            }

            for (int i = 0; i < Count; i++)
                Res[i] = RectangleF.FromLTRB(left[i], fHeight - top[i], right[i], fHeight - bottom[i]);

            return Res;
        }

        protected RectangleF GetFeedbackRect(int Id)
        {
            const int FeedbackBufferSize = 32768;
            float[] feedbackBuff = new float[FeedbackBufferSize];

            float top = 999999, left = 999999;
            float bottom = -999999, right = -999999;
            GL.FeedbackBuffer(feedbackBuff.Length, FeedbackType.Gl2D, feedbackBuff);
            GL.RenderMode(RenderingMode.Feedback);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            Paint();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            int Size = GL.RenderMode(RenderingMode.Render);
            // Анализируем буфер обратной связи
            // Получаем минимальные-максимальные координаты окна
            for (int i = 0; i < Size;)
            {
                // Ловим токен метки
                if ((FeedBackToken)feedbackBuff[i++] == FeedBackToken.PassThroughToken)
                {
                    // Дальше идёт идетификатор метки
                    if (feedbackBuff[i++] == Id)
                    {
                        // Проходим цикл, пока не дойдём до следующего токена
                        while ((i < Size) && ((FeedBackToken)feedbackBuff[i] != FeedBackToken.PassThroughToken))
                        {
                            var T = (FeedBackToken)feedbackBuff[i];

                            // Получаем многоугольники
                            if (T == FeedBackToken.PolygonToken)
                            {
                                // Получаем количество точек в многоугольнике
                                int count = Convert.ToInt16(feedbackBuff[++i]);

                                // По две координаты на точку, т.к. режим 2D
                                for (int j = 0; j < count; j++)
                                {
                                    float x = feedbackBuff[++i];
                                    if (x > right) right = x;
                                    if (x < left) left = x;

                                    float y = feedbackBuff[++i];
                                    if (y > bottom) bottom = y;
                                    if (y < top) top = y;
                                }
                            }
                            else if (T == FeedBackToken.PointToken)
                            {
                                float x = feedbackBuff[++i];
                                if (x > right) right = x;
                                if (x < left) left = x;

                                float y = feedbackBuff[++i];
                                if (y > bottom) bottom = y;
                                if (y < top) top = y;
                            }
                            else
                                i++; // Получаем сл. индекс и продолжаем поиск
                        }
                        break;
                    }
                }
            }

            return RectangleF.FromLTRB(left, fHeight - bottom, right, fHeight - top);
        }
        #endregion

        // Немного математики
        protected static float Sin(float Angle)
        {
            return Convert.ToSingle(Math.Sin(Angle));
        }

        // Немного математики
        protected static float Cos(float Angle)
        {
            return Convert.ToSingle(Math.Cos(Angle));
        }

        public enum MouseButton { Left, Middle, Right };
    }
}
