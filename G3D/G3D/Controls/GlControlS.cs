using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;

namespace G3D.Controls
{
    public class GlControlS : OpenTK.GLControl
    {
       // public event MouseEventHandler MouseWheel;

        public GlControlS() : base(new GraphicsMode(32, 24, 8, 4))
        {

        }

     //   protected override void OnMouseWheel(MouseEventArgs e)
     //   {
     //       base.OnMouseWheel(e);

    //        MouseWheel?.Invoke(this, e);
    //    }
    }
}
