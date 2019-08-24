namespace XnaDevRu.Physics
{
    public class ServoMotorData : MotorData
    {
        #region fields
        // Pointer to the Joint affected by this Motor.  
        private Joint joint;

        // The Motor's mode of operation.
        private ServoMotorMode mode;

        // The index of the Joint axes affected by this Motor.
        private int jointAxisNum;

        // The Motor's desired angle.  This is only used if the 
        // appropriate mode is set.
        private float desiredAngle;

        // The Motor's desired velocity.  This is only used if the 
        // appropriate mode is set.		
        private float desiredVel;

        // The maximum amount of torque that can be used to help the 
        // Joint axis achieve its desired angle or velocity.
        private float maxTorque;

        // A constant used in desired angle mode that scales how fast 
        // the Joint axis will achieve its desired angle.
        private float restoreSpeed; 
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
        /// The Motor's mode of operation.
        /// </summary>
        public ServoMotorMode Mode {
            get {
                return mode;
            }
            set {
                mode = value;
            }
        }

        /// <summary>
        /// The index of the Joint axes affected by this Motor.
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
        /// The Motor's desired angle.  This is only used if the 
        /// appropriate mode is set.
        /// </summary>
        public float DesiredAngle {
            get {
                return desiredAngle;
            }
            set {
                desiredAngle = value;
            }
        }

        /// <summary>
        /// The Motor's desired velocity.  This is only used if the 
        /// appropriate mode is set.	
        /// </summary>
        public float DesiredVelocity {
            get {
                return desiredVel;
            }
            set {
                desiredVel = value;
            }
        }

        /// <summary>
        /// The maximum amount of torque that can be used to help the 
        /// Joint axis achieve its desired angle or velocity.
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
        /// A constant used in desired angle mode that scales how fast 
        /// the Joint axis will achieve its desired angle.
        /// </summary>
        public float RestoreSpeed {
            get {
                return restoreSpeed;
            }
            set {
                restoreSpeed = value;
            }
        }

        #endregion

        public ServoMotorData()
		{
			type = MotorType.Servo;
			Mode = ServoMotorMode.DesiredAngleMode;
			MaxTorque = Defaults.Motor.Servo.MaxTorque;
			RestoreSpeed = Defaults.Motor.Servo.RestoreSpeed;
		}
    }

    /// <summary>
    /// The different ServoMotor modes of operation.  
    /// </summary>
    public enum ServoMotorMode
    {
        /// <summary>
        /// The ServoMotor tries to achieve a desired angle for the Joint axis.
        /// </summary>
        DesiredAngleMode,

        /// <summary>
        /// The ServoMotor tries to achieve a desired velocity for the Joint axis.
        /// </summary>
        DesiredVelMode
    };
}
