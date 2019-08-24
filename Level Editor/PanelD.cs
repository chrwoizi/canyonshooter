using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace CanyonShooter.Editor
{
    public partial class PanelD : Panel
    {
        public PanelD()
        {
            InitializeComponent();
        }

        public PanelD(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            const Int32 WM_ERASEBKGND = 20;
            if (m.Msg != WM_ERASEBKGND)
                base.WndProc(ref m);
        }
    }
}
