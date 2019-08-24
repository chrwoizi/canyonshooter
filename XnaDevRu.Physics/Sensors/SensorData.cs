using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a Sensor.
    /// </summary>
    public class SensorData
    {

        private bool enabled;

        /// <summary>
        /// True if the Sensor is enabled.
        /// </summary>
        public bool Enabled {
            get { return enabled; }
            set { enabled = value; }
        }

        private string name;

        /// <summary>
        /// An identifier for the Sensor.
        /// </summary>
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private Solid solid;

        /// <summary>
        /// Pointer to the Solid to which this Sensor is attached.  This
        /// will be NULL if the Sensor is not attached to a Solid (i.e.
        /// it is just positioned somewhere within the environment).
        /// </summary>
        public Solid Solid {
            get { return solid; }
            set { solid = value; }
        }

        /// <summary>
        /// If the Sensor is attached to a Solid, this transform is the global
        /// offset from that Solid's transform.  Otherwise, it is just the
        /// Sensor's global transform.
        /// </summary>
        public Matrix Transform;

        /// The Sensor type.
        protected SensorType type;

        public SensorData() {
            // The initial type doesn't matter since the abstract base
            // class will never be instantiated.
            type = SensorType.Acceleration;
            Enabled = Defaults.Sensor.Enabled;
            Name = "";
            Solid = null;
            // "transform" is initialized in its own constructor.
            //Transform = Matrix.Identity;
        }


        /// <summary>
        /// Returns the Sensor's type.
        /// </summary>
        public virtual SensorType Type {
            get { return type; }
        }

        public override string ToString() {
            string disable = Enabled ? " Enabled" : " Disabled";
            return "SensorData: " + Name + " " + this.Type + disable;
        }

    }
}
