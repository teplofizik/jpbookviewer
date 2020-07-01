using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JefViewer.SewViewer
{
    class BookWithMenuRenderer : BookRectRenderer
    {
        private void DrawMenu()
        {


        }

        protected override void DrawOverlay()
        {
            // Меню обрабатываем первым этапом
            DrawMenu();

            base.DrawOverlay();
        }
    }
}
