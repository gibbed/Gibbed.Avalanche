using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gibbed.Avalanche.ModelViewer
{
    public class ModelViewRenderPanel : Panel
    {
        public ModelViewRenderPanel() : base()
        {
            this.SetStyle(ControlStyles.Selectable, true);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Click += new EventHandler(OnClick);
        }

        private void OnClick(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
