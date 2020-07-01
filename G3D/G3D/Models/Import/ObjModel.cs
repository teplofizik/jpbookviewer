using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using OpenTK;
using System.Globalization;

namespace G3D.Models.Import
{
    public class ObjModel : Model
    {
        public ObjModel(string Filename)
        {
            Load(Filename);
        }

        private void Load(string Filename)
        {
            var Lines = File.ReadAllLines(Filename);

            foreach(var L in Lines)
            {
                var TL = L.Trim();
                if (TL.Length == 0) continue;
                var Line = TL.Split(new char[] { ' ' });

                switch(GetLineType(Line))
                {
                    case "v": Vertex.Add(GetVector3(Line)); break;
                    case "vt": TexCoord.Add(GetVector2(Line)); break;
                    case "vn": Normal.Add(GetVector3(Line)); break;
                    case "f": LoadFace(Line); break;
                }
            }
        }

        /// <summary>
        /// Тип строки
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        private string GetLineType(string[] Line)
        {
            if (Line.Length >= 1)
            {
                return Line[0];
            }
            else
                return "Unknown";
        }

        private float GetConvertedValue(string Value)
        {
            float Res;
            if (float.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out Res))
                return Res;
            else
                return 0;
        }

        /// <summary>
        /// Выделить вектор из двух компонент
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        private Vector2 GetVector2(string[] Line)
        {
            if (Line.Length == 3)
            {
                float X = GetConvertedValue(Line[1]);
                float Y = GetConvertedValue(Line[2]);
                return new Vector2(X, Y);
            }
            else
                return new Vector2(0, 0);
        }

        /// <summary>
        /// Выделить вектор из трёх компонент
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        private Vector3 GetVector3(string[] Line)
        {
            if (Line.Length == 4)
            {
                float X = GetConvertedValue(Line[1]);
                float Y = GetConvertedValue(Line[2]);
                float Z = GetConvertedValue(Line[3]);
                return new Vector3(X, Y, Z);
            }
            else
                return new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Загрузить таблицу индексов
        /// </summary>
        /// <param name="Line"></param>
        private void LoadFace(string[] Line)
        {
            var F = new Face();
            for(int i = 1; i < Line.Length; i++)
            {
                string[] Indexes = Line[i].Split(new char[] { '/' });

                int V = Convert.ToInt32(Indexes[0]) - 1;
                int T = Convert.ToInt32(Indexes[1]) - 1;
                int N = Convert.ToInt32(Indexes[2]) - 1;
                F.Indexes.Add(new VIndex(V, T, N));
            }
            Faces.Add(F);
        }
    }
}
