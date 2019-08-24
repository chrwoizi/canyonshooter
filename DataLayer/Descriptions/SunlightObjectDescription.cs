using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.DataLayer.Descriptions
{
    /// <summary>
    /// Individual enemy details like speed, strength, healthpoints... 
    /// </summary>
    [Serializable]
    public class SunlightObjectDescription : EditorDescription
    {
        private static int count = 0;
        public SunlightObjectDescription()
        {
            CreateType = "SunlightObject";
            count++;
            ObjectName = "SunlightObject" + count;
            sunlightColor = Microsoft.Xna.Framework.Graphics.Color.White;
        }

        private Color sunlightColor;
        
        [Category("Content"), Description("The color of the sunlight ingame.")]
        public Color Color
          {
            get { return sunlightColor; }
            set { sunlightColor = value; }
          }


    }
}
