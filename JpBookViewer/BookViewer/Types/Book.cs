using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace JefViewer.SewViewer.Types
{
    class Book
    {
        public List<Page> Pages = new List<Page>();
        public string Name = null;
        public string Dir;

        public void Load(string Dir)
        {
            Pages.Clear();

            var CR = new CodhRois(Dir);

            this.Dir = Dir;
            Name = CR.Name;
            foreach (var I in CR.Images)
            {
                Pages.Add(new Page(I));
            }
            foreach (var R in CR.Rects)
            {
                var Page = CR.GetPageId(R.Image);

                if (Page >= 0)
                {
                    var PR = new PageRect(R.Rect, R.CharView);
                    Pages[Page].Rects.Add(PR);
                }
            }
        }

        public void ProcessToWeb()
        {
            Directory.CreateDirectory($"{Dir}/web/");
            Directory.CreateDirectory($"{Dir}/web/data/");
            Directory.CreateDirectory($"{Dir}/web/images/");
            SaveImageList($"{Dir}/web/list.txt");
        }

        private void SaveImageList(string FileName)
        {
            var Lines = new List<string>();

            foreach (var P in Pages)
            {
                var FN = Path.GetFileName(P.Image);
                var B = new Bitmap(P.Image);
                float Scale = 1000.0f / B.Width;
                var BS = new Bitmap(B, new Size(1000, Convert.ToInt32(B.Height * Scale)));
                Lines.Add($"{FN}\t{P.Rects.Count}\t{BS.Width}\t{BS.Height}");
                B.Dispose();

                SaveImageRects(P, $"{Dir}/web/data/{Path.GetFileNameWithoutExtension(FN)}.txt", Scale);
                SaveImage($"{Dir}/web/images/{FN}", BS);
            }

            File.WriteAllLines(FileName, Lines.ToArray());
        }

        private void SaveImageRects(Page P, string DestFN, float Scale)
        {
            var Lines = new List<string>();

            foreach (var R in P.Rects)
            {
                Lines.Add($"{R.Rect.X * Scale:0}\t{R.Rect.Y * Scale:0}\t{R.Rect.Width * Scale:0}\t{R.Rect.Height * Scale:0}\t{R.Kanji}");
            }
            File.WriteAllLines(DestFN, Lines.ToArray());
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public void SaveImage(string Filename, Image Img)
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(qualityEncoder, 70L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            
            Img.Save(Filename, jpgEncoder, myEncoderParameters);
        }

    }
}
