namespace CanyonShooter.Editor
{
    partial class Client
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.ActiveBlock = new System.Windows.Forms.NumericUpDown();
            this.AddSegment = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.canyonShape1 = new CanyonShooter.Editor.CanyonShape();
            this.canyonOutlineZY = new CanyonShooter.Editor.CanyonOutline();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.canyonOutlineXZ = new CanyonShooter.Editor.CanyonOutline();
            this.canyonOutlineXY = new CanyonShooter.Editor.CanyonOutline();
            this.ZoomPlus = new System.Windows.Forms.Button();
            this.ZoomMinus = new System.Windows.Forms.Button();
            this.Center = new System.Windows.Forms.Button();
            this.ObjectBox = new System.Windows.Forms.ListBox();
            this.buttonObjAdd = new System.Windows.Forms.Button();
            this.AddObjectMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.enemyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonObjRemove = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gen = new System.Windows.Forms.Button();
            this.canyonTracker = new CanyonShooter.Editor.CanyonTracker();
            ((System.ComponentModel.ISupportInitialize)(this.ActiveBlock)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.AddObjectMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(604, 254);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(210, 295);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // ActiveBlock
            // 
            this.ActiveBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ActiveBlock.Location = new System.Drawing.Point(604, 72);
            this.ActiveBlock.Maximum = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.ActiveBlock.Name = "ActiveBlock";
            this.ActiveBlock.Size = new System.Drawing.Size(98, 20);
            this.ActiveBlock.TabIndex = 1;
            this.ActiveBlock.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ActiveBlock.ValueChanged += new System.EventHandler(this.ActiveBlock_ValueChanged);
            // 
            // AddSegment
            // 
            this.AddSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddSegment.Location = new System.Drawing.Point(604, 43);
            this.AddSegment.Name = "AddSegment";
            this.AddSegment.Size = new System.Drawing.Size(126, 23);
            this.AddSegment.TabIndex = 2;
            this.AddSegment.Text = "Add Segment";
            this.AddSegment.UseVisualStyleBackColor = true;
            this.AddSegment.Click += new System.EventHandler(this.AddSegment_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(11, 72);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(587, 477);
            this.splitContainer1.SplitterDistance = 288;
            this.splitContainer1.TabIndex = 4;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.canyonShape1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.canyonOutlineZY);
            this.splitContainer3.Size = new System.Drawing.Size(288, 477);
            this.splitContainer3.SplitterDistance = 258;
            this.splitContainer3.TabIndex = 0;
            this.splitContainer3.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainer3_SplitterMoving);
            this.splitContainer3.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer3_SplitterMoved);
            // 
            // canyonShape1
            // 
            this.canyonShape1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canyonShape1.Level = null;
            this.canyonShape1.Location = new System.Drawing.Point(0, 0);
            this.canyonShape1.Name = "canyonShape1";
            this.canyonShape1.SelectedOutlineIndex = 0;
            this.canyonShape1.SelectedShapeNode = 0;
            this.canyonShape1.Size = new System.Drawing.Size(288, 258);
            this.canyonShape1.TabIndex = 0;
            // 
            // canyonOutlineZY
            // 
            this.canyonOutlineZY.Caption = null;
            this.canyonOutlineZY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canyonOutlineZY.Level = null;
            this.canyonOutlineZY.Location = new System.Drawing.Point(0, 0);
            this.canyonOutlineZY.Name = "canyonOutlineZY";
            this.canyonOutlineZY.Offset = new System.Drawing.Point(205, 127);
            this.canyonOutlineZY.OutlineType = CanyonShooter.Editor.CanyonOutlineType.XY;
            this.canyonOutlineZY.SelectedIndex = 0;
            this.canyonOutlineZY.Size = new System.Drawing.Size(288, 215);
            this.canyonOutlineZY.TabIndex = 0;
            this.canyonOutlineZY.Zoom = 256;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.canyonOutlineXZ);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.canyonOutlineXY);
            this.splitContainer2.Size = new System.Drawing.Size(295, 477);
            this.splitContainer2.SplitterDistance = 258;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainer2_SplitterMoving);
            this.splitContainer2.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer2_SplitterMoved);
            // 
            // canyonOutlineXZ
            // 
            this.canyonOutlineXZ.Caption = null;
            this.canyonOutlineXZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canyonOutlineXZ.Level = null;
            this.canyonOutlineXZ.Location = new System.Drawing.Point(0, 0);
            this.canyonOutlineXZ.Name = "canyonOutlineXZ";
            this.canyonOutlineXZ.Offset = new System.Drawing.Point(205, 127);
            this.canyonOutlineXZ.OutlineType = CanyonShooter.Editor.CanyonOutlineType.XY;
            this.canyonOutlineXZ.SelectedIndex = 0;
            this.canyonOutlineXZ.Size = new System.Drawing.Size(295, 258);
            this.canyonOutlineXZ.TabIndex = 0;
            this.canyonOutlineXZ.Zoom = 256;
            // 
            // canyonOutlineXY
            // 
            this.canyonOutlineXY.Caption = null;
            this.canyonOutlineXY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canyonOutlineXY.Level = null;
            this.canyonOutlineXY.Location = new System.Drawing.Point(0, 0);
            this.canyonOutlineXY.Name = "canyonOutlineXY";
            this.canyonOutlineXY.Offset = new System.Drawing.Point(205, 127);
            this.canyonOutlineXY.OutlineType = CanyonShooter.Editor.CanyonOutlineType.XY;
            this.canyonOutlineXY.SelectedIndex = 0;
            this.canyonOutlineXY.Size = new System.Drawing.Size(295, 215);
            this.canyonOutlineXY.TabIndex = 0;
            this.canyonOutlineXY.Zoom = 256;
            // 
            // ZoomPlus
            // 
            this.ZoomPlus.Location = new System.Drawing.Point(11, 43);
            this.ZoomPlus.Name = "ZoomPlus";
            this.ZoomPlus.Size = new System.Drawing.Size(23, 23);
            this.ZoomPlus.TabIndex = 5;
            this.ZoomPlus.Text = "+";
            this.ZoomPlus.UseVisualStyleBackColor = true;
            this.ZoomPlus.Click += new System.EventHandler(this.ZoomPlus_Click);
            // 
            // ZoomMinus
            // 
            this.ZoomMinus.Location = new System.Drawing.Point(40, 43);
            this.ZoomMinus.Name = "ZoomMinus";
            this.ZoomMinus.Size = new System.Drawing.Size(23, 23);
            this.ZoomMinus.TabIndex = 6;
            this.ZoomMinus.Text = "-";
            this.ZoomMinus.UseVisualStyleBackColor = true;
            this.ZoomMinus.Click += new System.EventHandler(this.ZoomMinus_Click);
            // 
            // Center
            // 
            this.Center.Location = new System.Drawing.Point(69, 43);
            this.Center.Name = "Center";
            this.Center.Size = new System.Drawing.Size(23, 23);
            this.Center.TabIndex = 7;
            this.Center.Text = "C";
            this.Center.UseVisualStyleBackColor = true;
            this.Center.Click += new System.EventHandler(this.Center_Click);
            // 
            // ObjectBox
            // 
            this.ObjectBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectBox.FormattingEnabled = true;
            this.ObjectBox.Location = new System.Drawing.Point(604, 101);
            this.ObjectBox.Name = "ObjectBox";
            this.ObjectBox.Size = new System.Drawing.Size(210, 147);
            this.ObjectBox.TabIndex = 9;
            this.ObjectBox.SelectedIndexChanged += new System.EventHandler(this.ObjectBox_SelectedIndexChanged);
            // 
            // buttonObjAdd
            // 
            this.buttonObjAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonObjAdd.ContextMenuStrip = this.AddObjectMenu;
            this.buttonObjAdd.Location = new System.Drawing.Point(708, 72);
            this.buttonObjAdd.Name = "buttonObjAdd";
            this.buttonObjAdd.Size = new System.Drawing.Size(50, 23);
            this.buttonObjAdd.TabIndex = 10;
            this.buttonObjAdd.Text = "Obj +";
            this.buttonObjAdd.UseVisualStyleBackColor = true;
            this.buttonObjAdd.Click += new System.EventHandler(this.button1_Click);
            // 
            // AddObjectMenu
            // 
            this.AddObjectMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enemyToolStripMenuItem});
            this.AddObjectMenu.Name = "contextMenuStrip1";
            this.AddObjectMenu.Size = new System.Drawing.Size(111, 26);
            // 
            // enemyToolStripMenuItem
            // 
            this.enemyToolStripMenuItem.Name = "enemyToolStripMenuItem";
            this.enemyToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.enemyToolStripMenuItem.Text = "Enemy";
            this.enemyToolStripMenuItem.Click += new System.EventHandler(this.objectToolStripMenuItem_Click);
            // 
            // buttonObjRemove
            // 
            this.buttonObjRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonObjRemove.Location = new System.Drawing.Point(764, 72);
            this.buttonObjRemove.Name = "buttonObjRemove";
            this.buttonObjRemove.Size = new System.Drawing.Size(50, 23);
            this.buttonObjRemove.TabIndex = 11;
            this.buttonObjRemove.Text = "Obj -";
            this.buttonObjRemove.UseVisualStyleBackColor = true;
            this.buttonObjRemove.Click += new System.EventHandler(this.buttonObjRemove_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(142, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(399, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Canyon bewegen jetzt mit rechter Maustaste! Elemente ziehen mit linker Maustaste!" +
                "";
            // 
            // gen
            // 
            this.gen.Location = new System.Drawing.Point(98, 43);
            this.gen.Name = "gen";
            this.gen.Size = new System.Drawing.Size(38, 23);
            this.gen.TabIndex = 13;
            this.gen.Text = "Gen";
            this.gen.UseVisualStyleBackColor = true;
            this.gen.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // canyonTracker
            // 
            this.canyonTracker.ActiveBlock = 0;
            this.canyonTracker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.canyonTracker.Location = new System.Drawing.Point(11, 13);
            this.canyonTracker.Name = "canyonTracker";
            this.canyonTracker.Size = new System.Drawing.Size(803, 24);
            this.canyonTracker.TabIndex = 8;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 561);
            this.Controls.Add(this.gen);
            this.Controls.Add(this.buttonObjRemove);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonObjAdd);
            this.Controls.Add(this.ObjectBox);
            this.Controls.Add(this.canyonTracker);
            this.Controls.Add(this.Center);
            this.Controls.Add(this.ZoomMinus);
            this.Controls.Add(this.ZoomPlus);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.AddSegment);
            this.Controls.Add(this.ActiveBlock);
            this.Controls.Add(this.propertyGrid);
            this.Name = "Client";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Client_Load);
            this.Activated += new System.EventHandler(this.Client_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.ActiveBlock)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.AddObjectMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button AddSegment;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ZoomPlus;
        private System.Windows.Forms.Button ZoomMinus;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button Center;
        private CanyonTracker canyonTracker;
        public System.Windows.Forms.NumericUpDown ActiveBlock;
        private System.Windows.Forms.ListBox ObjectBox;
        private System.Windows.Forms.Button buttonObjAdd;
        private System.Windows.Forms.Button buttonObjRemove;
        private System.Windows.Forms.ContextMenuStrip AddObjectMenu;
        private System.Windows.Forms.ToolStripMenuItem enemyToolStripMenuItem;
        private CanyonOutline canyonOutlineXZ;
        private CanyonOutline canyonOutlineZY;
        private CanyonOutline canyonOutlineXY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button gen;
        private CanyonShape canyonShape1;
    }
}