using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A Sensor that queries a scene to find which Solids are intersecting 
    /// a specified volume.  Each volume query returns a data structure 
    /// containing a list of those Solids.  This Sensor does not do a volume 
    /// query every time step because that would be a waste of time in 
    /// most cases; it must be queried manually.
    /// </summary>
    public class VolumeSensor : Sensor
    {
        /// Stores data describing the Sensor.
        protected VolumeSensorData data;

        /// Pointer to the Simulator containing this Sensor; used to fire 
        /// rays into the Simulator.
        protected Simulator sim;


        public VolumeSensor(Simulator s)
        {
            // "mData" is initialized in its own constructor.
            sim = s;
        }

        /// <summary>
   		/// Initializes the Sensor with the given data structure.
        /// </summary>
        /// <param name="data"></param>
		public virtual void Init( VolumeSensorData data)
        {
            base.Init();
            this.data = data;
        }

        /// <summary>
        /// Queries the Sensor's environment with the given Solid's volume, 
        /// returning a list of the Solids that collide with that Solid.  
        /// The given Solid's transform will be totally ignored; use the 
        /// Sensor's transform instead.  If this Sensor is attached to 
        /// a Solid, that Solid will not be added to the results.
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public virtual VolumeQueryResult QueryVolume(Solid volume)
        {
            if (data.Enabled)
            {
                // The volume Solid's transform will be totally ignored.

                // Store the volume Solid's transform.
                Matrix originalVolumeTransform = volume.Transform;

                // If the Sensor is attached to a Solid, we need to transform 
                // the volume relative to the Solid's transform and the 
                // Sensor's transform.
                if (data.Solid != null)
                {
                    volume.Transform = data.Solid.Transform * data.Transform;
                }
                else
                {
                    // If the Sensor is not attached to a Solid, just use the 
                    // Sensor's transform as a global transform on the volume.
                    volume.Transform = data.Transform;
                }

                // If this is attached to a Solid, the Simulator volume query 
                // function will automatically ignore intersections between the 
                // volume and that Solid.

                // Query the volume for colliding Solids.
                VolumeQueryResult result = sim.InternalQueryVolume(volume, data.Solid);

                // Restore the volume Solid's original transform.
                volume.Transform = originalVolumeTransform;

                return result;
            }
            else
            {
                return new VolumeQueryResult();
            }
        }

        /// <summary>
        /// Returns all data describing the Sensor.
        /// </summary>
        public virtual VolumeSensorData Data
        {
            get { return data; }
        }


        public override bool Enabled
        {
            get
            {
                return data.Enabled;
            }
            set
            {
                data.Enabled = value;
            }
        }

        public override SensorType Type
        {
            get { return data.Type; }
        }

        public override Matrix Transform
        {
            get
            {
                return data.Transform;
            }
            set
            {
                data.Transform = value;
            }
        }

        public override string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        protected internal override void InternalUpdate()
        {
            // Do nothing.
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

    /// <summary>
   	/// A data structure containing information about a specific volume query.
    /// </summary>
    public struct VolumeQueryResult
    {
        /// A list of Solids that were found in a volume query.
        List<Solid> solidList;


        /// <summary>
        /// Adds a Solid pointer to the list of results.
        /// </summary>
        /// <param name="s"></param>
        public void InternalAddSolid(Solid s)
        {
            if (s == null)
                return;

            CheckList();

            solidList.Add(s);
        }

        private void CheckList()
        {
            if (solidList == null)
                solidList = new List<Solid>();
        }

        /// <summary>
        /// Returns the number of Solids in the results.
        /// </summary>
        public int NumSolids
        {
            get
            {
                if (solidList == null)
                    return 0;

                return (solidList.Count);
            }
        }

        public Solid GetSolid(int i)
        {
            if (solidList == null)
                return null;

            return solidList[i];
        }

        /// <summary>
        /// Removes all Solids from the results.
        /// </summary>
		public void InternalClearSolids()
        {
            if (solidList == null)
                return;

            solidList.Clear();
        }

        /// <summary>
        /// Removes a specified Solid from the list of collided Solids.
        /// </summary>
        /// <param name="s"></param>
		public void InternalRemoveSolid(Solid s)
        {
            if (solidList == null)
                return;

            solidList.Remove(s);
        }
    }
}
