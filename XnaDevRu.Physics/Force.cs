using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{

    /// <summary>
    /// Data structure containing all necessary info for applying a
    /// force/torque to a Solid.
    /// </summary>
    public struct Force
    {
        #region fields

        /// <summary>
        /// Specifies the type of force.
        /// </summary>
        private ForceType type;

        /// <summary>
        /// Specifies how long to apply to force.  This makes it easy to
        /// apply forces independent of the step size.  This will be ignored
        /// if "singleStep" is true.
        /// </summary>
        private float duration;

        /// <summary>
        /// This is mainly used internally by OPAL.  It specifies that
        /// the force will be applied across a single time step.  If this
        /// parameter is true, "duration" will be ignored.
        /// </summary>
        private bool singleStep;

        /// <summary>
        /// The force direction or torque axis.  This parameter encodes the
        /// magnitude of the force or torque.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Only used when the force is applied at an offset (i.e. not
        /// the center of mass).
        /// </summary>
        public Vector3 Position;

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the type of force.
        /// </summary>
        public ForceType Type {
            get {
                return type;
            }
            set {
                type = value;
            }
        }

        /// <summary>
        /// Specifies how long to apply to force.  This makes it easy to
        /// apply forces independent of the step size.  This will be ignored
        /// if "singleStep" is true.
        /// </summary>
        public float Duration {
            get {
                return duration;
            }
            set {
                duration = value;
            }
        }

        /// <summary>
        /// This is mainly used internally by OPAL.  It specifies that
        /// the force will be applied across a single time step.  If this
        /// parameter is true, "duration" will be ignored.
        /// </summary>
        public bool SingleStep {
            get {
                return singleStep;
            }
            set {
                singleStep = value;
            }
        }

        #endregion

        public override string ToString() {
            return "Force: " + this.Type + " duration:" + Duration;
        }
    }

    /// <summary>
    /// Types of Forces.  Used when creating a Force struct to designate
    /// how the Forces should be applied to a Solid.
    /// </summary>
    public enum ForceType
    {
        /// <summary>
        /// Apply a force in a direction relative to the Solid's local
        /// coordinate system.
        /// </summary>
        LocalForce,

        /// <summary>
        /// Apply a force in a direction relative to the global
        /// coordinate system.
        /// </summary>
        GlobalForce,

        /// <summary>
        /// Apply a torque with the axis specified relative to the Solid's
        /// local coordinate system.
        /// </summary>
        LocalTorque,

        /// <summary>
        /// Apply a torque with the axis specified relative to global
        /// coordinate system.
        /// </summary>
        GlobalTorque,

        /// <summary>
        /// Apply a force at a position relative to the Solid's local
        /// coordinate system in a direction relative to the Solid's local
        /// coordinate system.
        /// </summary>
        LocalForceAtLocalPosition,

        /// <summary>
        /// Apply a force at a position relative to the global
        /// coordinate system in a direction relative to the Solid's local
        /// coordinate system.
        /// </summary>
        LocalForceAtGlobalPosition,

        /// <summary>
        /// Apply a force at a position relative to the Solid's local
        /// coordinate system in a direction relative to the global
        /// coordinate system.
        /// </summary>
        GlobalForceAtLocalPosition,

        /// <summary>
        /// Apply a force at a position relative to the global
        /// coordinate system in a direction relative to the global
        /// coordinate system.
        /// </summary>
        GlobalForceAtGlobalPosition
    };
}
