using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a RaycastSensor.
    /// </summary>
    public class RaycastSensorData : SensorData
    {
        /// <summary>
        /// The ray used by this Sensor for ray casting.  The length of the 
        /// ray is important; objects beyond the length of the ray will not 
        /// be intersected.  Keep in mind that the Sensor's transform may 
        /// affect the ray's final origin and direction.  
        /// </summary>
        public Ray Ray;

        
        private int contactGroup;

        /// <summary>
        /// The ray's contact group.  This can be used to limit which objects 
        /// the ray collides with.  If this exceeds 31, bad things 
        /// might happen since there are only 32 groups.
        /// </summary>
        public int ContactGroup {
            get { return contactGroup; }
            set { contactGroup = value; }
        }

        public RaycastSensorData()
        {
            type = SensorType.Raycast;
            // "ray" is initialized in its own constructor.
            ContactGroup = Defaults.Shape.ContactGroup;
        }
    }
}
