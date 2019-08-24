using System;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// A class containing all default parameter values used in XPA.
    /// </summary>
    public static class Defaults
    {
        //Static readonly fields with integral types(such as int, double, bool, etc) should replaced with const

        /// <summary>
        /// 0.0167f ~ 60 Hz physics update rate.
        /// </summary>
        private static float stepSize = 0.0167f;
        private static Vector3 gravity = new Vector3(0, 0, 0);
        private static float bounceThreshold = 1.0f;
        private static SolverAccuracyLevel solverAccuracy = SolverAccuracyLevel.Medium;
        private static bool staticSleepingContactsEnabled = false;
        private static float maxCorrectingVel = 40.0f;
        private static int maxContacts = 24;

        public static float StepSize {
            get { return stepSize; }
            set { stepSize = value; }
        }

        public static Vector3 Gravity {
            get { return gravity; }
            set { gravity = value; }
        }

        public static float BounceThreshold {
            get { return bounceThreshold; }
            set { bounceThreshold = value; }
        }

        public static SolverAccuracyLevel SolverAccuracy {
            get { return solverAccuracy; }
            set { solverAccuracy = value; }
        }

        public static bool StaticSleepingContactsEnabled {
            get { return staticSleepingContactsEnabled; }
            set { staticSleepingContactsEnabled = value; }
        }

        public static float MaxCorrectingVel {
            get { return maxCorrectingVel; }
            set { maxCorrectingVel = value; }
        }

        public static int MaxContacts {
            get { return maxContacts; }
            set { maxContacts = value; }
        }

        private static long contactGroupFlags = 0xFFFFFFFF;

        /// <summary>
        /// All groups make contacts with all other groups by default.
        /// </summary>
        public static long ContactGroupFlags {
            get { return contactGroupFlags; }
            set { contactGroupFlags = value; }
        }



        /// <summary>
        /// Default parameters used in Sensor creation.
        /// </summary>
        public static class Sensor
        {
            private static bool enabled = true;

            public static bool Enabled {
                get { return enabled; }
                set { enabled = value; }
            }

            /// <summary>
            /// Default parameters used in InclineSensor creation.
            /// </summary>
            public static class Incline
            {
                private static Vector3 axis = new Vector3(1, 0, 0);

                public static Vector3 Axis {
                    get { return axis; }
                    set { axis = value; }
                }
            }
        }

        /// <summary>
        /// Default parameters specific to the ODE physics engine.
        /// </summary>
        public static class Ode
        {
            // ODE-specific defaults
            private static float autoDisableLinearMin = 0;
            private static float autoDisableLinearMax = 0.2f;
            private static float autoDisableAngularMin = 0;
            private static float autoDisableAngularMax = 0.2f;
            private static int autoDisableStepsMin = 1;
            private static int autoDisableStepsMax = 60;
            private static float autoDisableTimeMin = 0;
            private static float autoDisableTimeMax = 0.4f;
            private static float minMassRatio = 0.001f;
            private static float minERP = 0.1f;
            private static float maxERP = 0.9f;
            private static float globalCFM = 1e-5f;
            private static float jointFudgeFactor = 0.1f;
            private static float maxFriction = 1000.0f;
            private static float surfaceLayer = 0.001f;
            private static int maxRaycastContacts = 10;

            public static float AutoDisableLinearMin {
                get { return autoDisableLinearMin; }
                set { autoDisableLinearMin = value; }
            }

            public static float AutoDisableLinearMax {
                get { return autoDisableLinearMax; }
                set { autoDisableLinearMax = value; }
            }

            public static float AutoDisableAngularMin {
                get { return autoDisableAngularMin; }
                set { autoDisableAngularMin = value; }
            }

            public static float AutoDisableAngularMax {
                get { return autoDisableAngularMax; }
                set { autoDisableAngularMax = value; }
            }

            public static int AutoDisableStepsMin {
                get { return autoDisableStepsMin; }
                set { autoDisableStepsMin = value; }
            }

            public static int AutoDisableStepsMax {
                get { return autoDisableStepsMax; }
                set { autoDisableStepsMax = value; }
            }

            public static float AutoDisableTimeMin {
                get { return autoDisableTimeMin; }
                set { autoDisableTimeMin = value; }
            }

            public static float AutoDisableTimeMax {
                get { return autoDisableTimeMax; }
                set { autoDisableTimeMax = value; }
            }

            public static float MinMassRatio {
                get { return minMassRatio; }
                set { minMassRatio = value; }
            }

            //note: max and min mass ratios must be the inverse of each other
            //const real maxMassRatio=(real)1000.0;

            public static float MinERP {
                get { return minERP; }
                set { minERP = value; }
            }

            public static float MaxERP {
                get { return maxERP; }
                set { maxERP = value; }
            }

            public static float GlobalCFM {
                get { return globalCFM; }
                set { globalCFM = value; }
            }

            public static float JointFudgeFactor {
                get { return jointFudgeFactor; }
                set { jointFudgeFactor = value; }
            }

            public static float MaxFriction {
                get { return maxFriction; }
                set { maxFriction = value; }
            }

            public static float SurfaceLayer {
                get { return surfaceLayer; }
                set { surfaceLayer = value; }
            }

            public static int MaxRaycastContacts {
                get { return maxRaycastContacts; }
                set { maxRaycastContacts = value; }
            }
        }

        /// <summary>
        /// Default parameters used in Shape creation.
        /// </summary>
        public static class Shape
        {
            private static float[] planeABCD = new float[4] { 0, 1, 0, 0 };
            private static float capsuleLength = 1;
            private static float capsuleRadius = 1;
            private static float sphereRadius = 1;
            private static Vector3 boxDimensions = new Vector3(1, 1, 1);
            private static int contactGroup = 0;
            private static Material material = Material.WoodMaterial;

            public static Material Material {
                get { return material; }
                set { material = value; }
            }

            public static int ContactGroup {
                get { return contactGroup; }
                set { contactGroup = value; }
            }

            public static Vector3 BoxDimensions {
                get { return boxDimensions; }
                set { boxDimensions = value; }
            }

            public static float SphereRadius {
                get { return sphereRadius; }
                set { sphereRadius = value; }
            }

            public static float CapsuleRadius {
                get { return capsuleRadius; }
                set { capsuleRadius = value; }
            }

            public static float CapsuleLength {
                get { return capsuleLength; }
                set { capsuleLength = value; }
            }

            public static float[] PlaneABCD {
                get { return planeABCD; }
                set { planeABCD = value; }
            }
        }

        /// <summary>
        /// Default parameters used in Motor creation.
        /// </summary>
        public static class Motor
        {
            private static bool enabled = true;

            public static bool Enabled {
                get { return enabled; }
                set { enabled = value; }
            }

            /// <summary>
            /// Default parameters used in AttractorMotor creation.
            /// </summary>
            public static class Attractor
            {
                private static float strength = 1.0f;
                private static float exponent = 1.0f;


                public static float Strength {
                    get { return strength; }
                    set { strength = value; }
                }

                public static float Exponent {
                    get { return exponent; }
                    set { exponent = value; }
                }
            }

            /// <summary>
            /// Default parameters used in GearedMotor creation.
            /// </summary>
            public static class Geared
            {
                private static float maxTorque = 10.0f;
                private static float maxVelocity = 10.0f;

                public static float MaxVelocity {
                    get { return maxVelocity; }
                    set { maxVelocity = value; }
                }

                public static float MaxTorque {
                    get { return maxTorque; }
                    set { maxTorque = value; }
                }
            }

            /// <summary>
            /// Default parameters used in ServoMotor creation.
            /// </summary>
            public static class Servo
            {
                private static float maxTorque = 10.0f;
                private static float restoreSpeed = 1.0f;

                public static float RestoreSpeed {
                    get { return restoreSpeed; }
                    set { restoreSpeed = value; }
                }

                public static float MaxTorque {
                    get { return maxTorque; }
                    set { maxTorque = value; }
                }
            }

            /// <summary>
            /// Default parameters used in SpringMotor creation.
            /// </summary>
            public static class Spring
            {
                private static float linearKs = 1.0f;
                private static float linearKd = 0.1f;
                private static float angularKd = 0.1f;
                private static float angularKs = 1.0f;
                private static Vector3 desiredForward = new Vector3(0, 0, -1);
                private static Vector3 desiredUp = new Vector3(0, 1, 0);
                private static Vector3 desiredRight = new Vector3(1, 0, 0);

                public static float LinearKd {
                    get { return linearKd; }
                    set { linearKd = value; }
                }

                public static float LinearKs {
                    get { return linearKs; }
                    set { linearKs = value; }
                }

                public static float AngularKd {
                    get { return angularKd; }
                    set { angularKd = value; }
                }

                public static float AngularKs {
                    get { return angularKs; }
                    set { angularKs = value; }
                }

                public static Vector3 DesiredForward {
                    get { return desiredForward; }
                    set { desiredForward = value; }
                }

                public static Vector3 DesiredUp {
                    get { return desiredUp; }
                    set { desiredUp = value; }
                }

                public static Vector3 DesiredRight {
                    get { return desiredRight; }
                    set { desiredRight = value; }
                }
            }
        }

        /// <summary>
        /// Default parameters used in Joint creation.
        /// </summary>
        public static class Joint
        {
            private static JointType type = JointType.Hinge;
            private static bool enabled = true;
            private static float lowLimit = 0;
            private static float highLimit = 0;
            private static float limitHardness = 0.5f;
            private static float limitBounciness = 0.5f;
            private static Vector3 axis0Direction = new Vector3(1, 0, 0);
            private static Vector3 axis1Direction = new Vector3(0, 1, 0);
            private static Vector3 axis2Direction = new Vector3(0, 0, 1);
            private static bool limitsEnabled = false;
            private static Vector3 anchor = new Vector3(0, 0, 0);
            private static JointBreakMode breakMode = JointBreakMode.Unbreakable;
            private static float breakThresh = 100.0f;
            private static float accumThresh = 1000.0f;
            private static bool contactsEnabled = false;

            public static JointType Type {
                get { return type; }
                set { type = value; }
            }

            public static bool Enabled {
                get { return enabled; }
                set { enabled = value; }
            }

            public static float LowLimit {
                get { return lowLimit; }
                set { lowLimit = value; }
            }

            public static float HighLimit {
                get { return highLimit; }
                set { highLimit = value; }
            }

            public static float LimitHardness {
                get { return limitHardness; }
                set { limitHardness = value; }
            }

            public static float LimitBounciness {
                get { return limitBounciness; }
                set { limitBounciness = value; }
            }

            public static Vector3 Axis0Direction {
                get { return axis0Direction; }
                set { axis0Direction = value; }
            }

            public static Vector3 Axis1Direction {
                get { return axis1Direction; }
                set { axis1Direction = value; }
            }

            public static Vector3 Axis2Direction {
                get { return axis2Direction; }
                set { axis2Direction = value; }
            }

            public static bool LimitsEnabled {
                get { return limitsEnabled; }
                set { limitsEnabled = value; }
            }


            public static Vector3 Anchor {
                get { return anchor; }
                set { anchor = value; }
            }

            public static JointBreakMode BreakMode {
                get { return breakMode; }
                set { breakMode = value; }
            }

            public static float BreakThresh {
                get { return breakThresh; }
                set { breakThresh = value; }
            }

            public static float AccumThresh {
                get { return accumThresh; }
                set { accumThresh = value; }
            }

            public static bool ContactsEnabled {
                get { return contactsEnabled; }
                set { contactsEnabled = value; }
            }
        }

        /// <summary>
        /// Default parameters used in Solid creation.
        /// </summary>
        public static class Solid
        {
            private static bool enabled = true;
            private static bool sleeping = true;
            private static float sleepiness = 0.5f;
            private static bool isStatic = false;
            private static float linearDamping = 0.15f;
            private static float angularDamping = 0.15f;

            //TODO: Make xml documentation

            public static bool Enabled {
                get { return enabled; }
                set { enabled = value; }
            }

            public static bool Sleeping {
                get { return sleeping; }
                set { sleeping = value; }
            }

            public static float Sleepiness {
                get { return sleepiness; }
                set { sleepiness = value; }
            }

            public static bool IsStatic {
                get { return isStatic; }
                set { isStatic = value; }
            }

            public static float LinearDamping {
                get { return linearDamping; }
                set { linearDamping = value; }
            }

            public static float AngularDamping {
                get { return angularDamping; }
                set { angularDamping = value; }
            }
        }
    }

    /// <summary>
    /// A class containing globally useful parameters.
    /// </summary>
    public static class Globals
    {

        //Static readonly fields is slowly than const
        public const float PI = (float)Math.PI;
        public const float HalfPI = PI / 2;
        public const float Epsilon = 0.000001f;
        public const float OneThird = 0.33333333333333333333f;

        /// <summary>
        /// The highest value that can be used for the Simulator's
        /// max contacts parameter = 128.
        /// </summary>
        public const uint MaxMaxContacts = 128;
    }

    /// <summary>
    /// Solver accuracy levels determine how the physics engine constraint
    /// solver is used (e.g. the number of iterations in an iterative solver).
    /// </summary>
    public enum SolverAccuracyLevel
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    };
}
