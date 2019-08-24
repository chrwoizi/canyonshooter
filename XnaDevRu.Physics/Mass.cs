using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    /// <summary>
    /// Describes mass of the rigid body
    /// </summary>
    public class Mass
    {
        private float massa;

        /// <summary>
        /// Center of gravity position in body frame (x,y,z).
        /// Default is (0,0,0)
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// 3x3 inertia tensor in body frame, about POR
        /// Default is inertia of a box
        /// </summary>
        public Matrix Inertia;

        /// <summary>
        /// Total mass of the rigid body
        /// Default is 1
        /// </summary>
        public float MassValue {
            get { return massa; }
            set { massa = value; }
        }


        /// Default values are set
        public Mass()
        {
            MassValue = 1;
            Center = new Vector3(0, 0, 0);
            Inertia = Matrix.Identity;
            Inertia.M11 = 1 / 6.0f;
            Inertia.M22 = 1 / 6.0f;
            Inertia.M33 = 1 / 6.0f;
        }

        public override string ToString()
        {
            return "Mass: " + MassValue;
        }


    }
}
