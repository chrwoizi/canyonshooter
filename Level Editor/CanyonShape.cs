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
    public partial class CanyonShape : UserControl
    {
        private const MouseButtons mouseButtonsMoving = MouseButtons.Left;
        private Level propLevel;
        private int propSelectedOutlineIndex;
        private int indexMoving = -1;
        private int propSelectedShapeNode;
        private Point lastCursorLocation = new Point();
        private Point offset;
        private int size;
        private int modifier;
        private Point center;


        public CanyonShape()
        {
            InitializeComponent();
        }


        #region Properties
        public int SelectedOutlineIndex
        {
            get
            {
                return this.propSelectedOutlineIndex;
            }
            set
            {
                this.propSelectedOutlineIndex = value;
                this.panelD1.Invalidate();
            }
        }


        public int SelectedShapeNode
        {
            get
            {
                return this.propSelectedShapeNode;
            }
            set
            {
                this.propSelectedShapeNode = value;
            }
        }


        public Level Level
        {
            get
            {
                return this.propLevel;
            }
            set
            {
                this.propLevel = value;
            }
        }
        #endregion



        public new void Invalidate()
        {
            base.Invalidate();
            this.panelD1.Invalidate();
        }

        
        
        private void panelD1_Paint(object sender, PaintEventArgs e)
        {
            if (this.Level == null)
                return;

            BufferedGraphicsContext gc = BufferedGraphicsManager.Current;
            gc.MaximumBuffer = new Size(1024, 1024);
            if (e.ClipRectangle.Width < 1 || e.ClipRectangle.Height < 1)
                return;
            BufferedGraphics gb = gc.Allocate(e.Graphics, e.ClipRectangle);
            
            Graphics g = gb.Graphics;
            g.FillRectangle(Brushes.DarkGray, e.ClipRectangle);


            if (this.panelD1.Width > this.panelD1.Height)
            {
                size = this.panelD1.Height;
                offset = new Point((this.panelD1.Width - this.panelD1.Height) >> 1, 0);
            }
            else
            {
                size = this.panelD1.Width;
                offset = new Point(0, (this.panelD1.Height - this.panelD1.Width) >> 1);
            }
            g.FillRectangle(Brushes.Black, offset.X, offset.Y, size, size);

            modifier = size >> 1;
            center = new Point(offset.X + modifier, offset.Y + modifier);


            if (Level.Blocks.Length <= this.SelectedOutlineIndex)
                this.SelectedOutlineIndex = 0;

            Vec2[] Walls = Level.Blocks[this.SelectedOutlineIndex].Walls;
            Point[] p = new Point[Walls.Length];
            for (int i = 0; i < Walls.Length; i++)
            {
                p[i].X = center.X + (int)(Walls[i].X * modifier);
                p[i].Y = center.Y - (int)(Walls[i].Y * modifier);
            }
            
            g.DrawLines(Pens.Green, p);
            for (int i = 0; i < Walls.Length; i++)
            {
                Brush brush = Brushes.Green;
                if (i == this.SelectedShapeNode)
                {
                    brush = Brushes.GreenYellow;
                }
                g.FillRectangle(brush, new Rectangle(p[i].X - 2, p[i].Y - 2, 5, 5));
            }

            Font f = new Font(Font.FontFamily, 8, FontStyle.Bold);
            g.DrawString("Canyon Form", f, Brushes.White, 3, 3);

            gb.Render(e.Graphics);
        }

        private void CanyonShape_Resize(object sender, EventArgs e)
        {
            this.panelD1.Invalidate();
        }

        private void panelD1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == mouseButtonsMoving)
            {
                Vec2[] Walls = Level.Blocks[this.SelectedOutlineIndex].Walls;
                Point p = new Point();
                Rectangle rect = new Rectangle();
                this.lastCursorLocation = e.Location;
                for (int i = 0; i < Walls.Length; i++)
                {
                    p.X = center.X + (int)(Walls[i].X * modifier);
                    p.Y = center.Y - (int)(Walls[i].Y * modifier);
                    rect = new Rectangle(p.X - 2, p.Y - 2, 5, 5);
                    if (rect.Contains(e.Location))
                    {
                        this.indexMoving = i;
                        this.SelectedShapeNode = i;
                        this.panelD1.Invalidate();
                        break;
                    }
                }
            }
        }

        private void panelD1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == mouseButtonsMoving)
            {
                this.indexMoving = -1;
            }
        }

        private void panelD1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.indexMoving > -1)
            {
                Vec2[] Walls = Level.Blocks[this.SelectedOutlineIndex].Walls;

                // calculate the difference between current and last cursor position
                PointF difference = new PointF(
                    (e.Location.X - lastCursorLocation.X) / (float)modifier,
                    (lastCursorLocation.Y - e.Location.Y) / (float)modifier);

                                
                //Walls[this.indexMoving].X = (float)(e.X - center.X) / (float)modifier;
                //Walls[this.indexMoving].Y = (float)(center.Y - e.Y) / (float)modifier;
                Walls[this.indexMoving].X += difference.X;
                Walls[this.indexMoving].Y += difference.Y;
                this.panelD1.Invalidate();

                this.lastCursorLocation = e.Location;
            }
        }
    }
}
