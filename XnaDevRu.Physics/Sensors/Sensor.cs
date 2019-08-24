using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A device that records data from a simulation.  Sensors can either be 
    /// attached to Solids or just positioned somewhere within an 
    /// environment.  Each Sensor maintains a transform matrix; depending 
    /// on whether the Sensor is attached to a Solid, the transform is 
    /// relative to the attached Solid or the global origin.  Most Sensors 
    /// remain ineffective until they are initialized.
    /// </summary>
    public abstract class Sensor
    {

        /// <summary>
        /// Pointer to user data.  This is totally user-managed (i.e. OPAL 
        /// will never delete it).  
        /// </summary>
        protected object userData;

        /// True if the Sensor has been initialized.
        protected bool initCalled;

        public Sensor()
        {
            initCalled = false;
        }

        /// <summary>
        /// Called by subclasses when they are initialized. 
        /// </summary>
        protected void Init()
        {
            initCalled = true;
        }

        /// <summary>
   		/// Sets whether the Sensor can update its measurements.
        /// </summary>
		public abstract bool Enabled { get; set; }

        /// <summary>
		/// Set the user data pointer to some external data.  The user data 
		/// is totally user-managed
        /// </summary>
        public virtual object UserData
        {
            get {return userData;}
            set {userData = value;}
        }

        /// <summary>
		/// Returns the Sensor type.  
        /// </summary>
		public abstract SensorType Type { get; }

        /// <summary>
		/// Gets/Sets the Sensor's transform.
        /// </summary>
		public abstract Matrix Transform { get; set; }

        /// <summary>
		/// Gets/Sets the Sensor's name.
        /// </summary>
		public abstract string Name { get; set; }


        /// <summary>
        /// Called regularly to update the Sensor.  This does nothing if the 
        /// Sensor is disabled.
        /// </summary>
        protected internal abstract void InternalUpdate();

        /// <summary>
        /// Returns true if this Sensor depends on the given Solid.  
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected internal virtual bool InternalDependsOnSolid(Solid s)
        {
            return false;
        }

        public override string ToString()
        {
            string disable = Enabled ? " Enabled" : " Disabled";
            return "Sensor: " + Name + " " + this.Type + disable;
        }

    }

    /// <summary>
   	/// The types of Sensors currently available.
    /// </summary>
	public enum SensorType
	{
		Acceleration,
		Incline,
		Raycast,
		Volume
	};
}
