using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A Sensor that monitors the linear and angular acceleration of a 
    /// Solid.  Using its transform, it can be set to a desired offset from 
    /// the attached Solid.  This Sensor does nothing if it is not attached 
    /// to a Solid (i.e. its returned values are always zero).
    /// </summary>
    public class AccelerationSensor : Sensor
    {
        #region fields
        /// Stores data describing the Sensor.
        protected AccelerationSensorData data;

        /// Pointer to the Simulator containing this Sensor.  This is used 
        /// to get an accurate dt value on every update.
        protected Simulator sim;

        /// Stored copy of the current step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 currentGlobalLinearVel;

        /// Stored copy of the current step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 currentGlobalAngularVel;

        /// Stored copy of the current step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 currentLocalLinearVel;

        /// Stored copy of the current step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 currentLocalAngularVel;

        /// Stored copy of the previous step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 prevGlobalLinearVel;

        /// Stored copy of the previous step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 prevGlobalAngularVel;

        /// Stored copy of the previous step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 prevLocalLinearVel;

        /// Stored copy of the previous step's velocity data; used for 
        /// acceleration calculations.
        protected Vector3 prevLocalAngularVel; 
        #endregion

        public AccelerationSensor(Simulator s)
        {
            sim = s;
        }

        /// <summary>
        /// Initializes the Sensor with the given data structure.  The Solid 
        /// pointer should always be valid because this Sensor only works 
        /// when attached to something.
        /// </summary>
        /// <param name="data"></param>
		public virtual void Init( AccelerationSensorData data)
        {
            base.Init();
            this.data = data;
        }

        /// <summary>
		/// Returns all data describing the Sensor.
        /// </summary>
		public virtual AccelerationSensorData Data
        {
            get { return data; }
        }

		// Returns the Sensor's global linear acceleration.  If the Sensor 
		// is not attached to a Solid, this returns (0, 0, 0).
        public virtual Vector3 GlobalLinearAccel
        {
            get 
            {
                if (!data.Enabled || data.Solid == null)
                {
                    return Vector3.Zero;
                }

                return (currentGlobalLinearVel - prevGlobalLinearVel) / sim.StepSize;
            }
        }

		// Returns the Sensor's global angular acceleration.  If the Sensor 
		// is not attached to a Solid, this returns (0, 0, 0).
        public virtual Vector3 GlobalAngularAccel
        {
            get 
            {
                if (!data.Enabled || data.Solid == null)
                {
                    return Vector3.Zero;
                }

                return (currentGlobalAngularVel - prevGlobalAngularVel) / sim.StepSize;
            }
        }

		// Returns the Sensor's local linear acceleration.  If the Sensor 
		// is not attached to a Solid, this returns (0, 0, 0).
        public virtual Vector3 LocalLinearAccel
        {
            get 
            {
                if (!data.Enabled || data.Solid == null)
                {
                    return Vector3.Zero;
                }

                return (currentLocalLinearVel - prevLocalLinearVel) / sim.StepSize;
            }
        }

		// Returns the Sensor's local angular acceleration.  If the Sensor 
		// is not attached to a Solid, this returns (0, 0, 0).
        public virtual Vector3 LocalAngularAccel
        {
            get 
            {
                if (!data.Enabled || data.Solid == null)
                {
                    return Vector3.Zero;
                }

                return (currentLocalAngularVel - prevLocalAngularVel) / sim.StepSize;
            }
        }

        public override bool Enabled
        {
            get { return data.Enabled; }
            set { data.Enabled = value; }
        }

        public override Matrix Transform
        {
            get { return data.Transform; }
            set { data.Transform = value; }
        }

		public override SensorType Type
        {
            get { return data.Type; }
        }

        public override string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }

		protected internal override void InternalUpdate()
        {
            if (data.Enabled && data.Solid != null)
            {
                prevGlobalLinearVel = currentGlobalLinearVel;
                prevGlobalAngularVel = currentGlobalAngularVel;
                prevLocalLinearVel = currentLocalLinearVel;
                prevLocalAngularVel = currentLocalAngularVel;

                currentGlobalLinearVel = data.Solid.GlobalLinearVel;
                currentGlobalAngularVel = data.Solid.GlobalAngularVel;
                currentLocalLinearVel = data.Solid.LocalLinearVel;
                currentLocalAngularVel = data.Solid.LocalAngularVel;
            }
        }

		protected internal override bool InternalDependsOnSolid(Solid s)
        {
            if (s == data.Solid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
