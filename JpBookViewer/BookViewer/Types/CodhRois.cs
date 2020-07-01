using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JefViewer.SewViewer.Types
{
    class CodhRois
    {
        public string Name;
        public string[] Images;
        public CodhRoisRectangle[] Rects;

        // 100249476
        // ./100249476_coordinate.csv
        // ./100249476_report.csv
        // ./images/
        // ./images/100249476_00001_1.jpg
        // ./images/100249476_00002_1.jpg
        // ./images/100249476_00002_2.jpg
        // ./images/100249476_00003_1.jpg
        // ./images/
        // ./images/

        public CodhRois(string Dir)
        {
            Name = Path.GetFileName(Dir);
            var Lines = File.ReadAllLines($"{Dir}//{Name}_coordinate.csv");
            var TempRects = new List<CodhRoisRectangle>();

            for (int i = 1; i < Lines.Length; i++)
                TempRects.Add(new CodhRoisRectangle(Lines[i]));
            Rects = TempRects.ToArray();

            Images = Directory.GetFiles($"{Dir}\\images", "*.jpg");
        }

        public int GetPageId(string Image)
        {
            for(int i = 0; i < Images.Length; i++)
            {
                if (Images[i].IndexOf(Image) > 0)
                    return i;
            }

            return -1;
        }
        
        static public bool CheckIsRois(string Dir)
        {
            var Name = Path.GetFileName(Dir);
            if (!File.Exists($"{Dir}//{Name}_coordinate.csv")) return false;
            if (!Directory.Exists($"{Dir}//images")) return false;

            return true;
        }

        public class CodhRoisRectangle
        {
            /// <summary>
            /// Char code
            /// </summary>
            public int Unicode;
            /// <summary>
            /// Image file name
            /// </summary>
            public string Image;

            /// <summary>
            /// X
            /// </summary>
            public int X;

            /// <summary>
            /// Y
            /// </summary>
            public int Y;

            /// <summary>
            /// Block Id
            /// </summary>
            public string BlockId;

            /// <summary>
            /// Char Id (on page)
            /// </summary>
            public string CharId;

            /// <summary>
            /// Width
            /// </summary>
            public int Width;

            /// <summary>
            /// Height
            /// </summary>
            public int Height;

            public string CharView => Char.ConvertFromUtf32(Unicode);

            public Rectangle Rect => new Rectangle(X, Y, Width, Height);
            
            public CodhRoisRectangle(string Line)
            {
                var Parts = Line.Split(',');

                //Unicode,Image,X,Y,Block ID,Char ID,Width,Height
                //U+3082,100249476_00003_2,236,1830,B0001,C0133,47,108

                if(Parts.Length >= 8)
                {
                    if (Parts[0].Substring(0, 2).CompareTo("U+") == 0)
                        Unicode = Convert.ToInt32(Parts[0].Substring(2), 16);
                    else
                        Unicode = 0;

                    Image = Parts[1];
                    X = Convert.ToInt32(Parts[2]);
                    Y = Convert.ToInt32(Parts[3]);

                    BlockId = Parts[4];
                    CharId = Parts[5];

                    Width = Convert.ToInt32(Parts[6]);
                    Height = Convert.ToInt32(Parts[7]);
                }

            }
        }
    }
}
