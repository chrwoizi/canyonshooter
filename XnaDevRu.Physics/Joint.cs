using System;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A constraint between two Solids or between a Solid and the static
    /// environment.  There is a variety of Joint types, each constraining
    /// Solid motion in different ways.  There are 6 degrees of freedom
    /// for a Solid that can be constrained: 3 rotational and 3 linear.
    /// Each Joint type constrains a different subset of these 6.  When
    /// specifying the two Solids
    /// affected by the Joint, if at least one Solid
    /// is non-NULL, the Joint will be enabled.  If both Solids are
    /// NULL, the Joint will be disabled.  If only one Solid is NULL,
    /// the Joint will attach the other Solid to the static environment.
    /// If both Solids are the same, the Joint's Solids will both be set
    /// to NULL and the Joint will be disabled.  If either Solid is static,
    /// both will be set to NULL, and the Joint will be disabled.
    /// Note that some Joint types
    /// do not use all of the anchor and axis parameters.  An unused anchor
    /// or axis will be ignored (see the JointType description for more
    /// details).  Joints remain ineffective until they are initialized.
    /// </summary>
    public abstract class Joint
    {

        #region fields
        /// Stores data describing the Joint.
        protected JointData data;

        /// A pointer to the Joint's break event handler.
        protected JointBreakEventHandler jointBreakEventHandler;

        /// Pointer to user data.  This is totally user-managed (i.e. OPAL
        /// will never delete it).
        protected object userData;

        /// This is set to true when the Joint is initialized.
        protected bool initCalled;

        /// The number of axes used by the Joint.
        protected int numAxes;

        // This data stores which axes are rotational, as opposed to
        // translational, degrees of freedom.
        protected bool[] axisRotational;

        #endregion

        public Joint()
        {
            // "mData" is initialized in its own constructor.
            initCalled = false;
            numAxes = 0;
            axisRotational = new bool[3];
            axisRotational[0] = false;
            axisRotational[1] = false;
            axisRotational[2] = false;

			data = new JointData();
        }

        /// <summary>
        /// Initializes the Joint with the given data structure.  Calling
        /// this more than once will automatically detach the Joint from
        /// its old Solids first.
        /// </summary>
        /// <param name="data"></param>
        public virtual void Init(JointData data)
        {
            this.data = data;
            initCalled = true;
        }

        /// <summary>
        /// Returns all data describing the Joint.
        /// </summary>
        public virtual JointData Data
        {
            get 
            {
                // Update parameters that don't get updated automatically.
                for (int i = 0; i < numAxes; ++i)
                    data.Axis[i] = GetAxis(i);

                data.Anchor = Anchor;

                return data;
            }
        }

        /// <summary>
        /// Gets/Sets the Joint's name.
        /// </summary>
        public virtual string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }


        /// <summary>
        /// Sets whether the Joint's two Solids are constrained by
        /// physical contacts.
        /// </summary>
        public virtual bool ContactsEnabled
        {
            get { return data.ContactsEnabled; }
            set { data.ContactsEnabled = value; }
        }

        /// <summary>
        /// Returns the Joint type.
        /// </summary>
        public virtual JointType Type
        {
            get { return data.Type; }
        }

        /// <summary>
        /// Sets the parameters that determine how this Joint will break, if
        /// at all.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="breakThresh"></param>
        /// <param name="accumThresh"></param>
        public virtual void SetBreakParams(JointBreakMode mode, float breakThresh, float accumThresh)
        {
            data.BreakMode = mode;
            data.BreakThresh = breakThresh;
            data.AccumThresh = accumThresh;
        }

        /// <summary>
        ///  What is the mode of breaking?
        /// </summary>
        public virtual JointBreakMode BreakingMode
        {
            get { return data.BreakMode; }
        }

        /// <summary>
        /// How much damage can a Joint take?
        /// </summary>
        public virtual float BreakThresh
        {
            get { return data.BreakThresh; }
        }

        /// <summary>
        /// How much damage has a Joint taken so far?
        /// </summary>
        public virtual float AccumulatedDamage
        {
            get { return data.AccumDamage; }
        }

        /// <summary>
        /// What is minimum amount of damage that will be recorded?
        /// note if the damaga is lower than this value, that damage will be ignored
        /// </summary>
        public virtual float AccumulatedThresh
        {
            get { return data.AccumThresh; }
        }

        /// <summary>
        /// Returns true if the Joint has been broken
        /// </summary>
        public virtual bool IsBroken
        {
            get { return data.IsBroken; }
        }

        /// <summary>
        /// Repairs accumulated damage to breakable Joints in accumulated
        /// damage mode.  This does not reenable the Joint.
        /// </summary>
        public virtual void RepairAccumDamage()
        {
            data.AccumDamage = 0;
            //mIsBroken = false;
        }

        /// <summary>
        /// Gets/Sets the Joint's break event handler.
        /// </summary>
        public virtual JointBreakEventHandler JointBreakEventHandler
        {
            get { return jointBreakEventHandler; }
            set { jointBreakEventHandler = value; }
        }

        /// <summary>
        /// Enables or disables the given Joint axis' limits.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="e"></param>
        public virtual void SetLimitsEnabled(int axisNum, bool e)
        {
            if (axisNum >= 0 && axisNum < numAxes)
                data.Axis[axisNum].LimitsEnabled = e;
        }

        /// <summary>
        /// Returns true if the given Joint axis' limits are enabled.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public virtual bool AreLimitsEnabled(int axisNum)
        {
            if (axisNum >= 0 && axisNum < numAxes)
                return data.Axis[axisNum].LimitsEnabled;

            return true;
        }

        /// <summary>
        /// Sets the Joint's limit angles (in degrees for rotational axes,
        /// distance for translational axes).
        /// No limits are applied if this is not called.  The Wheel Joint does not
        /// use limits for axis 1, so setting this will do nothing.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public virtual void SetLimitRange(int axisNum, float low, float high)
        {
            if(high >= low)
                if (axisNum >= 0 && axisNum < numAxes)
                {
                    data.Axis[axisNum].Limits.Low = low;
                    data.Axis[axisNum].Limits.High = high;
                }
        }

        /// <summary>
        /// Returns the low limit for a given axis (angle in degrees for
        /// rotational axes, distance for translational axes).
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public virtual float GetLowLimit(int axisNum)
        {
            if (axisNum >= 0 && axisNum < numAxes)
                return data.Axis[axisNum].Limits.Low;

            return 0;
        }

        /// <summary>
        /// Returns the high limit for a given axis (angle in degrees for
        /// rotational axes, distance for translational axes).
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public virtual float GetHighLimit(int axisNum)
        {
            if (axisNum >= 0 && axisNum < numAxes)
                return data.Axis[axisNum].Limits.High;
            
            return 0;
        }

        /// <summary>
        /// Sets the hardness for the given axis' limits.  Hardness
        /// represents how "squishy" the limit is.  Hardness must be
        /// between 0 and 1, inclusive.  Setting the hardness for axis 1
        /// of the Wheel Joint will adjust its suspension.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="h"></param>
        public virtual void SetLimitHardness(int axisNum, float h)
        {
            if (h >= 0 && h <= 1)
                if (axisNum >= 0 && axisNum < numAxes)
                    data.Axis[axisNum].Limits.Hardness = h;
        }

        /// <summary>
        /// Returns the hardness for the given axis' limits.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public virtual float GetLimitHardness(int axisNum)
        {
            if (axisNum >= 0 && axisNum < numAxes)
                return data.Axis[axisNum].Limits.Hardness;

            return 0;
        }

        /// <summary>
        /// Sets the bounciness for the given axis' limits.  Bounciness
        /// (i.e. restitution) represents how much the Joint will bounce
        /// when it hits a limit.  Bounciness must be between 0 and 1,
        /// inclusive.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="b"></param>
        public virtual void SetLimitBounciness(int axisNum, float b)
        {
            if (b >= 0 && b <= 1)
                if (axisNum >= 0 && axisNum < numAxes)
                    data.Axis[axisNum].Limits.Bounciness = b;
        }


        /// <summary>
        /// Returns the bounciness for the given axis' limits.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public virtual float GetLimitBounciness(int axisNum)
        {
            if (axisNum >= 0 && axisNum < numAxes)
                return data.Axis[axisNum].Limits.Bounciness;

            return 0;
        }

        /// <summary>
        /// For rotational axes, returns the current angle in degrees
        /// measured from the initial Joint configuration.  For translational
        /// axes, simply returns 0.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public abstract float GetAngle(int axisNum);

        /// <summary>
        /// For translational axes, returns the distance from the initial
        /// Joint configuration.  For rotational axes, simply returns 0.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public abstract float GetDistance(int axisNum);


        /// <summary>
        /// Returns the current rate (degrees per second for rotational
        /// axes, distance units per second for translational axes) for a
        /// given axis.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public abstract float GetVelocity(int axisNum);

        /// <summary>
        /// Applies a force to this Joint's Solid(s).  To be used for
        /// translational axes.  This does nothing if the Joint is disabled.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="magnitude"></param>
        /// <param name="duration"> Specifies how long to apply to force.  This makes it easy to
        /// apply forces independent of the step size.  This will be ignored
        /// if "singleStep" is true.</param>
        /// <param name="singleStep">It specifies that
        /// the force will be applied across a single time step.  If this
        /// parameter is true, "duration" will be ignored.</param>
        public virtual void AddForce(int axisNum, float magnitude, float duration, bool singleStep)
        {
            if (axisNum < 0 || axisNum >= numAxes)
				throw new ArgumentOutOfRangeException("axisNum");

            if (data.Enabled)
            {
                Force f = new Force();
                f.SingleStep = singleStep;

                // We only care about the duration if this is not a single-step
                // force.
                if (!f.SingleStep)
                {
                    f.Duration = duration;
                }

                f.Type =  ForceType.LocalForce;
                Vector3 direction = data.Axis[axisNum].Direction;
                f.Direction = magnitude * direction;

                if (data.Solid0 != null)
                {
                    data.Solid0.AddForce(f);
                }

                f.Direction *= -1.0f;

                if (data.Solid1 != null)
                {
                    data.Solid1.AddForce(f);
                }
            }
        }

        /// <summary>
        /// Applies a torque to this Joint's Solid(s).  To be used for
        /// rotational Joints. This does nothing if the Joint is disabled.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="magnitude"></param>
        /// <param name="duration"> Specifies how long to apply to force.  This makes it easy to
        /// apply forces independent of the step size.  This will be ignored
        /// if "singleStep" is true.
        /// </param>
        /// <param name="singleStep">It specifies that
        /// the force will be applied across a single time step.  If this
        /// parameter is true, "duration" will be ignored.
        /// </param>
        public virtual void AddTorque(int axisNum, float magnitude, float duration, bool singleStep)
        {
            if (axisNum < 0 || axisNum >= numAxes)
				throw new ArgumentOutOfRangeException("axisNum");

            if (data.Enabled)
            {
                Force f = new Force();
                f.SingleStep = singleStep;

                // We only care about the duration if this is not a single-step
                // force.
                if (!f.SingleStep)
                {
                    f.Duration = duration;
                }

                f.Type =  ForceType.LocalTorque;
                Vector3 axis = data.Axis[axisNum].Direction;
                f.Direction = magnitude * axis;

                if (data.Solid0 != null)
                {
                    data.Solid0.AddForce(f);
                }

                f.Direction *= -1.0f;

                if (data.Solid1 != null)
                {
                    data.Solid1.AddForce(f);
                }
            }
        }

        /// <summary>
        /// Wakes up this Joint's two Solids.
        /// </summary>
        public virtual void WakeSolids()
        {
            if (data.Solid0 != null)
                data.Solid0.Sleeping = false;

            if (data.Solid1 != null)
                data.Solid1.Sleeping = false;
        }

        /// <summary>
        /// Returns a pointer to Solid0.
        /// </summary>
        public virtual Solid Solid0
        {
			get { return this.data.Solid0; }
        }

        /// <summary>
        /// Returns a pointer to Solid1.
        /// </summary>
        public virtual Solid Solid1
        {
			get { return this.data.Solid1; }
        }

        /// <summary>
        /// Returns the current specified axis in global coordinates.
        /// Passing in an invalid axis number will return invalid data.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public abstract JointAxis GetAxis(int axisNum);

        /// <summary>
        /// Returns the current anchor point in global coordinates.
        /// Passing in an invalid axis number will return invalid data.
        /// </summary>
        public abstract Vector3 Anchor { get; }

        /// <summary>
        /// Returns the number of axes used by this Joint.
        /// </summary>
        public virtual int NumAxes
        {
            get { return numAxes; }
        }

        /// <summary>
        /// Gets/Sets whether the Joint can affect its Solids.  If both Solids are
        /// NULL, this will remain disabled.  If the Joint has not yet
        /// been initialized, this will have no effect.
        /// </summary>
        public virtual bool Enabled
        {
			get { return data.Enabled; }
            set 
            {
                data.Enabled = value;
            }
        }


        /// <summary>
        /// Returns true if the given Joint axis is rotational, false if it is linear.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <returns></returns>
        public virtual bool IsRotational(int axisNum)
        {
			if (axisNum >= 0 && axisNum < numAxes)
				return axisRotational[axisNum];

			return false;
        }

        /// <summary>
        /// Set the user data pointer to some external data.  The user data
        /// is totally user-managed
        /// </summary>
        public virtual object UserData
        {
            get { return this.userData; }
            set { this.userData = value; }
        }

        /// <summary>
        /// Various things could be updated here, including damage values.
        /// If the Joint breaks during this update, it will automatically
        /// be disabled, and the event handler will be notified.
        /// </summary>
        protected internal abstract void InternalUpdate();

        /// <summary>
        /// Set the desired linear or angular velocity for this Joint.
        /// This is to be used internally by Motors.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="value"></param>
        protected internal abstract void InternalSetDesiredVel(int axisNum, float value);

        /// <summary>
        /// Set the max force this Joint can use to attain its desired
        /// velocity.  This is to be used internally by Motors.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="value"></param>
        protected internal abstract void InternalSetMaxTorque(int axisNum, float value);

        //virtual bool OPAL_CALL internal_isBroken();

        /// <summary>
        /// Returns true if this Joint depends on the given Solid.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected internal virtual bool InternalDependsOnSolid(Solid s)
        {
            if (s == data.Solid0 || s == data.Solid1)
                return true;

			return false;
        }

        /// <summary>
        /// Sets the Solids constrained by this Joint.
        /// </summary>
        /// <param name="s0"></param>
        /// <param name="s1"></param>
        protected void SetSolids(Solid s0, Solid s1)
        {
            data.Solid0 = s0;
            data.Solid1 = s1;
        }

        /// <summary>
        /// Sets the anchor point for this Joint.  Both Solids must be
        /// valid (non-NULL) before this is called for it to affect anything.
        /// This Joint's Solids must be positioned and attached
        /// before calling this function.
        /// </summary>
        /// <param name="anchor"></param>
        protected virtual void SetAnchor(Vector3 anchor)
        {
            data.Anchor = anchor;
        }

        /// <summary>
        /// Specifies the given axis for this Joint.  Invalid axes numbers
        /// will be silently ignored.  The axis direction vector will be
        /// normalized.  This Joint's Solids must be positioned and
        /// attached before calling this function.
        /// </summary>
        /// <param name="axisNum"></param>
        /// <param name="axis"></param>
        protected virtual void SetAxis(int axisNum, JointAxis axis)
        {
            if (axisNum >= 0 && axisNum < numAxes)
                data.Axis[axisNum] = axis;
        }

        /// <summary>
        /// Updates this Joint's current damage status based on the current
        /// amount of stress.
        /// </summary>
        /// <param name="currentStress"></param>
        protected void UpdateDamage(float currentStress)
        {
            switch (data.BreakMode)
            {
                case JointBreakMode.Unbreakable:
                    //nothing to do
                    break;

                case JointBreakMode.Threshold:
                    {
                        if (currentStress >= data.BreakThresh)
                            data.IsBroken = true;

                        break;
                    }

                case JointBreakMode.Accumulated:
                    {
                        if (currentStress >= data.AccumThresh)
                            data.AccumDamage += currentStress;

                        if (data.AccumDamage >= data.BreakThresh)
                            data.IsBroken = true;

                        break;
                    }

                default:
                    throw new InvalidOperationException();
            }

            if (data.IsBroken)
            {
                this.Enabled = false;

                if (jointBreakEventHandler != null)
                    jointBreakEventHandler(this);
            }

        }

        public override string ToString()
        {
            string disable = Enabled ? " Enabled" : " Disabled";
            return "Joint: " + Name + " " + this.Type + disable;
        }


    }

    /// <summary>
    /// A listener handler that gets notified when a particular Joint breaks. 
    /// </summary>
    /// <param name="joint"></param>
    public delegate void JointBreakEventHandler(Joint joint);

    /// <summary>
    /// Joints use different break modes to determine how they are damaged
    /// from stress.
    /// </summary>
    public enum JointBreakMode
    {
        /// <summary>
        /// Joint can never break.
        /// </summary>
        Unbreakable,

        /// <summary>
        /// Joint breaks when force/torque exceeds a threshold.
        /// </summary>
        Threshold,

        /// <summary>
        /// Joint breaks when enough damage is accumulated.
        /// </summary>
        Accumulated
    };

    /// <summary>
    /// The types of Joints currently available.
    /// </summary>
    public enum JointType
    {
        /// <summary>
        /// Anchor: used
        /// Axis 0: rotational
        /// Axis 1: not used
        /// Axis 2: not used
        /// </summary>
        Hinge,

        /// <summary>
        /// Anchor: used
        /// Axis 0: rotational
        /// Axis 1: rotational
        /// Axis 2: not used
        /// </summary>
        Universal,

        /// <summary>
        /// Anchor: used
        /// Axis 0: rotational
        /// Axis 1: rotational (need not be set; calculated automatically)
        /// Axis 2: rotational
        /// </summary>
        Ball,

        /// <summary>
        /// Anchor: not used
        /// Axis 0: linear
        /// Axis 1: not used
        /// Axis 2: not used
        /// </summary>
        Slider,

        /// <summary>
        /// Anchor: used
        /// Axis 0: rotational ("steering axis")
        /// Axis 1: rotational ("wheel axis")
        /// Axis 2: not used
        /// </summary>
        Wheel,

        /// <summary>
        /// Anchor: not used
        /// Axis 0: not used
        /// Axis 1: not used
        /// Axis 2: not used
        /// </summary>
        Fixed
    };
}
