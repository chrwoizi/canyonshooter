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
    public class WaypointObjectDescription : EditorDescription
    {
        private static int count = 0;
        public WaypointObjectDescription()
        {
            CreateType = "WaypointObject";
            count++;
            ObjectName = "WaypointObject" + count;
        }
        private string model = "Waypoint";

        [Category("Content"), Description("The Model-Assetname of the StaticObject.")]
        public string Model
          {
            get { return model; }
            set { model = value; }
          }

       
        private int timeInSeconds = 30;
        public int TimeInSeconds
        {
            get
            {
                return timeInSeconds;
            }
            set
            {
                timeInSeconds = value;
            }
        }

    }
}
