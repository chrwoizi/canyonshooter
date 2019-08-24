namespace XnaDevRu.Physics
{
    public class MotorData
    {
        #region fields

        /// True if the Motor is enabled.
        private bool enabled;

        /// An identifier for the Motor.  
        private string name;

        /// The Motor type.
        protected MotorType type; 

        #endregion

        #region Properties

        /// <summary>
        /// True if the Motor is enabled.
        /// </summary>
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                enabled = value;
            }
        }

        /// <summary>
        /// An identifier for the Motor.  
        /// </summary>
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        #endregion

        public MotorData()
        {
            // The initial type doesn't matter since the abstract base 
            // class will never be instantiated.
            type = MotorType.Attractor;
            Enabled = Defaults.Motor.Enabled;
            Name = "";

            
        }


        /// <summary>
        /// Returns the Motor's type.
        /// </summary>
        public virtual MotorType Type
        {
            get { return type; }
        }

        public override string ToString()
        {
            string disable = Enabled ? " Enabled" : " Disabled";
            return "MotorData: " + Name + " " + this.Type + disable;
        }

    }
}
