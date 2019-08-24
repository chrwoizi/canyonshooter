namespace XnaDevRu.Physics
{
    /// <summary>
    /// This Motor provides a constant force/torque on a Solid.  Its 
    /// purpose is to simulate physical phenomena where a constant 
    /// force is applied to an object, usually found in a rocket 
    /// thruster.
    /// </summary>
    public class ThrusterMotor : Motor
    {
        /// Stores data describing the Motor.
        protected ThrusterMotorData data;


        public ThrusterMotor()
        {
        }

        /// <summary>
        /// Initializes the Motor with the given data structure.  If the 
        /// Solid pointer in the data are NULL, the Motor will do nothing.  
        /// The Force in this data structure will automatically be set to a 
        /// "single step" Force.
        /// </summary>
        /// <param name="data"></param>
        public virtual void Init(ThrusterMotorData data)
        {
            base.Init();
            this.data = data;
            this.data.Force.SingleStep = true;
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

        public override MotorType Type
        {
            get { return data.Type; }
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
        /// Returns all data describing the Motor.
        /// </summary>
        public virtual ThrusterMotorData Data
        {
            get { return data; }
        }

        /// <summary>
        /// Sets the Force applied by this Motor every time step.  The 
        /// Force in this data structure will automatically be set to a 
        /// "single step" Force.
        /// </summary>
        public virtual Force Force
        {
            get { return data.Force; }
            set
            {
                data.Force = value;                
                data.Force.SingleStep = true;
            }
        }


        protected internal override void InternalUpdate()
        {
            if (data.Enabled && data.Solid != null)
            {
                data.Solid.AddForce(data.Force);
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
