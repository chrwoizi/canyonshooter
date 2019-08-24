using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CanyonShooter.DataLayer.Level;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace CanyonShooter.Editor
{
    public partial class Editor : Form
    {
        private int childFormNumber = 0;

        public Editor()
        {
            InitializeComponent();

            EditorPath = Path.GetDirectoryName(Application.ExecutablePath);
            string path = Path.GetFullPath(EditorPath + @"\..\..\..\CanyonShooter\GameClasses\Levels");

            if(Directory.Exists(path) )
                LevelPath = path;

            path = Path.GetFullPath(EditorPath + @"\..\..\..\CanyonShooter\Content\Items");
            if(Directory.Exists(path))
                ItemPath = path;
            path = Path.GetFullPath(EditorPath + @"\..\..\..\CanyonShooter\Content\Enemies");
            if (Directory.Exists(path))
                EnemyPath = path;

        }
        
        public static string EditorPath = "";
        public static string LevelPath = "";
        public static string ItemPath = "";
        public static string EnemyPath = "";

        private void ShowNewForm(object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            Form childForm = new Client();
            // Make it a child of this MDI form before showing it.
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = LevelPath;

            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Level Files (*.csl)|*.csl|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;

                Client childForm = new Client();
                childForm.MdiParent = this;
                childForm.Text = System.IO.Path.GetFileName(FileName);
                childForm.FileName = FileName;
                childForm.Level = LevelManager.Load(FileName);
                childForm.Show();
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null) return;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = LevelPath;

            //saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Level Files (*.csl)|*.csl|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;

                LevelManager.Save(FileName, ((Client)ActiveMdiChild).Level);
                ((Client)ActiveMdiChild).FileName = FileName;
                ((Client)ActiveMdiChild).Text = System.IO.Path.GetFileName(FileName);
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard.GetText() or System.Windows.Forms.GetData to retrieve information from the clipboard.
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void SaveFile(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null) return;
            if (((Client)ActiveMdiChild).FileName == "")
            {
                SaveAsToolStripMenuItem_Click(sender, e);
                return;
            }
            LevelManager.Save(((Client)ActiveMdiChild).FileName, ((Client)ActiveMdiChild).Level);
        }



        private void Editor_Load(object sender, EventArgs e)
        {

        }

        private void btnTestLevel_Click(object sender, EventArgs e)
        {
                        OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = LevelPath;
            dlg.Filter = "Level Files (*.csl)|*.csl|All Files (*.*)|*.*";
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                Thread csThread = new Thread(new ParameterizedThreadStart(RunCS));
                string name = Path.GetFileName(dlg.FileName);
                csThread.Start(name.Substring(0,name.Length-4)); // without .csl file ending
            }
        }
        public void RunCS(object map)
        {

            List<string> parameters = new List<string>();
            parameters.Add("--screenResolution");
            parameters.Add("800x600");
            parameters.Add("--quickGame");
            parameters.Add(map.ToString());
            using (CanyonShooterGame game = new CanyonShooterGame(parameters.ToArray()))
            {
                game.Run();
            }
            MessageBox.Show("Preview Closed.");
            
        }
    }
}
