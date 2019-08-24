namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a GearedMotor.
    /// </summary>
    public class GearedMotorData : MotorData
    {
        #region fields
        // Pointer to the Joint affected by this Motor.
        private Joint joint;

        // The index of the specific Joint axis affected by this Motor.
        private int jointAxisNum;

        // The maximum amount of torque that can be applied to the Joint 
        // axis.
        private float maxTorque;

        // The maximum angular velocity that can be achieved by the Motor.
        private float maxVelocity;

        // The throttle (i.e. the "gas") affects how much of the available 
        // torque is applied to the Joint axis.
        private float throttle;

        #endregion

        #region Properties

        /// <summary>
        /// Pointer to the Joint affected by this Motor.
        /// </summary>
        public Joint Joint {
            get {
                return joint;
            }
            set {
                joint = value;
            }
        }

        /// <summary>
        /// The index of the specific Joint axis affected by this Motor.
        /// </summary>
        public int JointAxisNum {
            get {
                return jointAxisNum;
            }
            set {
                jointAxisNum = value;
            }
        }

        /// <summary>
        /// The maximum amount of torque that can be applied to the Joint 
        /// axis.
        /// </summary>
        public float MaxTorque {
            get {
                return maxTorque;
            }
            set {
                maxTorque = value;
            }
        }

        /// <summary>
        /// The maximum angular velocity that can be achieved by the Motor.
        /// </summary>
        public float MaxVelocity {
            get {
                return maxVelocity;
            }
            set {
                maxVelocity = value;
            }
        }

        /// <summary>
        /// The throttle (i.e. the "gas") affects how much of the available 
        /// torque is applied to the Joint axis.
        /// </summary>
        public float Throttle {
            get {
                return throttle;
            }
            set {
                throttle = value;
            }
        }

        #endregion

        public GearedMotorData()
        {
            type = MotorType.Geared;
            JointAxisNum = 0;
            MaxTorque = Defaults.Motor.Geared.MaxTorque;
            MaxVelocity = Defaults.Motor.Geared.MaxVelocity;
            Throttle = 0;
        }
    }
}
