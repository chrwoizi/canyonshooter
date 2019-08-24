using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CanyonShooter.DataLayer.Level;
using CanyonShooter.DataLayer.Descriptions;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CanyonShooter.Editor
{
    public partial class CanyonGenerator : Form
    {
        public int segmentNumber;
        private Client currentClient;

        private int points;
        private int numberOfSegments;
        private int[] prevSeg;
        private float curviness;
        private float difHeight;
        private float segLengthFrom;
        private float segLengthTo;
        private int pointTolerance;
        private int enemyChance;
        private int enemyNumber;
        Random rnd;


        public CanyonGenerator(Client cl, int segmentnumber)
        {
            InitializeComponent();
            currentClient = cl;
            this.segmentNumber = segmentnumber;
            rnd = new Random((int)DateTime.Now.Ticks);


            //** Init EditorDescriptions (Objects)

            listBoxObjects.Items.Add(typeof(EnemyDescription));
            listBoxObjects.Items.Add(typeof(StaticObjectDescription));
            listBoxObjects.Items.Add(typeof (SunlightObjectDescription));
            listBoxObjects.Items.Add(typeof(WaypointObjectDescription));

            //**

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private LevelBlock getReference(int number)
        {
            LevelBlock nr1 = new LevelBlock();
            nr1.Walls = new Vec2[10];
            nr1.Walls[0].X = -1;
            nr1.Walls[0].Y = 1;

            nr1.Walls[1].X = -0.8f;
            nr1.Walls[1].Y = 1;

            nr1.Walls[2].X = -0.9f;
            nr1.Walls[2].Y = 0.3f;

            nr1.Walls[3].X = -0.6f;
            nr1.Walls[3].Y = -0.5f;

            nr1.Walls[4].X = -0.2f;
            nr1.Walls[4].Y = -0.6f;

            nr1.Walls[5].X = 0.2f;
            nr1.Walls[5].Y = -0.6f;

            nr1.Walls[6].X = 0.6f;
            nr1.Walls[6].Y = -0.5f;

            nr1.Walls[7].X = 0.9f;
            nr1.Walls[7].Y = 0.3f;

            nr1.Walls[8].X = 0.8f;
            nr1.Walls[8].Y = 1;

            nr1.Walls[9].X = 1;
            nr1.Walls[9].Y = 1;

            return nr1;
        }

        private LevelBlock copyBlockWithTolerance(LevelBlock current, int tolerance)
        {
            LevelBlock newblock = new LevelBlock();
            newblock.Walls = new Vec2[current.Walls.Length];
            for (int i = 0; i < current.Walls.Length; i++)
            {
                newblock.Walls[i].X = current.Walls[i].X + (float)rnd.Next(-tolerance, tolerance) / 100;
                newblock.Walls[i].Y = current.Walls[i].Y + (float)rnd.Next(-tolerance, tolerance) / 100;
            }
            return newblock;
        }

        private void GenerateCanyon()
        {
            
            if(checkOnlyObjects.Checked) // generate only objects
            {
                ResetChecks();
                for (int i = 0; i < currentClient.Level.Blocks.Length; i++)
                {
                    GenerateObjects(out currentClient.Level.Blocks[i]);
                }
                return;
            }

            int levelLength = segmentNumber - 1 + this.numberOfSegments;

            LevelBlock[] referenceBlocks = new LevelBlock[prevSeg.Length];
            for (int i = 0; i < prevSeg.Length; i++)
            {
                referenceBlocks[i] = currentClient.Level.Blocks[prevSeg[i]];
            }
            Level tmp = new Level(segmentNumber - 1 + this.numberOfSegments);
            for (int i = 0; i < segmentNumber - 1; i++)
            {
                tmp.Blocks[i] = currentClient.Level.Blocks[i];
            }


            if (prevSeg.Length <= 0)
            {
                referenceBlocks = new LevelBlock[1];
                referenceBlocks[0] = this.getReference(0);
            }

            LevelBlock currentSegment =
                this.copyBlockWithTolerance(referenceBlocks[rnd.Next(0, referenceBlocks.Length)], pointTolerance);

            currentSegment.DirectionX = 0;
            currentSegment.DirectionY = 0;
            currentSegment.DirectionZ = rnd.Next((int)(-segLengthTo), (int)(-segLengthFrom));

            tmp.Blocks[segmentNumber - 1] = currentSegment;
          
            ResetChecks(); // Reset Canyon Objects Checks

            for (int i = segmentNumber; i < levelLength; i += 1)
            {
                currentSegment = this.copyBlockWithTolerance(referenceBlocks[rnd.Next(0, referenceBlocks.Length)], pointTolerance);
                int newY = rnd.Next((int)(-difHeight), (int)(difHeight));
                int newZ = rnd.Next((int)(-segLengthTo), (int)(-segLengthFrom));
                int newX = rnd.Next((int)(-curviness), (int)(curviness));
                if (newX < 0)
                {
                    newZ -= newX;
                }
                else
                {
                    newZ += newX;
                }
                currentSegment.DirectionX = newX;
                currentSegment.DirectionY = newY;
                currentSegment.DirectionZ = newZ;

                GenerateObjects(out currentSegment);

                tmp.Blocks[i] = currentSegment;
            }
            currentClient.Level = tmp;
            
        }


        private void GenerateObjects(out LevelBlock currentSegment)
        {
            // Create a temporary list to hold objects.
            List<EditorDescription> descList = new List<EditorDescription>();
            foreach (EditorDescription desc in listBoxObjectsToCreate.Items)
            {
                if (rnd.Next(1, 101) < desc.ChanceToGenerate && IsDistanceOk(desc))
                {
                    if (IsObjectCountOk(desc))
                    {
                        descList.Add(desc);
                        ResetDistanceCheck(desc);
                    }
                }
            }
            currentSegment.Objects = new EditorDescription[descList.Count];

            // Add Objects to Canyon
            for (int a = 0; a < descList.Count; a++)
            {
                currentSegment.Objects[a] = descList[a];
            }
        }


        private void ResetChecks()
        {
            objectCount.Clear();
            objectDistance.Clear();
        }

        private Dictionary<string, int> objectCount = new Dictionary<string, int>();
        private Dictionary<string, int> objectDistance = new Dictionary<string, int>();


        private bool IsObjectCountOk(EditorDescription desc)
        {
            if(!objectCount.ContainsKey(desc.ObjectName))
            {
                objectCount.Add(desc.ObjectName,1);
                return true;
            }

            if(objectCount[desc.ObjectName] < desc.MaximumAmount)
            {
                objectCount[desc.ObjectName]++;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Determines whether [is distance ok] [the specified desc].
        /// Distance in canyon segments, to place this item again
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <returns>
        /// 	<c>true</c> if [is distance ok] [the specified desc]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDistanceOk(EditorDescription desc)
        {
            if (!objectDistance.ContainsKey(desc.ObjectName))
            {
                objectDistance.Add(desc.ObjectName, desc.MinDistance);
                return true;
            }
            else if (objectDistance[desc.ObjectName] <= 0)
                
                return true;
            else
                objectDistance[desc.ObjectName]--;
            return false;

        }

        /// <summary>
        /// Resets the distance check.
        /// </summary>
        /// <param name="desc">The desc.</param>
        private void ResetDistanceCheck(EditorDescription desc)
        {
            if (objectDistance.ContainsKey(desc.ObjectName))
                objectDistance[desc.ObjectName] = desc.MinDistance;          
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CanyonGenerator_Load(object sender, EventArgs e)
        {
            ReloadPredefinedObjectList();
            LoadQuickItems();
            LoadEnemyTypes();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void Generate_Click(object sender, EventArgs e)
        {
            numberOfSegments = Convert.ToInt32(textBoxNumberofSegements.Text);
            pointTolerance = Convert.ToInt32(textBoxTolerance.Text);
            String[] prevSegString;
            prevSegString = textBoxReferences.Text.Split(';');
            prevSeg = new int[prevSegString.Length - 1];
            for (int i = 0; i < prevSegString.Length - 1; i++)
            {
                prevSeg[i] = Convert.ToInt32(prevSegString[i]);
                if (prevSeg[i] >= currentClient.Level.Blocks.Length)
                {
                    labelError.Text = "Error: Es dürfen keine nicht verfügbaren ReferenceElemente gewählt werden.";
                    return;
                }
            }

            curviness = Convert.ToInt32(textBoxCurviness.Text);
            difHeight = Convert.ToInt32(textBoxDifHeight.Text);
            segLengthFrom = Convert.ToInt32(textBoxSegmentLengthFrom.Text);
            segLengthTo = Convert.ToInt32(textBoxSegmentLengthTo.Text);

            if ((segmentNumber <= 1) && (numberOfSegments <= 1))
            {
                labelError.Text = "Error: Es darf nicht Das erste und nur 1 Segement generiert werden.";
                return;
            }
            GenerateCanyon();
            currentClient.RecalcCanyonShape();
            Close();
        }

        private void listBoxObjects_DoubleClick(object sender, EventArgs e)
        {
            listBoxObjectsToCreate.Items.Add(Activator.CreateInstance((listBoxObjects.SelectedItem as Type)));
        }

        private void listBoxObjectsToCreate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                if (listBoxObjectsToCreate.SelectedItem != null)
                {
                    listBoxObjectsToCreate.Items.Remove(listBoxObjectsToCreate.SelectedItem);
                    ObjectProperties.SelectedObject = null;
                }
        }

        private void listBoxObjectsToCreate_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBoxObjectsToCreate.SelectedItem != null)
                ObjectProperties.SelectedObject = listBoxObjectsToCreate.SelectedItem;
        }

        private void ObjectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "ObjectName")
            {
                listBoxObjectsToCreate.SelectedItem = ObjectProperties.SelectedObject;
                int index = listBoxObjectsToCreate.SelectedIndex;
                listBoxObjectsToCreate.Items.Remove(listBoxObjectsToCreate.SelectedItem);
                listBoxObjectsToCreate.Items.Insert(index, ObjectProperties.SelectedObject);
            }
        }

        private Stream s;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // create Predefined Object Dir, if it does not exists
                string objDir = string.Format(@"{0}\PredefinedObjects", Editor.EditorPath);
                if (!Directory.Exists(objDir))
                    Directory.CreateDirectory(objDir);

                // if object exists, try to save it as an xml predefined object

                if (listBoxObjectsToCreate.SelectedItem != null)
                {
                    string name = ((EditorDescription)listBoxObjectsToCreate.SelectedItem).ObjectName;
                    EditorDescription desc = (EditorDescription)listBoxObjectsToCreate.SelectedItem;
                    XmlSerializer x = new XmlSerializer(typeof(EditorDescription));

                    s = File.Create(string.Format(@"{0}\PredefinedObjects\{1}.xml", Editor.EditorPath, name));
                    x.Serialize(s, desc as EditorDescription);
                    s.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while saving Predefined Object:");
            }
            finally
            {
                s.Close();
                
            }
            ReloadPredefinedObjectList();
        }

        /// <summary>
        /// Reloads the predefined object list.
        /// </summary>
        private void ReloadPredefinedObjectList()
        {
            listPredefinedObjects.Items.Clear();
            
            DirectoryInfo dir = new DirectoryInfo(string.Format(@"{0}\PredefinedObjects\", Editor.EditorPath));
            foreach (FileInfo file in dir.GetFiles("*.xml"))
            {
                LoadPredefinedObject(file.FullName);
            }
        }

        private void LoadPredefinedObject(string file)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(EditorDescription));
                s = File.OpenRead(file);
                listPredefinedObjects.Items.Add(x.Deserialize(s) as EditorDescription);
                s.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while loading Predefined Object:");
            }
            finally
            {
                s.Close();

            }

        }

        private void listPredefinedObjects_DoubleClick(object sender, EventArgs e)
        {
            if (listPredefinedObjects.SelectedItem != null)
            {
                EditorDescription desc = DeepClone(listPredefinedObjects.SelectedItem) as EditorDescription;
                if (desc != null)
                {
                    listBoxObjectsToCreate.Items.Add(desc);
                }
            }
        }

        /// <summary>
        /// Deeps the clone. Muhahahahaa
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static object DeepClone(object source)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(m, source);
            m.Position = 0;
            return b.Deserialize(m);

        }

        private void LoadQuickItems()
        {
            listQuickItems.Items.Clear();

            DirectoryInfo dir = new DirectoryInfo(string.Format(@"{0}\", Editor.ItemPath));
            foreach (FileInfo file in dir.GetFiles("*.xml"))
            {
                listQuickItems.Items.Add(file.Name.Substring(0, file.Name.Length - 4));
            }
        }

        private void listQuickItems_DoubleClick(object sender, EventArgs e)
        {
            if(listQuickItems.SelectedItem != null)
                if(listBoxObjectsToCreate.SelectedItem != null)
                    if(listBoxObjectsToCreate.SelectedItem is EnemyDescription)
                        ((EnemyDescription)listBoxObjectsToCreate.SelectedItem).ItemList.Add(listQuickItems.SelectedItem.ToString());
        }

        private void LoadEnemyTypes()
        {
            listEnemies.Items.Clear();

            DirectoryInfo dir = new DirectoryInfo(string.Format(@"{0}\", Editor.EnemyPath));
            foreach (FileInfo file in dir.GetFiles("*.xml"))
            {
                listEnemies.Items.Add(file.Name.Substring(0, file.Name.Length - 4));
            }
        }

        private void listEnemies_DoubleClick(object sender, EventArgs e)
        {
            if (listEnemies.SelectedItem != null)
                if (listBoxObjectsToCreate.SelectedItem != null)
                    if (listBoxObjectsToCreate.SelectedItem is EnemyDescription)
                        ((EnemyDescription)listBoxObjectsToCreate.SelectedItem).Type = listEnemies.SelectedItem.ToString();
            ObjectProperties.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ReloadPredefinedObjectList();
            LoadQuickItems();
            LoadEnemyTypes();
        }

    }
}