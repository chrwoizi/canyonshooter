namespace CanyonShooter.Editor
{
    partial class CanyonOutline
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelD1 = new CanyonShooter.Editor.PanelD(this.components);
            this.SuspendLayout();
            // 
            // panelD1
            // 
            this.panelD1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelD1.Location = new System.Drawing.Point(0, 0);
            this.panelD1.Name = "panelD1";
            this.panelD1.Size = new System.Drawing.Size(410, 255);
            this.panelD1.TabIndex = 0;
            this.panelD1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelD1_Paint);
            this.panelD1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelD1_MouseMove);
            this.panelD1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelD1_MouseDown);
            this.panelD1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelD1_MouseUp);
            // 
            // CanyonOutline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelD1);
            this.Name = "CanyonOutline";
            this.Size = new System.Drawing.Size(410, 255);
            this.ResumeLayout(false);

        }

        #endregion

        private PanelD panelD1;
    }
}
