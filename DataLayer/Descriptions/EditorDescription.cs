using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Xml.Serialization;

namespace CanyonShooter.DataLayer.Descriptions
{
    [Serializable]
    [XmlInclude(typeof(StaticObjectDescription))]
    [XmlInclude(typeof(EnemyDescription))]
    [XmlInclude(typeof(SunlightObjectDescription))]
    [XmlInclude(typeof(WaypointObjectDescription))]
    public class EditorDescription : ICloneable
    {
        private string objectName = "BaseObject";
        public string ObjectName
        {
            get { return objectName; }
            set { objectName = value; }
        }

        [Browsable(false)]
        public string CreateType = string.Empty;

        [Browsable(false),NonSerialized]
        public Vector3 RelativeSpawnLocation;

        [Browsable(false), NonSerialized]
        public int SegmentId;

        private Vector3 relativeSpawnPosition;
        [Description("Spawn Position relativ zum Ursprung dieses Blocks")]
        public Vector3 RelativeSpawnPosition
        {
            get { return relativeSpawnPosition; }
            set { relativeSpawnPosition = value; }
        }

        private int chanceToGenerate = 70;

        [Category("Generation"), Description("The chance, that this object will be generated")]
        public int ChanceToGenerate
        {
            get { return chanceToGenerate; }
            set { chanceToGenerate = value; }
        }


        private int maximumAmount = 1000;

        [Category("Generation"), Description("The maximum amount of objects to generate in a Generation-Process")]
        public int MaximumAmount
        {
            get { return maximumAmount; }
            set { maximumAmount = value; }
        }


        private int minSegmentDistance = 3;

        [Category("Generation"), Description("The minimum CanyonSegment-Distance to place this Object again.")]
        public int MinDistance
        {
            get { return minSegmentDistance; }
            set { minSegmentDistance = value; }
        }


        #region ICloneable Member

        public object Clone()
        {
            object newObject = Activator.CreateInstance(this.GetType());
            FieldInfo[] newFields = newObject.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo[] oldFields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < oldFields.Length; ++i)
            {
                newFields[i].SetValue(newObject, oldFields[i].GetValue(this));
            }

            return newObject;
        }


        public override string ToString()
        {
            return objectName;
        }

        #endregion
    }
}
