using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace JefViewer.SewViewer.Types
{
    class PageRect
    {
        /// <summary>
        /// Прямоугольник
        /// </summary>
        public Rectangle Rect;

        /// <summary>
        /// Иероглиф
        /// </summary>
        public string Kanji;

        public PageRect(Rectangle R, string Text)
        {
            Rect = R;
            Kanji = Text;
        }
    }
}
