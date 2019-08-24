namespace XnaDevRu.Physics
{
    /// <summary>
    /// A data structure describing an AttractorMotor.
    /// </summary>
    public class AttractorMotorData : MotorData
    {
        #region fields
        //Pointer to Solid 0.
        private Solid solid0;

        //Pointer to Solid 1.
        private Solid solid1;

        //Constant used to scale attraction (repulsion if strength < 0).  
        private float strength;

        //Determines exponential relationship of attraction/repulsion.  
        private float exponent; 
        #endregion

        #region Properties

        /// <summary>
        /// Pointer to Solid 0.
        /// </summary>
        public Solid Solid0 {
            get {
                return solid0;
            }
            set {
                solid0 = value;
            }
        }

        /// <summary>
        /// Pointer to Solid 1.
        /// </summary>
        public Solid Solid1 {
            get {
                return solid1;
            }
            set {
                solid1 = value;
            }
        }

        /// <summary>
        /// Constant used to scale attraction (repulsion if strength < 0).
        /// </summary>
        public float Strength {
            get {
                return strength;
            }
            set {
                strength = value;
            }
        }

        /// <summary>
        /// Determines exponential relationship of attraction/repulsion.
        /// </summary>
        public float Exponent {
            get {
                return exponent;
            }
            set {
                exponent = value;
            }
        }

        #endregion

        public AttractorMotorData()
        {
            type = MotorType.Attractor;
            Strength = Defaults.Motor.Attractor.Strength;
            Exponent = Defaults.Motor.Attractor.Exponent;
        }

    }
}
