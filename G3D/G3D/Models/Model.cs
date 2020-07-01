using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace G3D.Models
{
    public class Model
    {
        protected List<Vector3> Vertex = new List<Vector3>();
        protected List<Vector3> Normal = new List<Vector3>();
        protected List<Vector2> TexCoord = new List<Vector2>();
        protected List<Face> Faces = new List<Face>();

        public Model()
        {

        }

        private void DrawWith(PrimitiveType T)
        {
            for (int f = 0; f < Faces.Count; f++)
            {
                var F = Faces[f];
                GL.Begin(T);

                for (int i = 0; i < F.Indexes.Count; i++)
                {
                    var I = F.Indexes[i];
                    GL.Normal3(Normal[I.Normal]);
                    GL.TexCoord2(TexCoord[I.Texture]);
                    GL.Vertex3(Vertex[I.Vertex]);
                }
                GL.End();
            }
        }

        private void DrawWithCond(PrimitiveType T, Vector3 Vect, bool Similar)
        {
            for (int f = 0; f < Faces.Count; f++)
            {
                var F = Faces[f];

                // Проверка поверхности
                bool Sim = IsVectorSimilar(Normal[F.Indexes[0].Normal], Vect, 0.5f);

                if (Sim == Similar)
                {
                    GL.Begin(T);
                    for (int i = 0; i < F.Indexes.Count; i++)
                    {
                        var I = F.Indexes[i];
                        GL.Normal3(Normal[I.Normal]);
                        GL.TexCoord2(TexCoord[I.Texture]);
                        GL.Vertex3(Vertex[I.Vertex]);
                    }
                    GL.End();
                }
            }
        }

        private bool IsVectorSimilar(Vector3 A, Vector3 B, float Diff)
        {
            A = Vector3.Normalize(A);
            B = Vector3.Normalize(B);

            var D = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2) + Math.Pow(A.Z - B.Z, 2));

            return (D < Diff);
        }


        public void DrawLines()
        {
            DrawWith(PrimitiveType.LineLoop);
        }

        public void Draw()
        {
            DrawWith(PrimitiveType.Polygon);
        }

        public void DrawCond(Vector3 Vect, bool Similar)
        {
            DrawWithCond(PrimitiveType.Polygon, Vect, Similar);
        }

        protected class Face
        {
            public List<VIndex> Indexes = new List<VIndex>();
        }

        protected class VIndex
        {
            public int Vertex;
            public int Texture;
            public int Normal;

            public VIndex(int V, int T, int N)
            {
                Vertex = V;
                Texture = T;
                Normal = N;
            }
        }
    }
}
