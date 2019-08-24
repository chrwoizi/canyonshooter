using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A Sensor that fires a ray into a scene and returns data describing
    /// the closest intersection, if any.  This Sensor does not fire a ray
    /// every time step because that would be a waste of time in most
    /// cases; it must be "fired" manually.
    /// </summary>
    public class RaycastSensor : Sensor
    {
        /// Stores data describing the Sensor.
        protected RaycastSensorData data;

        /// Pointer to the Simulator containing this Sensor; used to fire
        /// rays into the Simulator.
        protected Simulator sim;

        public RaycastSensor(Simulator s)
        {
            // "mData" is initialized in its own constructor.
            sim = s;
        }

        /// <summary>
        /// Initializes the Sensor with the given data structure.  If the
        /// Solid pointer in the data is valid, the Sensor's offset will
        /// be relative to the Solid's transform instead of the global
        /// origin.
        /// </summary>
        /// <param name="data"></param>
        public virtual void Init(RaycastSensorData data)
        {
            base.Init();
            this.data = data;
        }

        /// <summary>
        /// Fires a ray into the Sensor's environment, returning
        /// information about the closest intersection encountered.  The
        /// length of the ray used will be the RaycastSensorData length.
        /// </summary>
        /// <returns></returns>
        public virtual RaycastResult FireRay()
        {
            return FireRay(data.Ray.Direction.Length());
        }

        /// <summary>
        /// Same as fireRay, except that this function returns a set of sorted results.
        ///  * @note The length of the ray will be the RaycastSensorData length.
        ///  * @return All intersections, sorted by their distance from the base of the ray.
        /// </summary>
        /// <returns>All intersections, sorted by their distance from the base of the ray.</returns>
        public virtual List<RaycastResult> FirePiercingRay()
        {
            return FirePiercingRay(data.Ray.Direction.Length());
        }

        /// <summary>
        /// Same as the other fireRay, except that the length is an explicit parameter.
        /// </summary>
        /// <param name="length">The maximum distance of the ray cast.</param>
        /// <returns>The closest intersection that was encountered.</returns>
        public virtual RaycastResult FireRay(float length)
        {
            RaycastResult junkResult = new RaycastResult();

            if (data.Enabled)
            {
                Ray ray = data.Ray;

                // If the Sensor is attached to a Solid, we need to transform
                // the ray relative to that Solid's transform.
                if (data.Solid != null)
                {
                    ray = MathUtil.TransformRay(data.Solid.Transform * data.Transform, ray);
                }
                else
                {
                    // If the Sensor is not attached to a Solid, just use the 
                    // Sensor's transform as a global transform on the ray.
                    ray = MathUtil.TransformRay(data.Transform, ray);
                }

                // If this is attached to a Solid, the Simulator raycast function
                // will automatically ignore intersections between the ray and
                // that Solid.

                List<RaycastResult> results = sim.InternalFireRay(ray, length, data.Solid, data.ContactGroup);

                if (results == null || results.Count == 0)
                    return junkResult;

                int closest = 0;

                for (int i = 1; i < results.Count; i++)
                {
                    if (results[i].Distance < results[closest].Distance)
                        closest = i;
                }

                return results[closest];
            }
            else
            {
                return junkResult;
            }
        }

        /// <summary>
        /// Same as the other firePiercingRay, except that the length is an explicit parameter.
        /// </summary>
        /// <param name="length">The maximum distance of the ray cast.</param>
        /// <returns>All intersections, sorted by their distance from the base of the ray.</returns>
        public virtual List<RaycastResult> FirePiercingRay(float length)
        {
            List<RaycastResult> empty = new List<RaycastResult>();

            if (data.Enabled)
            {
                // the original ray shouldn't change
                Ray ray = data.Ray;

                // If the Sensor is attached to a Solid, we need to transform
                // the ray relative to that Solid's transform.
                if (data.Solid != null)
                {
                    ray = MathUtil.TransformRay(data.Solid.Transform * data.Transform, ray);
                }
                else
                {
                    // If the Sensor is not attached to a Solid, just use the 
                    // Sensor's transform as a global transform on the ray.
                    ray = MathUtil.TransformRay(data.Transform, ray);
                }

                // If this is attached to a Solid, the Simulator raycast function
                // will automatically ignore intersections between the ray and
                // that Solid.

                List<RaycastResult> results = sim.InternalFireRay(ray, length, data.Solid, data.ContactGroup);

                results.Sort(RaycastResult.Compare);

                return results;
            }
            else
            {
                return empty;
            }
        }

        /// <summary>
        /// Returns all data describing the Sensor.
        /// </summary>
        public RaycastSensorData Data
        {
            get { return data; }
        }

        public virtual Ray Ray
        {
            get { return data.Ray; }
            set { data.Ray = value; }
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
    /// A data structure containing information about a specific
    /// intersection from a ray cast.
    /// </summary>
    public struct RaycastResult
    {

        /// <summary>
        /// The first Solid hit by the ray.  This will remain NULL if no
        /// Solid is hit.
        /// </summary>
        public Solid Solid;

        /// <summary>
        /// The point of intersection.
        /// </summary>
        public Vector3 Intersection;

        /// <summary>
        /// The normal vector at the point of intersection.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The distance from the base of the ray to the intersection
        /// point.
        /// </summary>
        public float Distance;

        public static bool operator <(RaycastResult l, RaycastResult r)
        {
            return l.Distance < r.Distance;
        }

        public static bool operator >(RaycastResult l, RaycastResult r)
        {
            return l.Distance > r.Distance;
        }


        /// <summary>
        /// Represents the method that compares two RaycastResult objects.
        /// </summary>
        /// <returns>Value Condition Less than 0 - l is less than r. 0 - l equals r.Greater than 0 - l is greater than r.</returns>
        public static int Compare(RaycastResult l, RaycastResult r)
        {
            if (l.Distance < r.Distance)
                return -1;

            if (l.Distance > r.Distance)
                return 1;

            return 0;
        }
    }
}
