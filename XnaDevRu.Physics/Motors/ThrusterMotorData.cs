namespace XnaDevRu.Physics
{
    /// <summary>
    ///  A data structure describing a ThrusterMotor.
    /// </summary>
    public class ThrusterMotorData : MotorData
    {
        #region fields
        /// Pointer to the Solid.
        private Solid solid;

        /// The Force that gets applied to the Solid every time step.  
        public Force Force; //Structs cannot be properties
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


        //Structs cannot be properties

        // /// <summary>
        // /// The Force that gets applied to the Solid every time step.  
        // /// </summary>
        //public Force Force {
        //    get {
        //        return force;
        //    }
        //    set {
        //        force = value;
        //    }
        //}

        #endregion


        public ThrusterMotorData() {
            type = MotorType.Thruster;

            Force = new Force();
            Force.SingleStep = true;
        }
    }
}
