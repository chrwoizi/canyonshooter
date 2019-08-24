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
    public partial class CanyonOutline : UserControl
    {
        private const MouseButtons mouseButtonsMove = MouseButtons.Right;
        private const MouseButtons mouseButtonsSelect = MouseButtons.Left;
        private Level propLevel;
        private CanyonOutlineType propOutlineType = CanyonOutlineType.XY;
        private int propZoom = 256;
        private Point propOffset = new Point();
        private Point lastCursorLocation = new Point();
        private bool backgroundMoving;
        private string propCaption;
        private Cursor previousCursor;
        private int propSelectedIndex = -1;
        private int selectedCacheElement;
        private event OutlineEventHandler propUpdate;


        public CanyonOutline()
        {
            InitializeComponent();
            this.CenterCanyon();
            this.Invalidate();
        }



        #region Properties
        /// <summary>
        /// Occurs when the selected index has changed or an element has been moved.
        /// </summary>
        public event OutlineEventHandler Update
        {
            add
            {
                this.propUpdate += value;
            }
            remove
            {
                this.propUpdate -= value;
            }
        }



        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public Level Level
        {
            get
            {
                return this.propLevel;
            }
            set
            {
                this.propLevel = value;
                this.Invalidate();
            }
        }


        /// <summary>
        /// Gets or sets the type of the outline. The type specifies the view plane as XY, XZ or YZ.
        /// </summary>
        /// <value>The type of the outline.</value>
        public CanyonOutlineType OutlineType
        {
            get
            {
                return this.propOutlineType;
            }
            set
            {
                this.propOutlineType = value;
                this.Invalidate();
            }
        }



        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        /// <value>The zoom.</value>
        public int Zoom
        {
            get
            {
                return this.propZoom;
            }
            set
            {
                this.propZoom = value;
                this.Invalidate();
            }
        }


        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Point Offset
        {
            get
            {
                return this.propOffset;
            }
            set
            {
                this.propOffset = value;
                this.Invalidate();
            }
        }


        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get
            {
                return this.propCaption;
            }
            set
            {
                this.propCaption = value;
                this.Invalidate();
            }
        }


        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get
            {
                return this.propSelectedIndex;
            }
            set
            {
                this.propSelectedIndex = value;
                this.Invalidate();
            }
        }
        #endregion



        /// <summary>
        /// Invalidates the entire surface of the control and causes the control to be redrawn.
        /// </summary>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public new void Invalidate()
        {
            base.Invalidate();
            this.panelD1.Invalidate();
        }



        /// <summary>
        /// Handles the Paint event of the panelD1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void panelD1_Paint(object sender, PaintEventArgs e)
        {
            if (this.Level == null)
                return;

            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.Black, e.ClipRectangle);

            // calculate the element points from cache
            Point[] points = this.CalcPoints();

            // draw lines to connect all points
            g.DrawLines(Pens.Green, points);

            // calculate rects from points
            Rectangle[] rects = this.CalcRects(points);

            // draw all rects
            for (int i = 0; i < rects.Length; i++)
            {
                Brush brush = Brushes.Green;
                if (i == this.SelectedIndex)
                {
                    brush = Brushes.YellowGreen;
                }
                if (i == 0)
                {
                    brush = Brushes.Blue;
                }
                if (i == 0 && i == this.SelectedIndex)
                {
                    brush = Brushes.LightBlue;
                }

                g.FillRectangle(brush, rects[i]);
            }

            // Caption ausgeben
            Font f = new Font(Font.FontFamily, 8, FontStyle.Bold);
            g.DrawString(this.Caption, f, Brushes.White, 3, 3);
        }



        /// <summary>
        /// Calcs the points which represent the level elements.
        /// </summary>
        /// <returns></returns>
        private Point[] CalcPoints()
        {
            Point[] points = new Point[this.Level.Cache.Length];
            for (int i = 0; i < points.Length; i++)
            {
                PointF p;

                // get point coordinates depending on the outline type
                switch (this.OutlineType)
                {
                    case CanyonOutlineType.XZ:
                        p = new PointF(Level.Cache[i].APosX, Level.Cache[i].APosZ);
                        break;
                    case CanyonOutlineType.XY:
                        p = new PointF(Level.Cache[i].APosX, -Level.Cache[i].APosY);
                        break;
                    case CanyonOutlineType.ZY:
                        p = new PointF(Level.Cache[i].APosZ, -Level.Cache[i].APosY);
                        break;
                    default:
                        return null;
                }

                // calculate point coordinates with offset and zoom
                points[i] = new Point(
                    Offset.X + (int)(p.X * Zoom / 1024),
                    Offset.Y + (int)(p.Y * Zoom / 1024));
            }
            return points;
        }



        /// <summary>
        /// Calcs the rectangles which represent the level elements.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private Rectangle[] CalcRects(Point[] points)
        {
            Rectangle[] rects = new Rectangle[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                rects[i] = new Rectangle(points[i].X - 2, points[i].Y - 2, 5, 5);
            }
            return rects;
        }



        /// <summary>
        /// Handles the MouseDown event of the panelD1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void panelD1_MouseDown(object sender, MouseEventArgs e)
        {
            // move background
            if (e.Button == mouseButtonsMove)
            {
                this.backgroundMoving = true;
                this.lastCursorLocation = new Point(e.X, e.Y);
                this.previousCursor = this.panelD1.Cursor;
                this.panelD1.Cursor = Cursors.Hand;
            }

            // move level element
            if (e.Button == mouseButtonsSelect)
            {
                Rectangle[] rects = this.CalcRects(this.CalcPoints());
                for (int i = 0; i < rects.Length; i++)
                {
                    if (rects[i].Contains(e.Location))
                    {
                        this.selectedCacheElement = i;
                        this.SelectedIndex = i;
                        this.lastCursorLocation = new Point(e.X, e.Y);
                        if (this.propUpdate != null)
                            this.propUpdate(this, new OutlineEventArgs(this.SelectedIndex));
                        this.panelD1.Invalidate();
                        break;
                    }
                }
            }
        }



        /// <summary>
        /// Handles the MouseUp event of the panelD1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void panelD1_MouseUp(object sender, MouseEventArgs e)
        {
            // move background
            if (e.Button == mouseButtonsMove)
            {
                this.backgroundMoving = false;
                this.lastCursorLocation = new Point();
                this.panelD1.Cursor = this.previousCursor;
            }

            // move level element
            if (e.Button == mouseButtonsSelect)
            {
                selectedCacheElement = -1;
                this.lastCursorLocation = new Point();
            }
        }



        /// <summary>
        /// Handles the MouseMove event of the panelD1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void panelD1_MouseMove(object sender, MouseEventArgs e)
        {
            // move background
            if (this.backgroundMoving)
            {
                if (this.lastCursorLocation.IsEmpty) 
                    return;

                Point p = new Point(e.X - this.lastCursorLocation.X, e.Y - this.lastCursorLocation.Y);
                lastCursorLocation = new Point(e.X, e.Y);
                this.propOffset.Offset(p);
                this.panelD1.Invalidate();
            }

            // move level element
            if (selectedCacheElement > -1)
            {
                if (this.lastCursorLocation.IsEmpty)
                    return;

                // calculate the difference between current and last cursor position
                Point difference = e.Location;
                difference.Offset(new Point(-lastCursorLocation.X, -lastCursorLocation.Y));                

                // zoom the difference
                difference = new Point(
                    (int)(difference.X * 1024 / Zoom),
                    (int)(difference.Y * 1024 / Zoom));

                // add difference to cache-element
                switch (this.OutlineType)
                {
                    case CanyonOutlineType.XZ:
                        Level.SetBlockAbsolutePosition(
                            Level.Cache[SelectedIndex].APosX + difference.X,
                            Level.Cache[SelectedIndex].APosY,
                            Level.Cache[SelectedIndex].APosZ + difference.Y,
                            SelectedIndex);
                        Level.CalcCache();
                        //this.Level.Cache[this.SelectedIndex].APosX += difference.X;
                        //this.Level.Cache[this.SelectedIndex].APosZ += difference.Y;
                        break;
                    case CanyonOutlineType.XY:
                        Level.SetBlockAbsolutePosition(
                            Level.Cache[SelectedIndex].APosX + difference.X,
                            Level.Cache[SelectedIndex].APosY - difference.Y,
                            Level.Cache[SelectedIndex].APosZ,
                            SelectedIndex);
                        Level.CalcCache();
                        //this.Level.Cache[this.SelectedIndex].APosX += difference.X;
                        //this.Level.Cache[this.SelectedIndex].APosY += -difference.Y;
                        break;
                    case CanyonOutlineType.ZY:
                        Level.SetBlockAbsolutePosition(
                            Level.Cache[SelectedIndex].APosX,
                            Level.Cache[SelectedIndex].APosY - difference.Y,
                            Level.Cache[SelectedIndex].APosZ + difference.X,
                            SelectedIndex);
                        Level.CalcCache();
                        //this.Level.Cache[this.SelectedIndex].APosZ += difference.X;
                        //this.Level.Cache[this.SelectedIndex].APosY += -difference.Y;
                        break;
                    default:
                        return;
                }

                // raise the update event to notify current changes
                if (this.propUpdate != null)
                    this.propUpdate(this, new OutlineEventArgs(SelectedIndex));

                // cause repaint
                this.panelD1.Invalidate();

                // set last cursor location to current cursor location
                this.lastCursorLocation = e.Location;
            }
        }


        /// <summary>
        /// Centers the canyon.
        /// </summary>
        public void CenterCanyon()
        {
            this.Offset = new Point(this.panelD1.Width / 2, this.panelD1.Height / 2);
        }
    }



    /// <summary>
    /// Specifies the view plane of the outline.
    /// </summary>
    public enum CanyonOutlineType
    {
        /// <summary>
        /// XY plane
        /// </summary>
        XY,
        /// <summary>
        /// XZ plane
        /// </summary>
        XZ,
        /// <summary>
        /// ZY plane
        /// </summary>
        ZY
    }



    /// <summary>
    /// Represents a method which handles the update event of the CanyonOutline class.
    /// </summary>
    public delegate void OutlineEventHandler(object sender, OutlineEventArgs e);


    /// <summary>
    /// Contains the update event data.
    /// </summary>
    class OutlineEventArgs : EventArgs
    {
        private int propSelectedIndex;



        public OutlineEventArgs(int selectedIndex)
        {
            this.propSelectedIndex = selectedIndex;
        }



        public int SelectedIndex
        {
            get
            {
                return this.propSelectedIndex;
            }
        }

    }
}
