using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CanyonShooter.DataLayer.Descriptions
{
    /// <summary>
    /// Individual enemy details like speed, strength, healthpoints... 
    /// </summary>
    [Serializable]
    public class StaticObjectDescription : EditorDescription
    {
        private static int count = 0;
        public StaticObjectDescription()
        {
            CreateType = "StaticObject";
            count++;
            ObjectName = "StaticObject" + count;
        }
        private string name = "Bridge";
        [Category("Content"), Description("The Assetname of the StaticObject.")]
        public string Name
          {
            get { return name; }
            set { name = value; }
          }


    }
}
