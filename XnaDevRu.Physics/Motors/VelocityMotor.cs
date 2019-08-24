using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// Tries to achieve given velocity in one time step of the simulation.
    /// </summary>
    public class VelocityMotor : Motor
    {
        /// Stores data describing the Motor.
        protected VelocityMotorData data;

        private Simulator sim;


        public VelocityMotor(Simulator s)
        {
            sim = s;
        }

        /// <summary>
        /// Initializes the Motor with the given data structure.
        /// </summary>
        /// <param name="data"></param>
        public virtual void Init(VelocityMotorData data)
        {
            base.Init();
            this.data = data;

			if (this.data.Solid != null)
            {
				this.data.Solid.LinearDamping = 0;
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
        /// Set target velocity.
        /// </summary>
        public virtual Vector3 Velocity
        {
            get { return data.Velocity; }
            set { data.Velocity = value; }
        }

        /// <summary>
        /// against gravity or not?
        /// * @param affect if true, motor will NOT work against gravity,
        ///     so solid will be affected by gravity
        /// </summary>
        public virtual bool GravityAffectSolid
        {
            get { return data.LetGravityAffectSolid; }
            set { data.LetGravityAffectSolid = value; }
        }

        /// <summary>
        /// Returns the Motor type.
        /// </summary>
        public override MotorType Type
        {
            get
            {
                return MotorType.Velocity;
            }
        }


        /// <summary>
        ///  Gets/Sets Maximum allowed force for the motor to use.
        /// </summary>
        public virtual float MaximumForce
        {
            get { return data.MaxForce; }
            set { data.MaxForce = value; }
        }


        protected internal override void InternalUpdate()
        {
            // check if we have a solid
            if (data.Solid == null || Enabled == false)
                return;

            Vector3 targetVelocity = data.Velocity;
            Solid solid = data.Solid;

            Vector3 currentAchievedVelocity = solid.GlobalLinearVel;

            if (GravityAffectSolid)
            {
                Vector3 gravity = sim.Gravity;

                if (gravity.LengthSquared() > 0)
                {
                    Vector3 gravity_velocity = MathUtil.Project(gravity, currentAchievedVelocity);
                    currentAchievedVelocity -= gravity_velocity;
                }
            }

            Vector3 deltaVelocity = targetVelocity - currentAchievedVelocity;

            Vector3 forceVector = deltaVelocity / sim.StepSize * solid.Mass;

            if (!GravityAffectSolid)
                forceVector -= sim.Gravity * solid.Mass;

            if (forceVector.Length() > MaximumForce)
            {
                forceVector.Normalize();
                forceVector *= MaximumForce;
            }

            Force controllingForce = new Force();
            controllingForce.Duration = 0;
            controllingForce.SingleStep = true;
            controllingForce.Type = ForceType.GlobalForce;
            controllingForce.Direction = forceVector;

            solid.AddForce(controllingForce);
        }

        protected internal override bool InternalDependsOnSolid(Solid s)
        {
            if (data.Solid == s)
                return true;
            else
                return false;
        }
    }
}
