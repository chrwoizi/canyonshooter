using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{

    /// <summary>
    /// A data structure describing a Joint.
    /// </summary>
    public class JointData
    {

        #region fields

        
        private bool enabled;

        /// <summary>
        /// Determines whether the Joint is enabled.
        /// </summary>
        public bool Enabled {
            get { return enabled; }
            set { enabled = value; }
        }

        
        private string name;

        /// <summary>
        /// An identifier for the Joint.
        /// </summary>
        public string Name {
            get { return name; }
            set { name = value; }
        }

        
        private Solid solid0;

        /// <summary>
        /// Pointer to the Joint's Solid0.
        /// </summary>
        public Solid Solid0 {
            get { return solid0; }
            set { solid0 = value; }
        }

        
        private Solid solid1;

        /// <summary>
        /// Pointer to the Joint's Solid1.
        /// </summary>
        public Solid Solid1 {
            get { return solid1; }
            set { solid1 = value; }
        }

        /// <summary>
        /// The anchor point for this Joint.  This is not used for every
        /// Joint type.
        /// </summary>
        public Vector3 Anchor;

        
        private JointAxis[] axis;

        /// <summary>
        /// The axes that describe the Joint's degrees of freedom.  Not all
        /// axes are used for every Joint type.
        /// </summary>
        public JointAxis[] Axis {
            get { return axis; }
            set { axis = value; }
        }

        
        private JointBreakMode breakMode;

        /// <summary>
        /// Determines how damage is handled.
        /// </summary>
        public JointBreakMode BreakMode {
            get { return breakMode; }
            set { breakMode = value; }
        }

        
        private float breakThresh;

        /// <summary>
        /// Joint breaks if force/torque exceeds this value.
        /// </summary>
        public float BreakThresh {
            get { return breakThresh; }
            set { breakThresh = value; }
        }

        
        private float accumThresh;

        /// <summary>
        /// Force/torque exceeding this adds to accumulated damage.
        /// </summary>
        public float AccumThresh {
            get { return accumThresh; }
            set { accumThresh = value; }
        }

        
        private float accumDamage;

        /// <summary>
        /// The amount of accumulated damage.
        /// </summary>
        public float AccumDamage {
            get { return accumDamage; }
            set { accumDamage = value; }
        }

        
        private bool contactsEnabled;

        /// <summary>
        /// Determines whether Solids connected by this Joint should make
        /// contacts when they collide.  If multiple Joints connect the same
        /// two Solids, each with different contactsEnabled settings,
        /// the behavior is undefined.
        /// </summary>
        public bool ContactsEnabled {
            get { return contactsEnabled; }
            set { contactsEnabled = value; }
        }

        
        private bool isBroken;

        /// <summary>
        /// If true, the joint is broken.
        /// note the default is false - joint is not broken
        /// </summary>
        public bool IsBroken {
            get { return isBroken; }
            set { isBroken = value; }
        }

        /// The Joint type.
        protected JointType type;

        #endregion

        public JointData()
        {
            type = Defaults.Joint.Type;
            Enabled = Defaults.Joint.Enabled;
            Name = "";
            Anchor = Defaults.Joint.Anchor;
            Axis = new JointAxis[3];
			for (int i = 0; i < 3; i++)
				Axis[i] = new JointAxis();
            Axis[0].Direction = Defaults.Joint.Axis0Direction;
            Axis[1].Direction = Defaults.Joint.Axis1Direction;
            Axis[2].Direction = Defaults.Joint.Axis2Direction;
            // The rest of the data from the axes are initialized in the
            // JointAxis constructor.
            BreakMode = Defaults.Joint.BreakMode;
            BreakThresh = Defaults.Joint.BreakThresh;
            AccumThresh = Defaults.Joint.AccumThresh;
            AccumDamage = 0;
            ContactsEnabled = Defaults.Joint.ContactsEnabled;
            IsBroken = false;
        }


        /// <summary>
        /// Gets/Sets the Joint's type.
        /// </summary>
        public virtual JointType Type
        {
            get { return type; }
            set { type = value; }
        }

        public override string ToString()
        {
            string disable = Enabled ? " Enabled" : " Disabled";
            return "JointData: " + Name + " " + this.Type + disable;
        }


    }

    /// <summary>
    /// A data structure describing the limits for a single Joint axis.
    /// </summary>
    public class JointLimits
    {
        #region fields
        /// <summary>
        /// The limit angle or distance with the smaller value.
        /// </summary>
        public float Low;

        /// <summary>
        /// The limit angle or distance with the higher value.
        /// </summary>
        public float High;

        /// <summary>
        /// Determines how far a Joint can exceed its limits.  This must
        /// be between 0 and 1.
        /// </summary>
        public float Hardness;

        /// <summary>
        /// Bounciness (i.e. restitution) how much the Joint will bounce
        /// when it hits a limit.
        /// </summary>
        public float Bounciness; 
        #endregion

        public JointLimits()
        {
            Low = Defaults.Joint.LowLimit;
            High = Defaults.Joint.HighLimit;
            Hardness = Defaults.Joint.LimitHardness;
            Bounciness = Defaults.Joint.LimitBounciness;
        }
    }


    /// <summary>
    /// A data structure describing a single Joint axis.
    /// </summary>
    public class JointAxis
    {
        #region fields
        /// <summary>
        /// The direction vector of the axis.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Determines whether Joint limits are enabled for this axis.
        /// </summary>
        public bool LimitsEnabled;

        /// <summary>
        /// The limits of the Joint axis.
        /// </summary>
        public JointLimits Limits = new JointLimits(); 
        #endregion

        public JointAxis()
        {
            Direction = Defaults.Joint.Axis0Direction;
            // "limits" is initialized in its own constructor.
            LimitsEnabled = Defaults.Joint.LimitsEnabled;
        }
    }
}
