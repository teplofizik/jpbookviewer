using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace G3D.Scripts
{
    public class Geometry
    {
        /// <summary>
        /// Установить перспективную проекцию
        /// </summary>
        /// <param name="fovy">Угол в радианах</param>
        /// <param name="aspect">Прямоугольность</param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public static void Perspective(float fovy, float aspect, float zNear, float zFar)
        {
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar);
            GL.MultMatrix(ref perspective);
        }
        
        public static void LookAt(float cx, float cy, float cz, 
                                  float tx, float ty, float tz, 
                                  float ux, float uy, float uz)
        {
            Matrix4 lookat = Matrix4.LookAt(new Vector3(cx, cy, cz),
                                            new Vector3(tx, ty, tz),
                                            new Vector3(ux, uy, uz));
            GL.MultMatrix(ref lookat);
        }

        public static void SetNormal(double x, double y, double z)
        {
            Vector3 V = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));
            V.Normalize();

            GL.Normal3(V);
        }

        public static Vector3 GetNormal(Vector3 A, Vector3 B, Vector3 C)
        {
            var V = Vector3.Cross(A - B, B - C);
            V.Normalize();

            return V;
        }

        public static void Torus(double ra, double rb, int nph, int nks)
        {
            for (int ksi = 0; ksi < nks; ++ksi)
            {
                GL.Begin(PrimitiveType.QuadStrip);
                for (int phi = nph; phi >= 0; --phi)
                {
                    double x, y, z;

                    // Определим центр вторичной окружности
                    x = ra * Math.Cos(2 * phi * Math.PI / nph);
                    y = ra * Math.Sin(2 * phi * Math.PI / nph);
                    z = 0;
                    {
                        double x1, y1, z1;
                        x1 = x + rb * Math.Cos(2 * phi * Math.PI / nph) * Math.Cos(2 * ksi * Math.PI / nks);
                        y1 = y + rb * Math.Sin(2 * phi * Math.PI / nph) * Math.Cos(2 * ksi * Math.PI / nks);
                        z1 = z + rb * Math.Sin(2 * ksi * Math.PI / nks);

                        GL.Normal3(x1, y1, z1);//нормаль направлена от центра
                        GL.TexCoord2((double)phi / (double)nph, (double)ksi / (double)nks);
                        GL.Vertex3(x1, y1, z1);
                    }
                    {
                        double x2, y2, z2;
                        x2 = x + rb * Math.Cos(2 * phi * Math.PI / nph) * Math.Cos(2 * (ksi + 1) * Math.PI / nks);
                        y2 = y + rb * Math.Sin(2 * phi * Math.PI / nph) * Math.Cos(2 * (ksi + 1) * Math.PI / nks);
                        z2 = z + rb * Math.Sin(2 * (ksi+1) * Math.PI / nks);

                        GL.Normal3(x2, y2, z2);//нормаль направлена от центра
                        GL.TexCoord2((double)phi / (double)nph, (double)(ksi + 1) / (double)nks);
                        GL.Vertex3(x2, y2, z2);
                    }
                }
                GL.End();
            }
        }
        
        /// <summary>
        /// Сфера
        /// </summary>
        /// <param name="r">Радиус</param>
        /// <param name="nx">Количество сегментов по радиусу</param>
        /// <param name="ny">Количество сегментов по высоте</param>
        public static void Sphere(double r, int nx, int ny)
        {
            for (int iy = 0; iy < ny; ++iy)
            {
                GL.Begin(PrimitiveType.QuadStrip);
                for (int ix = 0; ix <= nx; ++ix)
                {
                    double x, y, z;

                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos(iy * Math.PI / ny);
                    SetNormal(x, y, z);//нормаль направлена от центра
                    GL.TexCoord2((double)ix / (double)nx, (double)iy / (double)ny);
                    GL.Vertex3(x, y, z);

                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos((iy + 1) * Math.PI / ny);
                    SetNormal(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)(iy + 1) / (double)ny);
                    GL.Vertex3(x, y, z);
                }
                GL.End();
            }
        }

        /// <summary>
        /// Куб со стороной заданного размера
        /// </summary>
        /// <param name="Size"></param>
        public static void Cube(float Size)
        {
            Size /= 2;
            Vector3[] Corners = new Vector3[]
            {
            new Vector3(-Size,  Size,  Size), new Vector3( Size,  Size,  Size),
            new Vector3( Size, -Size,  Size), new Vector3(-Size, -Size,  Size),
            new Vector3(-Size,  Size, -Size), new Vector3( Size,  Size, -Size),
            new Vector3( Size, -Size, -Size), new Vector3(-Size, -Size, -Size),
            };

            int[] Indexes = new int[] {0, 1, 2, 3,
                                       4, 5, 1, 0,
                                       3, 2, 6, 7,
                                       5, 4, 7, 6,
                                       1, 5, 6, 2,
                                       4, 0, 3, 7};

            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < Indexes.Length / 4; i++)
            {
                GL.Normal3(GetNormal(Corners[Indexes[i * 4 + 2]], 
                                     Corners[Indexes[i * 4 + 1]], 
                                     Corners[Indexes[i * 4]]));
                GL.Vertex3(Corners[Indexes[i * 4 + 3]]);
                GL.Vertex3(Corners[Indexes[i * 4 + 2]]);
                GL.Vertex3(Corners[Indexes[i * 4 + 1]]);
                GL.Vertex3(Corners[Indexes[i * 4 + 0]]);
            }
            GL.End();
        }
    }
}
