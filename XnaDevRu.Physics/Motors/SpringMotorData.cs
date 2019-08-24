using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing a SpringMotor.
    /// </summary>
    public class SpringMotorData : MotorData
    {
        #region fields
        /// Pointer to the Solid affected by this Motor.  
        private Solid solid;

        /// The Motor's mode of operation.  
        private SpringMotorMode mode;

        /// Local offset (relative to the Solid's position) that defines 
        /// where the SpringMotor attaches to the Solid.  This is only 
        /// used in linear mode.
        private Vector3 attachOffset;

        /// The Motor's desired position.  This is only used if the 
        /// appropriate mode is set.
        private Vector3 desiredPos;

        /// The Motor's desired forward direction, part of the desired 
        /// orientation.  This is only used if the appropriate mode is set.
        private Vector3 desiredForward;

        /// The Motor's desired up direction, part of the desired 
        /// orientation.  This is only used if the appropriate mode is set.
        private Vector3 desiredUp;

        /// The Motor's desired right direction, part of the desired 
        /// orientation.  This is only used if the appropriate mode is set.
        private Vector3 desiredRight;

        /// The damping constant for linear mode.  
        private float linearKd;

        /// The spring constant for linear mode.  
        private float linearKs;

        /// The damping constant for angular mode.  
        private float angularKd;

        /// The spring constant for angular mode.  
        private float angularKs; 

        #endregion

        #region Properties

        /// <summary>
        /// Pointer to the Solid affected by this Motor.  
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
        /// The Motor's mode of operation.  
        /// </summary>
        public SpringMotorMode Mode {
            get {
                return mode;
            }
            set {
                mode = value;
            }
        }

        /// <summary>
        /// Local offset (relative to the Solid's position) that defines 
        /// where the SpringMotor attaches to the Solid.  This is only 
        /// used in linear mode.
        /// </summary>
        public Vector3 AttachOffset {
            get {
                return attachOffset;
            }
            set {
                attachOffset = value;
            }
        }

        /// <summary>
        /// The Motor's desired forward direction, part of the desired 
        /// orientation.  This is only used if the appropriate mode is set.
        /// </summary>
        public Vector3 DesiredPosition {
            get {
                return desiredPos;
            }
            set {
                desiredPos = value;
            }
        }

        /// <summary>
        /// The Motor's desired forward direction, part of the desired 
        /// orientation.  This is only used if the appropriate mode is set.
        /// </summary>
        public Vector3 DesiredForward {
            get {
                return desiredForward;
            }
            set {
                desiredForward = value;
            }
        }

        /// <summary>
        /// The Motor's desired up direction, part of the desired 
        /// orientation.  This is only used if the appropriate mode is set.
        /// </summary>
        public Vector3 DesiredUp {
            get {
                return desiredUp;
            }
            set {
                desiredUp = value;
            }
        }

        /// <summary>
        /// The Motor's desired right direction, part of the desired 
        /// orientation.  This is only used if the appropriate mode is set.
        /// </summary>
        public Vector3 DesiredRight {
            get {
                return desiredRight;
            }
            set {
                desiredRight = value;
            }
        }

        /// <summary>
        /// The damping constant for linear mode.  
        /// </summary>
        public float LinearKd {
            get {
                return linearKd;
            }
            set {
                linearKd = value;
            }
        }

        /// <summary>
        /// The spring constant for linear mode.  
        /// </summary>
        public float LinearKs {
            get {
                return linearKs;
            }
            set {
                linearKs = value;
            }
        }

        /// <summary>
        /// The damping constant for angular mode.  
        /// </summary>
        public float AngularKd {
            get {
                return angularKd;
            }
            set {
                angularKd = value;
            }
        }

        /// <summary>
        /// The spring constant for angular mode.  
        /// </summary>
        public float AngularKs {
            get {
                return angularKs;
            }
            set {
                angularKs = value;
            }
        }

        #endregion


        public SpringMotorData()
        {
            type = MotorType.Spring;
            Mode = SpringMotorMode.Linear;
            // "attachOffset" is initialized in its own constructor.
            // "desiredPos" is initialized in its own constructor.
            DesiredForward = Defaults.Motor.Spring.DesiredForward;
            DesiredUp = Defaults.Motor.Spring.DesiredUp;
            DesiredRight = Defaults.Motor.Spring.DesiredRight;
            LinearKd = Defaults.Motor.Spring.LinearKd;
            LinearKs = Defaults.Motor.Spring.LinearKs;
            AngularKd = Defaults.Motor.Spring.AngularKd;
            AngularKs = Defaults.Motor.Spring.AngularKs;
        }
    }

    /// <summary>
    /// The different SpringMotor modes of operation.  
    /// </summary>
    public enum SpringMotorMode
    {
        /// <summary>
        /// Makes the Motor work to achieve a desired position.  
        /// </summary>
        Linear,

        /// <summary>
        /// Makes the Motor work to achieve a desired orientation.  
        /// </summary>
        Angular,

        /// <summary>
        /// Makes the Motor work to achieve a desired position and orientation.
        /// </summary>
        LinearAndAngular
    };
}
