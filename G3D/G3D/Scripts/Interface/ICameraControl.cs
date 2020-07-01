using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G3D.Scripts.Interface
{
    public interface ICameraControl
    {
        /// <summary>
        /// Вращение по вертикали
        /// </summary>
        /// <param name="Angle"></param>
        void RotateVertical(float Angle);

        /// <summary>
        /// Вращение по горизонтали
        /// </summary>
        /// <param name="Angle"></param>
        void RotateHorizontal(float Angle);
    }
}
