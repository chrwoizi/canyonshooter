using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// This is a spring that either 1) operates in 3 dimensions to keep
    /// a Solid in a desired position, or 2) operates in 3 degrees of
    /// rotational motion to keep a Solid in a desired orientation.  It can
    /// also do both modes at once.
    /// </summary>
    public class SpringMotor : Motor
    {
        /// Stores data describing the Motor.
        protected SpringMotorData data;


        public SpringMotor()
        {
        }

        /// <summary>
        /// Initializes the Motor with the given data structure.  If the
        /// Solid pointer in the data are NULL, the Motor will do nothing.
        /// </summary>
        /// <param name="data"></param>
        public virtual void Init(SpringMotorData data)
        {
            base.Init();
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

        /// <summary>
        /// Sets the spring's attach point on the Solid.  This is a local
        /// offset point from the Solid's position.
        /// </summary>
        public virtual Vector3 LocalAttachOffset
        {
            get { return data.AttachOffset; }
            set
            {
                data.AttachOffset = value;
            }
        }

        /// <summary>
        /// Gets/Sets the spring's attach point on the Solid in global
        /// coordinates.
        /// </summary>
        public virtual Vector3 GlobalAttachPoint
        {
            get
            {
                if (data.Solid == null)
                {
                    return new Vector3();
                }

                // The global position is a combination of the Solid's global 
                // transform and the spring's local offset from the Solid's 
                // transform.
                Vector3 offset = new Vector3(data.AttachOffset.X, data.AttachOffset.Y, data.AttachOffset.Z);

                Vector3 globalPos = data.Solid.Position + Vector3.Transform(offset, data.Solid.Transform);

                return globalPos;
            }
            set
            {
                if (data.Solid == null)
                {
                    return;
                }

                // Convert the global point to a local point offset from the Solid's 
                // transform.
                Matrix inv = Matrix.Invert(data.Solid.Transform);

                data.AttachOffset = Vector3.Transform(value, inv);
            }
        }

        /// <summary>
        /// Gets/Sets the desired position and orientation.
        /// </summary>
        public virtual Matrix DesiredTransform
        {
            set
            {
                data.DesiredPosition = value.Translation;

                data.DesiredForward = value.Forward;

                if (0 != data.DesiredForward.LengthSquared())
                {
                    data.DesiredForward.Normalize();
                }

                data.DesiredUp = value.Up;
                if (0 != data.DesiredUp.LengthSquared())
                {
                    data.DesiredUp.Normalize();
                }

                data.DesiredRight = value.Right;

                if (0 != data.DesiredRight.LengthSquared())
                {
                    data.DesiredRight.Normalize();
                }
            }
        }

        public virtual Vector3 DesiredPosition
        {
            set
            {
                data.DesiredPosition = value;
            }
        }

        /// <summary>
        /// Sets the desired orientation.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <param name="right"></param>
        public virtual void SetDesiredOrientation(Vector3 forward, Vector3 up, Vector3 right)
        {
            data.DesiredForward = forward;

            if (0 != data.DesiredForward.LengthSquared())
            {
                data.DesiredForward.Normalize();
            }

            data.DesiredUp = up;

            if (0 != data.DesiredUp.LengthSquared())
            {
                data.DesiredUp.Normalize();
            }

            data.DesiredRight = right;
            if (0 != data.DesiredRight.LengthSquared())
            {
                data.DesiredRight.Normalize();
            }
        }

        /// <summary>
        /// Gets/Sets the damping constant for linear mode.
        /// </summary>
        public float LinearKd
        {
            get { return data.LinearKd; }
            set
            {
                data.LinearKd = value;
            }
        }

        /// <summary>
        /// Gets/Sets the spring constant for linear mode.
        /// </summary>
        public float LinearKs
        {
            get { return data.LinearKs; }
            set
            {
                data.LinearKs = value;
            }
        }

        /// <summary>
        /// Gets/Sets the damping constant for angular mode.
        /// </summary>
        public float AngularKd
        {
            get { return data.AngularKd; }
            set
            {
                data.AngularKd = value;
            }
        }

        /// <summary>
        /// Gets/Sets the spring constant for angular mode.
        /// </summary>
        public float AngularKs
        {
            get { return data.AngularKs; }
            set
            {
                data.AngularKs = value;
            }
        }

        protected internal override void InternalUpdate()
        {
            // Note: this only applies to global position and orientation.

            if (data.Enabled && data.Solid != null)
            {
                if (data.Mode == SpringMotorMode.Linear || data.Mode == SpringMotorMode.LinearAndAngular)
                {
                    Vector3 error = data.DesiredPosition - GlobalAttachPoint;
                    Force f = new Force();
                    f.SingleStep = true;
                    f.Type = ForceType.GlobalForceAtLocalPosition;
                    f.Position = data.AttachOffset;
                    f.Direction = data.LinearKs * error - data.LinearKd * data.Solid.GlobalLinearVel;

                    // Scale the force vector by the Solid's mass.
                    f.Direction *= data.Solid.Mass;
                    data.Solid.AddForce(f);
                }

                if (data.Mode == SpringMotorMode.Angular || data.Mode == SpringMotorMode.LinearAndAngular)
                {
                    // Find cross products of actual and desired forward, up, 
                    // and right vectors; these represent the orientation error.
                    Matrix transform = data.Solid.Transform;
                    Vector3 actualForward = transform.Forward;
                    Vector3 actualUp = transform.Up;
                    Vector3 actualRight = transform.Right;

                    if (0 != actualForward.LengthSquared())
                    {
                        actualForward.Normalize();
                    }
                    if (0 != actualUp.LengthSquared())
                    {
                        actualUp.Normalize();
                    }
                    if (0 != actualRight.LengthSquared())
                    {
                        actualRight.Normalize();
                    }

                    Vector3 forwardError = Vector3.Cross(data.DesiredForward, actualForward);
                    Vector3 upError = Vector3.Cross(data.DesiredUp, actualUp);
                    Vector3 rightError = Vector3.Cross(data.DesiredRight, actualRight);

                    if (0 != forwardError.LengthSquared())
                    {
                        forwardError.Normalize();
                    }
                    if (0 != upError.LengthSquared())
                    {
                        upError.Normalize();
                    }
                    if (0 != rightError.LengthSquared())
                    {
                        rightError.Normalize();
                    }

                    // Scale error vectors by the magnitude of the angles.
                    float fangle = MathUtil.AngleBetweenPreNorm(data.DesiredForward, actualForward);
                    float uangle = MathUtil.AngleBetweenPreNorm(data.DesiredUp, actualUp);
                    float rangle = MathUtil.AngleBetweenPreNorm(data.DesiredRight, actualRight);

                    forwardError *= -fangle;
                    upError *= -uangle;
                    rightError *= -rangle;

                    // Average the vectors into one.
                    Vector3 errorAxis = (forwardError + upError + rightError) * Globals.OneThird;

                    // Use the error vector to calculate torque.
                    Force f = new Force();
                    f.SingleStep = true;
                    f.Type = ForceType.GlobalTorque;
                    f.Direction = data.AngularKs * errorAxis - data.AngularKd * data.Solid.GlobalAngularVel;

                    // Scale the torque vector by the Solid's intertia tensor.
                    f.Direction = Vector3.TransformNormal(f.Direction, data.Solid.InertiaTensor);
                    data.Solid.AddForce(f);
                }
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
