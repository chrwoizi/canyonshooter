using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a VelocityMotor.
    /// </summary>
    public class VelocityMotorData : MotorData
    {
        #region fields
        // Pointer to the Solid.
        private Solid solid;

        // Maximum force that can be used to achieve target velocity.
        private float maxForce;

        // if true, the solid will be affected by gravity
        private bool letGravityAffectSolid;

        // The velocity that is to be achieved every frame
        private Vector3 velocity;
        #endregion

        #region Properties

        /// <summary>
        /// Pointer to the Solid.
        /// </summary>
        public Solid Solid {
            get {
                return solid;
            }
            set {
                solid = value;
            }
        }

        /// <summary>
        /// Maximum force that can be used to achieve target velocity.
        /// </summary>
        public float MaxForce {
            get {
                return maxForce;
            }
            set {
                maxForce = value;
            }
        }

        /// <summary>
        /// if true, the solid will be affected by gravity
        /// </summary>
        public bool LetGravityAffectSolid {
            get {
                return letGravityAffectSolid;
            }
            set {
                letGravityAffectSolid = value;
            }
        }

        /// <summary>
        /// The velocity that is to be achieved every frame
        /// </summary>
        public Vector3 Velocity {
            get {
                return velocity;
            }
            set {
                velocity = value;
            }
        }

        #endregion

        public VelocityMotorData()
        {
            type = MotorType.Velocity;
            Velocity = new Vector3(0, 0, 0);
            MaxForce = 1;
            LetGravityAffectSolid = false;
        }

    }
}
