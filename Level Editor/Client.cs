using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CanyonShooter.DataLayer.Level;
using CanyonShooter.DataLayer.Descriptions;

namespace CanyonShooter.Editor
{
    public partial class Client : Form
    {
        public String FileName = "";
        public Level level = new Level();
        private List<CanyonOutline> outlines = new List<CanyonOutline>();

        public int Zoom = 256;

        public Client()
        {
            InitializeComponent();

            canyonOutlineXZ.OutlineType = CanyonOutlineType.XZ;
            canyonOutlineXZ.Caption = "Oben (XZ)";
            outlines.Add(canyonOutlineXZ);

            canyonOutlineXY.OutlineType = CanyonOutlineType.XY;
            canyonOutlineXY.Caption = "Vorne (XY)";
            outlines.Add(canyonOutlineXY);

            canyonOutlineZY.OutlineType = CanyonOutlineType.ZY;
            canyonOutlineZY.Caption = "Links (ZY)";
            outlines.Add(canyonOutlineZY);

            foreach (CanyonOutline outline in outlines)
            {
                outline.Level = this.Level;
                outline.Update += canyonOutline_Update;
            }

            canyonShape1.Level = this.Level;
        }


        public Level Level
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
                foreach (CanyonOutline outline in outlines)
                {
                    outline.Level = value;
                }
                canyonShape1.Level = value;
            }
        }


        public void CenterCanyon()
        {
            foreach (CanyonOutline outline in outlines)
            {
                outline.CenterCanyon();
            }
            InvalidateCanyonShape();
        }

        private void canyonOutline_Update(object sender, OutlineEventArgs e)
        {
            ActiveBlock.Value = e.SelectedIndex + 1;
            canyonShape1.SelectedOutlineIndex = e.SelectedIndex;
            foreach (CanyonOutline outline in outlines)
            {
                if (outline != sender)
                {
                    outline.SelectedIndex = e.SelectedIndex;
                }
            }
        }

        private void Client_Load(object sender, EventArgs e)
        {
            Level.CalcCache();
            ActiveBlock.Value = 1;
            CenterCanyon();
        }

        private void ActiveBlock_ValueChanged(object sender, EventArgs e)
        {
            if (ActiveBlock.Value < 1) ActiveBlock.Value = 1;
            if (ActiveBlock.Value > Level.Blocks.Length) ActiveBlock.Value = Level.Blocks.Length;
            propertyGrid.SelectedObject = Level.Blocks[(int)ActiveBlock.Value - 1];

            // Outline anpassen
            foreach (CanyonOutline outline in this.outlines)
            {
                outline.SelectedIndex = (int)(ActiveBlock.Value - 1);
            }
            canyonShape1.SelectedOutlineIndex = (int)(ActiveBlock.Value - 1);
            
            InvalidateCanyonShape();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!(propertyGrid.SelectedObject is LevelBlock)) return; // ToDo
            Level.Blocks[(int)ActiveBlock.Value - 1] = (LevelBlock)propertyGrid.SelectedObject;
            RecalcCanyonShape();
        }

        private void AddSegment_Click(object sender, EventArgs e)
        {
            LevelBlock[] ol = Level.Blocks;
            Level.Blocks = new LevelBlock[ol.Length + 1];
            for (int i = 0; i < ol.Length; i++)
                Level.Blocks[i] = ol[i];
            Level.Blocks[ol.Length] = Level.GetStandardBlock();

            RecalcCanyonShape();
        }
        private void ZoomPlus_Click(object sender, EventArgs e)
        {
            if (Zoom < 256*256) Zoom = Zoom << 1;
            canyonOutlineXZ.Zoom = Zoom;
            canyonOutlineXY.Zoom = Zoom;
            canyonOutlineZY.Zoom = Zoom;
            InvalidateCanyonShape();
        }

        private void ZoomMinus_Click(object sender, EventArgs e)
        {
            if (Zoom > 1) Zoom = Zoom >> 1;
            canyonOutlineXZ.Zoom = Zoom;
            canyonOutlineXY.Zoom = Zoom;
            canyonOutlineZY.Zoom = Zoom;
            InvalidateCanyonShape();
        }

        public void RecalcCanyonShape()
        {
            Level.CalcCache();
            InvalidateCanyonShape();
        }

        public void InvalidateCanyonShape()
        {
            canyonTracker.Invalidate();
            canyonShape1.Invalidate();
            foreach (CanyonOutline outline in this.outlines)
            {
                outline.Invalidate();
            }

            ObjectBox.Items.Clear();
            ObjectBox.Items.Add("Canyon");
            if (Level.Blocks[(int)ActiveBlock.Value - 1].Objects != null)
            {
                foreach (EditorDescription obj in Level.Blocks[(int)ActiveBlock.Value - 1].Objects)
                {
                    ObjectBox.Items.Add("Objekt: "+obj.CreateType);
                }
            }
        }

        private void splitContainer3_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            splitContainer2.SplitterDistance = splitContainer3.SplitterDistance;
        }

        private void splitContainer2_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            splitContainer3.SplitterDistance = splitContainer2.SplitterDistance;
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            splitContainer3.SplitterDistance = splitContainer2.SplitterDistance;
        }

        private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {
            splitContainer2.SplitterDistance = splitContainer3.SplitterDistance;
        }

        private void Center_Click(object sender, EventArgs e)
        {
            CenterCanyon();
        }

        /*
        private void canyonConn_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.DarkGray, e.ClipRectangle);

            Point Offset;
            int Size;

            if (canyonConn.Width > canyonConn.Height)
            {
                Size = canyonConn.Height;
                Offset = new Point((canyonConn.Width - canyonConn.Height) >> 1, 0);
            }
            else
            {
                Size = canyonConn.Width;
                Offset = new Point(0, (canyonConn.Height - canyonConn.Width) >> 1);
            }
            g.FillRectangle(Brushes.Black, Offset.X, Offset.Y, Size, Size);

            int Modifier = Size >> 1;
            Point Center = new Point(Offset.X + Modifier, Offset.Y + Modifier);

            Vec2[] Walls = Level.Blocks[(int)ActiveBlock.Value - 1].Walls;
            Point[] p = new Point[Walls.Length];
            for (int i = 0; i < Walls.Length; i++)
            {
                p[i].X = Center.X + (int)(Walls[i].X * Modifier);
                p[i].Y = Center.Y - (int)(Walls[i].Y * Modifier);
            }
            g.DrawLines(Pens.Green, p);
            for (int i = 1; i < Walls.Length; i++)
            {
                g.FillRectangle(Brushes.Green, new Rectangle(p[i].X - 2, p[i].Y - 2, 5, 5));
            }
            g.FillRectangle(Brushes.YellowGreen, new Rectangle(p[0].X - 2, p[0].Y - 2, 5, 5));

            Font f = new Font(Font.FontFamily, 8, FontStyle.Bold);
            g.DrawString("Canyon Form", f, Brushes.White, 3, 3);
        }

        private void canyonConn_Resize(object sender, EventArgs e)
        {
            canyonConn.Invalidate();
        }
        */

        private void ObjectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ObjectBox.SelectedIndex == 0)
            {
                propertyGrid.SelectedObject = Level.Blocks[(int)ActiveBlock.Value - 1];
                return;
            }
            propertyGrid.SelectedObject = Level.Blocks[(int)ActiveBlock.Value - 1].Objects[ObjectBox.SelectedIndex-1];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buttonObjAdd.ContextMenuStrip.Show(buttonObjAdd,5,5);
        }

        private void objectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorDescription[] oldo = Level.Blocks[(int)ActiveBlock.Value - 1].Objects;
            EditorDescription[] newo;
            if (oldo != null)
            {
                newo = new EditorDescription[oldo.Length + 1];
                for (int i = 0; i < oldo.Length; i++)
                {
                    newo[i] = oldo[i];
                }
            }
            else
            {
                newo = new EditorDescription[1];
            }

            bool ready = false;

            if (sender.ToString() == "Enemy")
            {
                newo[newo.Length - 1] = new EnemyDescription();
                ready = true;
            }

            if (!ready) throw new Exception("Objekt unbekannt");
                
            Level.Blocks[(int)ActiveBlock.Value - 1].Objects = newo;
            InvalidateCanyonShape();
        }

        private void buttonObjRemove_Click(object sender, EventArgs e)
        {
            EditorDescription[] oldo = Level.Blocks[(int)ActiveBlock.Value - 1].Objects;
            EditorDescription[] newo;
            if (oldo == null) return;
            int sel = ObjectBox.SelectedIndex-1;
            if (sel < 0 || sel >= oldo.Length)
            {
                MessageBox.Show("Bitte zuerst ein Objekt auswählen");
                return;
            }

            if (oldo[sel] == propertyGrid.SelectedObject) propertyGrid.SelectedObject = null;

            newo = new EditorDescription[oldo.Length - 1];
            for (int i = 0; i < newo.Length; i++)
            {
                if (i >= sel)
                    newo[i] = oldo[i+1];
                else
                    newo[i] = oldo[i];
            }
            Level.Blocks[(int)ActiveBlock.Value - 1].Objects = newo;
            InvalidateCanyonShape();
 
        }

        public CanyonGenerator generator = null;
        private void button1_Click_1(object sender, EventArgs e)
        {
            generator = new CanyonGenerator(this, (int)ActiveBlock.Value);
            generator.Show(this);
        }

        private void Client_Activated(object sender, EventArgs e)
        {

        }
    }
}