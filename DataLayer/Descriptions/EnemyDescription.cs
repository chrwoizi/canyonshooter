using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;

namespace CanyonShooter.DataLayer.Descriptions
{
    /// <summary>
    /// Individual enemy details like speed, strength, healthpoints... 
    /// </summary>
    [Serializable]
    public class EnemyDescription : EditorDescription
    {
        private static int count = 0;
        public EnemyDescription()
        {
            CreateType = "Enemies.EnemyAI2";
            count++;
            ObjectName = "Enemy" + count;
        }
        private string model = string.Empty;
        private int maxHitpoints = 100;
        private float speed = 250f;
        private List<Vector3> path;
        private List<string> itemList = new List<string>();
        private int itemChance = 80;

        private string type;

        private int squadronCount = 1;

        private string enemyAI = "EnemyAI2";
        public string EnemyAI
        {
            get { return enemyAI; }
            set
            {
                enemyAI = value;
                CreateType = "Enemies." + enemyAI;
            }
        }

        public int SquadronCount
        {
            get { return squadronCount; }
            set { squadronCount = value; }
        }

        [Category("Content"), Description("The Assetname of the Enemy")]
        public string Type
          {
              get { return type; }
              set { type = value; }
          }

        public string Model
          {
            get { return model; }
            set { model = value; }
          }
        public int MaxHitpoints
          {
            get { return maxHitpoints; }
            set { maxHitpoints = value; }
          }
        public float Speed
          {
            get { return speed; }
            set { speed = value; }
          }

        [Editor(@"System.Windows.Forms.Design.StringCollectionEditor,
System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
typeof(System.Drawing.Design.UITypeEditor))] 
        public List<string> ItemList
        {
            get { return itemList; }
            set { itemList = value;}
        }

        public int ItemChance
        {
            get { return itemChance; }
            set { itemChance = value;}
        }

    }
}
