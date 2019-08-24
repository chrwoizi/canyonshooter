namespace XnaDevRu.Physics
{
    /// <summary>
    /// The GearedMotor is intended to simulate an automobile engine.  
    /// This Motor only works on rotational Joint axes.  It applies a torque 
    /// to one of its degrees of freedom, but the amount of torque decreases 
    /// linearly as the angular velocity increases, similar to an automobile 
    /// engine.  The Motor's max torque is applied when angular velocity is 
    /// zero.  When the angular velocity reaches the Motor's max velocity, 
    /// zero torque is applied.  Thus, there is always more torque 
    /// available when the Joint axis is at the lower end of its velocity 
    /// </summary>
    public class GearedMotor : Motor
    {
		/// <summary>
		/// Stores data describing the Motor.
		/// </summary>
        protected GearedMotorData data;

		/// <summary>
		/// Returns or sets all data describing the Motor.
		/// </summary>
		public GearedMotorData Data { get { return data; } set { Init(value); } }
		/// <summary>
		/// Returns or sets the max torque parameter.
		/// </summary>
		public float MaxTorque { get { return data.MaxTorque; } set { data.MaxTorque = value; } }
		/// <summary>
		/// Returns or sets the max velocity parameter. The maximum cannot be set to 
		/// zero.
		/// </summary>
		public float MaxVelocity { get { return data.MaxVelocity; } set { data.MaxVelocity = value; } }
		/// <summary>
		/// Returns or sets the throttle parameter.
		/// </summary>
		public float Throttle { get { return data.Throttle; } set { data.Throttle = value; } }

        public GearedMotor()
			: base()
        {
        }

		/// <summary>
		/// Initializes the Motor with the given data structure.  Joint 
		/// in the data must be valid.
		/// </summary>
		/// <param name="data">Given data structure.</param>
		public void Init(GearedMotorData data)
		{
			if (data.Joint == null)
				return; // TODO: exceptions

			Init();

			if (!data.Joint.IsRotational(data.JointAxisNum))
				return;

			this.data = data;
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

        protected internal override void InternalUpdate()
        {
			if (data.Enabled && data.Joint != null)
			{
				// Global/local direction makes no difference here since 
				// we simply use the joint axis.
				float normalizedVel = data.Joint.GetVelocity(data.JointAxisNum) / data.MaxVelocity;
				float magnitude = data.MaxTorque * (data.Throttle - normalizedVel);
				data.Joint.AddTorque(data.JointAxisNum, magnitude, 0, true);
			}
        }

		protected internal override bool InternalDependsOnJoint(Joint j)
		{
			if (j == data.Joint)
				return true;
			else
				return false;
		}
    }
}
