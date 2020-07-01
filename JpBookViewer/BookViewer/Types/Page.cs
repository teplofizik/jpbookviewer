using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JefViewer.SewViewer.Types
{
    class Page
    {
        public string Image;
        public List<PageRect> Rects = new List<PageRect>();

        public Page(string FN)
        {
            Image = FN;
        }

        public Page()
        {
            Image = null;
        }
    }
}
