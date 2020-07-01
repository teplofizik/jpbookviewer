using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace G3D.Scripts
{
    public class GLFrame
    {
        public Vector3 Location;
        public Vector3 Up;
        public Vector3 Forward;

        public GLFrame()
        {
            Location = Vector3.Zero;
            Up = Vector3.Zero;
            Forward = Vector3.Zero;

            Up.Y = 1;
            Forward.Z = -1;
        }

        public void ApplyCameraTransform()
        {
            var Flipped = -Forward;
            var AxisX = Vector3.Cross(Up, Flipped);

            Matrix4 M = Matrix4.Identity;
            M[0, 0] = AxisX.X;
            M[1, 0] = AxisX.Y;
            M[2, 0] = AxisX.Z;

            M[0, 1] = Up.X;
            M[1, 1] = Up.Y;
            M[2, 1] = Up.Z;

            M[0, 2] = Flipped.X;
            M[1, 2] = Flipped.Y;
            M[2, 2] = Flipped.Z;
            
            GL.MultMatrix(ref M);
            GL.Translate(-Location);
        }
        
        public void ApplyLookAtTransform(float X, float Y, float Z)
        {
            var M = Matrix4.LookAt(Location, new Vector3(X, Y, Z), Up);
            GL.MultMatrix(ref M);
        }

        /// <summary>
        /// Переместить вперёд
        /// </summary>
        /// <param name="Step"></param>
        public void MoveFrameForward(float Step)
        {
            Location += Forward * Step;
        }

        /// <summary>
        /// Переместить вверх
        /// </summary>
        /// <param name="Step"></param>
        public void MoveFrameUp(float Step)
        {
            Location += Up * Step;
        }
        
        /// <summary>
        /// Переместить вправо
        /// </summary>
        /// <param name="Step"></param>
        public void MoveFrameRight(float Step)
        {
            Location += Vector3.Cross(Up, Forward) * Step;
        }

        /// <summary>
        /// Вращать вокруг оси Y
        /// </summary>
        /// <param name="Step"></param>
        public void RotateFrameLocalY(float Angle)
        {
            var M = GetRotationMatrix(Angle, Up);
            Forward = RotateVector(Forward, M);
        }

        /// <summary>
        /// Вращать вокруг оси X
        /// </summary>
        /// <param name="Step"></param>
        public void RotateFrameLocalX(float Angle)
        {
            var M = GetRotationMatrix(Angle, Vector3.Cross(Up, Forward));
            Forward = RotateVector(Forward, M);
            Up = RotateVector(Up, M);
        }

        /// <summary>
        /// Вращать вокруг оси Z
        /// </summary>
        /// <param name="Step"></param>
        public void RotateFrameLocalZ(float Angle)
        {
            var M = GetRotationMatrix(Angle, Forward);
            Up = RotateVector(Up, M);
        }

        private Vector3 RotateVector(Vector3 axis, Matrix4 M)
        {
            var R = Vector3.Zero;

            R.X = M[0, 0] * axis.X + M[1, 0] * axis.Y + M[2, 0] * axis.Z;
            R.Y = M[0, 1] * axis.X + M[1, 1] * axis.Y + M[2, 1] * axis.Z;
            R.Z = M[0, 2] * axis.X + M[1, 2] * axis.Y + M[2, 2] * axis.Z;
            
            return R;
        }

        private Matrix4 GetRotationMatrix(float angle, Vector3 axis)
        {
            Matrix4 M = Matrix4.Identity;

            if(axis != Vector3.Zero)
            {
                axis.Normalize();

                float sinSave = Convert.ToSingle(Math.Sin(angle));
                float cosSave = Convert.ToSingle(Math.Cos(angle));
                float oneMinusCos = 1 - cosSave;

                float xx = axis.X * axis.X;
                float yy = axis.Y * axis.Y;
                float zz = axis.Z * axis.Z;
                float xy = axis.X * axis.Y;
                float yz = axis.Y * axis.Z;
                float zx = axis.Z * axis.X;
                float xs = axis.X * sinSave;
                float ys = axis.Y * sinSave;
                float zs = axis.Z * sinSave;

                M[0, 0] = (oneMinusCos * xx) + cosSave;
                M[1, 0] = (oneMinusCos * xy) - zs;
                M[2, 0] = (oneMinusCos * zx) + ys;

                M[0, 1] = (oneMinusCos * xy) + zs;
                M[1, 1] = (oneMinusCos * yy) + cosSave;
                M[2, 1] = (oneMinusCos * yz) - xs;

                M[0, 2] = (oneMinusCos * zx) - ys;
                M[1, 2] = (oneMinusCos * yz) + xs;
                M[2, 2] = (oneMinusCos * zz) + cosSave;
            }

            return M;
        }
    }
}
