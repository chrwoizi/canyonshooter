namespace XnaDevRu.Physics
{
    /// <summary>
	/// A device that can be attached to Solids and/or Joints to affect 
	/// them in a variety of ways.  The point of Motors is to give users 
	/// high-level systems for controlling various parts of the simulation.  
	/// For example, instead of manually applying Forces to a robot arm to 
	/// attain a desired state, users should use a Motor that takes a 
	/// desired position or velocity, automatically applying forces every 
	/// time step to attain that state.  Most Motors remain ineffective 
	/// until they are initialized.
    /// </summary>
    public abstract class Motor
    {

        #region fields
        /// Pointer to user data.  This is totally user-managed (i.e. OPAL 
        /// will never delete it).  
        protected object userData;

        /// True if this is a custom Motor.  This is used to ensure that 
        /// custom Motors are not destroyed by OPAL.
        //bool mIsCustom;

        /// True if the Motor has been initialized.  Some Motors use 
        /// this to take special actions when a Motor is reinitialized.
        protected bool initCalled;

        #endregion

        public Motor()
        {
            //mIsCustom = false;
            initCalled = false;

        }

        /// <summary>
        /// Sets whether the Motor has any effect.
        /// </summary>
        public abstract bool Enabled { get; set; }


        /// <summary>
        /// Set the user data pointer to some external data.  The user data 
        /// is totally user-managed
        /// </summary>
        public virtual object UserData
        {
            get { return userData; }
            set { userData = value; }
        }


        /// <summary>
        /// Returns the Motor type.  
        /// </summary>
        public abstract MotorType Type
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the Motor's name.
        /// </summary>
        public abstract string Name { get; set; }


        /// <summary>
        /// Called regularly to update the Motor.  This does nothing if the 
        /// Motor is disabled.
        /// </summary>
        protected internal abstract void InternalUpdate();

        /// <summary>
        /// Returns true if this Motor depends on the given Solid.  
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected internal virtual bool InternalDependsOnSolid(Solid s)
        {
            return false;
        }

        /// <summary>
        /// Returns true if this Motor depends on the given Joint.  
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        protected internal virtual bool InternalDependsOnJoint(Joint j)
        {
            return false;
        }

        /// Sets whether this is a custom Motor.  Used internally to 
        /// track custom Motors which must not be destroyed by OPAL.
        //virtual void OPAL_CALL internal_setCustom(bool c);

        /// Returns true if this is a custom Motor.  
        //virtual bool OPAL_CALL internal_isCustom();


        /// <summary>
        /// Called by subclasses when they are initialized.  
        /// </summary>
        protected void Init()
        {
            initCalled = true;
        }

        public override string ToString()
        {
            string disable = Enabled ? " Enabled" : " Disabled";
            return "Motor: " + Name + " " + this.Type + disable;
        }

    }

    /// <summary>
   	/// The types of Motors currently available.
    /// </summary>
	public enum MotorType
	{
		Attractor,
		Geared,
		Servo,
		Spring,
		Thruster,
        Velocity
	};
}
