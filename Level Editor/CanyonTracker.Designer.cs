namespace CanyonShooter.Editor
{
    partial class CanyonTracker
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CanyonTracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CanyonTracker";
            this.Size = new System.Drawing.Size(287, 30);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CanyonTracker_MouseClick);
            this.Resize += new System.EventHandler(this.CanyonTracker_Resize);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CanyonTracker_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
