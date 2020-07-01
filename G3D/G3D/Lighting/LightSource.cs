using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace G3D.Lighting
{
    /// <summary>
    /// Источник освещения
    /// </summary>
    public class LightSource
    {
        protected int LightIndex = 0;

        /// <summary>
        /// Номер источника - от 0 до 7
        /// </summary>
        /// <param name="Index"></param>
        public LightSource(int Index)
        {
            LightIndex = Index;
        }

        protected bool CheckId() { return (LightIndex >= 0) && (LightIndex <= 7); }

        protected EnableCap LightCap
        {
            get
            {
                switch (LightIndex)
                {
                    case 0: return EnableCap.Light0;
                    case 1: return EnableCap.Light1;
                    case 2: return EnableCap.Light2;
                    case 3: return EnableCap.Light3;
                    case 4: return EnableCap.Light4;
                    case 5: return EnableCap.Light5;
                    case 6: return EnableCap.Light6;
                    case 7: return EnableCap.Light7;
                    default: return (EnableCap)0;
                }
            }
        }

        /// <summary>
        /// Название источника
        /// </summary>
        /// <returns></returns>
        protected LightName LightName
        {
            get
            {
                switch (LightIndex)
                {
                    case 0: return LightName.Light0;
                    case 1: return LightName.Light1;
                    case 2: return LightName.Light2;
                    case 3: return LightName.Light3;
                    case 4: return LightName.Light4;
                    case 5: return LightName.Light5;
                    case 6: return LightName.Light6;
                    case 7: return LightName.Light7;
                    default: return (LightName)0;
                }
            }
        }

        /// <summary>
        /// Управление источником света
        /// </summary>
        /// <param name="Par"></param>
        /// <param name="Arg"></param>
        protected void Light(LightParameter Par, float[] Arg)
        {
            GL.Light(LightName, Par, Arg);
        }

        /// <summary>
        /// Включить источник света
        /// </summary>
        public void Enable()
        {
            if (!CheckId()) return;
            GL.Enable(LightCap);
        }

        /// <summary>
        /// Выключить источник света
        /// </summary>
        public void Disable()
        {
            if (!CheckId()) return;

            GL.Disable(LightCap);
        }

        /// <summary>
        /// Поместить на сцене
        /// </summary>
        public virtual void Put()
        {

        }
    }
}
