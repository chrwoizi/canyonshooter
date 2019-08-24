using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CanyonShooter.DataLayer.Level;

namespace CanyonShooter.Editor
{
    public partial class CanyonTracker : UserControl
    {
        public CanyonTracker()
        {
            InitializeComponent();
        }

        public Level Level
        {
            get
            {
                if (!(Parent is Client)) return null;
                return ((Client)Parent).Level;
            }
        }
        public int LevelLength
        {
            get { if (Level == null) return 4; return Level.Blocks.Length; }
        }

        public int ActiveBlock
        {
            get { if (Level == null) return 0; return (int)((Client)Parent).ActiveBlock.Value - 1; }
            set { if (Level == null) return; ((Client)Parent).ActiveBlock.Value = value + 1; }
        }

        private void CanyonTracker_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.FillRectangle(Brushes.White, e.ClipRectangle);
            g.DrawRectangle(Pens.Black, 0, 0, Width-1, Height-1);

            float SegmentSize = (float)(Width - 1) / (float)LevelLength;
            if (SegmentSize > 30) SegmentSize = 30;

            for (int i = 0; i < LevelLength; i++)
            {
                int left = (int)((i) * SegmentSize);
                int right = (int)((i + 1) * SegmentSize);
                g.DrawLine(Pens.Black, right, 0, right, Height);
                g.FillRectangle(Brushes.White, left + 1, 1, right - left - 1, Height - 2);
                if (ActiveBlock == i)
                    g.DrawRectangle(Pens.Red, left + 2, 2, right - left - 4, Height - 5);
            }
        }

        private void CanyonTracker_MouseClick(object sender, MouseEventArgs e)
        {
            float SegmentSize = (float)(Width - 1) / (float)LevelLength;
            if (SegmentSize > 30) SegmentSize = 30;

            for (int i = 0; i < LevelLength; i++)
            {
                int left = (int)((i) * SegmentSize);
                int right = (int)((i + 1) * SegmentSize);
                if (e.X >= left && e.X <= right)
                {
                    ActiveBlock = i;
                    return;
                }
            }
        }

        private void CanyonTracker_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
