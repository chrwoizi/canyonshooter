using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A Sensor that measures a Solid's "tilt."  This is similar to a real 
    /// inclinometer that monitors how an object is oriented with respect 
    /// to gravity.  This Sensor takes a rotation axis (defined relative to 
    /// the attached Solid's transform) and returns an angle describing how 
    /// far the Solid has rotated around that axis.  The rotation of the 
    /// Solid is defined as zero degrees when the Sensor is initialized or 
    /// when the rotation axis is redefined.  This Sensor does 
    /// nothing if it is not attached to a Solid (i.e. its returned values 
    /// are always zero).
    /// </summary>
    public class InclineSensor : Sensor
    {
        /// Stores data describing the Sensor.
        protected InclineSensorData data;

        /// A vector used to measure the angle of rotation.
        protected Vector3 localReferenceVec;

        /// A vector used to measure the angle of rotation.
        protected Vector3 initGlobalReferenceVec;

        public InclineSensor()
        {
        }

        /// A helper function that sets up all internal vectors used when 
        /// calculating the angle of rotation.
        protected void SetupInternalVectors()
        {
            if (data.Solid == null)
                return;

            // We need to calculate a local reference vector that's orthogonal 
            // to the rotation axis.

            // This temporary vector can be anything as long as its not 
            // collinear with the rotation axis.
            Vector3 tempVec = new Vector3(1, 0, 0);

            if (MathUtil.AreCollinear(data.Axis, tempVec))
            {
                // Pick a new tempVec.
                tempVec = new Vector3(0, 1, 0);
            }

            // Now we can get the orthogonal reference vector.
            localReferenceVec = Vector3.Cross(data.Axis, tempVec);

            // Also store a copy of this reference vector in its initial global 
            // representation.
            initGlobalReferenceVec = Vector3.Transform(localReferenceVec, data.Solid.Transform);
        }
    

        /// <summary>
   		/// Initializes the Sensor with the given data structure.  This will 
		/// define the rotation angle as zero degrees when called.  The Solid 
		/// pointer should be valid because this Sensor only works when 
		/// attached to something.  This does nothing if the Sensor's Solid 
		/// pointer is NULL.
        /// </summary>
        /// <param name="data"></param>
		public virtual void Init(InclineSensorData data)
        {
            base.Init();

            this.data = data;

            // Define the current setup as zero degrees.
            SetupInternalVectors();

        }

        /// <summary>
		/// Returns all data describing the Sensor.
        /// </summary>
		public virtual InclineSensorData Data
        {
            get { return data; }
        }

        /// <summary>
        /// Returns the angle of rotation about the local rotation axis 
        /// relative to the initial angle of rotation.
        /// </summary>
        public virtual float Angle
        {
            get 
            {
                if (data == null || !data.Enabled || data.Solid == null)
                {
					throw new PhysicsDataException();
                }

                // By this time setupInternalVectors should have been called, so 
                // the mLocalReferenceVec and mInitGlobalReferenceVec vectors are 
                // valid.

                // Let's call the plane orthogonal to the rotation axis the 
                // "plane of rotation."  The local reference vector is currently on 
                // the plane of rotation.  We need to get the initial global 
                // reference vector projected onto the plane of rotation.

                Vector3 currentGlobalReferenceVec = Vector3.Transform(localReferenceVec, data.Solid.Transform);

                if (MathUtil.AreCollinear(initGlobalReferenceVec, currentGlobalReferenceVec))
                {
                    // Return zero degrees if the initial global reference vector 
                    // is collinear with the current global reference vector.
                    return 0;
                }

                Vector3 tempVec = Vector3.Cross(data.Axis, initGlobalReferenceVec);
                Vector3 u = Vector3.Cross(data.Axis, tempVec);

                // Now 'u' should be on the plane of rotation.  We just need to 
                // project the initial global reference vector onto it.

                Vector3 projInitGlobalReferenceVec = MathUtil.Project(u,
                    initGlobalReferenceVec);

                // Now calculate the angle between the projected global reference 
                // vector and the current global reference vector.
                float angle = MathUtil.AngleBetween(projInitGlobalReferenceVec,
                    currentGlobalReferenceVec);

                return angle;
            }
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

        /// <summary>
        /// Gets/Sets the local rotation axis around which the angle of rotation 
        /// will be measured.  This will redefine the rotation angle as zero 
        /// degrees when called.  This does nothing if the Sensor's Solid 
        /// pointer is NULL.
        /// </summary>
        public virtual Vector3 Axis
        {
            get
            {
                return data.Axis;
            }
            set
            {
                if (data == null || data.Solid == null)
                    throw new PhysicsDataException();

                data.Axis = value;

                // Redefine the current setup as zero degrees.
                SetupInternalVectors();

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
}
