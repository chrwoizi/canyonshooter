namespace XnaDevRu.Physics
{
    /// <summary>
    /// This is a Motor with an internal feedback loop, allowing 
    /// precise positioning control with minimal overshooting.   This 
    /// Motor only works on rotational Joint axes.  It 
    /// controls a single Joint axis.  Depending on the desired mode of 
    /// operation, it tries to achieve a desired angle or angular velocity 
    /// using up to a limited maximum force.
    /// </summary>
    public class ServoMotor : Motor
    {
        /// Stores data describing the Motor.
        protected ServoMotorData data;

        public ServoMotor()
        {
        }

        /// <summary>
        /// Initializes the Motor with the given data structure.  Joint 
        /// pointer in the data must be valid.
        /// </summary>
        /// <param name="data"></param>
        public virtual void Init(ServoMotorData data)
        {
            if (initCalled)
            {
                // If the Servo is already in operation, we first need to
                // set the old Joint's desired vel and max force to 0.  The
                // following function call will automatically handle this
                // when set to false.
                Enabled = false;
            }

            if (data.Joint == null)
                return;

            if (data.JointAxisNum >= 0 && data.JointAxisNum < data.Joint.NumAxes)
            {

                base.Init();

                if (!data.Joint.IsRotational(data.JointAxisNum))
                    return;

                this.data = data;
                Enabled = data.Enabled;
            }
        }

        public ServoMotorData Data
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

                if (data.Joint != null)
                {
                    if (value)
                    {
                        data.Joint.InternalSetDesiredVel(data.JointAxisNum, data.DesiredVelocity);
                        data.Joint.InternalSetMaxTorque(data.JointAxisNum, data.MaxTorque);
                    }
                    else
                    {
                        data.Joint.InternalSetDesiredVel(data.JointAxisNum, 0);
                        data.Joint.InternalSetMaxTorque(data.JointAxisNum, 0);
                    }
                }
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
        /// Gets/Sets the desired angle to a value between the Joint axis' limits.
		/// Clamps given angle into allowed range for the joint.
        /// </summary>
		public virtual float DesiredAngle
        {
            get { return data.DesiredAngle; }
            set 
            {
                // this clamping is needed since there are sometimes float point errors
                float low = data.Joint.GetLowLimit(data.JointAxisNum);

                float a = value;

                if (a < low)
                {
                    a = low;
                }

                float high = data.Joint.GetHighLimit(data.JointAxisNum);

                if (a > high)
                {
                    a = high;
                }

                data.DesiredAngle = a;
            }
        }

        /// <summary>
		/// Sets the desired angle to a value between 0.0 and 1.0 which 
		/// will be mapped to the Joint axis' limits.
		/// Clamps given angle into [0,1] range.
        /// </summary>
		public virtual float DesiredAngleNorm
        {
            set 
            {
                float a = value;

                if (a < 0)
                    a = 0;
                
                if (a > 1)
                    a = 1;

                float lowLimit = data.Joint.GetLowLimit(data.JointAxisNum);
                float highLimit = data.Joint.GetHighLimit(data.JointAxisNum);

                // map the pos value onto the joint limits
                data.DesiredAngle = a * (highLimit - lowLimit) + lowLimit;

                // Keep desired angle slightly away from the limit to avoid jitter.
                // @todo: fix this; this should just keep the thing away from the
                // limit when it's close, not all the time.
                data.DesiredAngle *= 0.99f;
            }
        }

        /// <summary>
		/// Gets/Sets the desired velocity.  
        /// </summary>
		public virtual float DesiredVel
        {
            get { return data.DesiredVelocity; }
            set 
            {
                data.DesiredVelocity = value;
                data.Joint.InternalSetDesiredVel(data.JointAxisNum, value);
            }
        }


        /// <summary>
		/// Gets/Sets the maximum amount of torque this Motor can use.  
        /// </summary>
		public virtual float MaxTorque
        {
            get { return data.MaxTorque; }
            set 
            {
                data.MaxTorque = value;
                data.Joint.InternalSetMaxTorque(data.JointAxisNum, value);
            }
        }


        /// <summary>
		/// Gets/Sets the restore speed, the parameter used to scale how fast 
		/// the Motor will achieve its desired position.  Only used in the 
		/// desired position mode.
        /// </summary>
		public virtual float RestoreSpeed
        {
            get { return data.RestoreSpeed; }
            set { data.RestoreSpeed = value; }
        }

        protected internal override void InternalUpdate()
        {
            if (data.Enabled && data.Joint != null)
            {
                // Make sure both Solids are awake at this point.
                data.Joint.WakeSolids();

                if ( data.Mode == ServoMotorMode.DesiredAngleMode)
                {
                    // No longer support linear degrees of freedom.
                    //if (true == mData.joint->isRotational(mData.jointAxisNum))
                    //{
                    float velocity = data.DesiredAngle - data.Joint.GetAngle(data.JointAxisNum);
                    
                    if (velocity > 180.0f)
                        velocity = -360 + velocity;
                    
                    if (velocity < -180.0f)
                        velocity = 360 + velocity;
                    
                    data.Joint.InternalSetDesiredVel(data.JointAxisNum, data.RestoreSpeed * velocity);
                    //}
                    //else
                    //{
                    //	// This axis must be a linear degree of freedom.
                    //	real velocity = mData.desiredPos -
                    //		mData.joint->getState(mData.jointAxisNum);
                    //	mData.joint->internal_setDesiredVel(mData.jointAxisNum,
                    //		mData.restoreSpeed * velocity);
                    //}
                }
                else
                {
                    // Nothing to do for desired velocity mode; the Joint's
                    // desired velocity should already handle this.
                }
            }
        }

        protected internal override bool InternalDependsOnJoint(Joint j)
        {
            if (j == data.Joint)
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
