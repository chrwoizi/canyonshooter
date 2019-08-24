using System;
using Microsoft.Xna.Framework;

namespace XnaDevRu.Physics
{
    public class AttractorMotor : Motor
    {
        #region fields
        // Stores data describing the Motor.
        protected AttractorMotorData data;

        // Cached copy of Solid 0's mass.
        protected float solid0Mass;

        // Cached copy of Solid 1's mass.
        protected float solid1Mass;

        // Cached copy of strength * m0 * m1.   
        protected float massConstant; 
        #endregion

        public AttractorMotor()
        {
        }

        /// <summary>
        /// Initializes the Motor with the given data structure.  If the 
        /// Solid pointers in the data are NULL, the Motor will do nothing.
        /// </summary>
        /// <param name="data"></param>
        public void Init(AttractorMotorData data)
        {
            base.Init();
            this.data = data;

            if (data.Solid0 != null && data.Solid1 != null)
            {
                solid0Mass = data.Solid0.Mass;
                solid1Mass = data.Solid1.Mass;

                // Update this constant since the masses changed.
                massConstant = data.Strength * solid0Mass * solid1Mass;
            }
        }

        /// <summary>
        /// Gets/Sets the strength parameter.
        /// </summary>
        public virtual float Strength
        {
            get { return this.data.Strength; }
            set 
            {
                data.Strength = value;

                // Update this constant since the strength changed.
                massConstant = data.Strength * solid0Mass * solid1Mass;
            }
        }

        /// <summary>
        /// Gets/Sets the exponent parameter.
        /// </summary>
        public virtual float Exponent
        {
            get { return data.Exponent; }
            set { data.Exponent = value; }
        }

        public override bool Enabled
        {
            get
            {
                return data.Enabled;
            }
            set
            {
                data.Enabled = value;
            }
        }

        public override MotorType Type
        {
            get { return data.Type; }
        }

        public override string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        protected internal override void InternalUpdate()
        {
            if (!data.Enabled)
                return;

            if (data.Solid0 != null && data.Solid1 != null)
            {
                Vector3 pos1 = data.Solid0.Position;
                Vector3 pos2 = data.Solid1.Position;

                // Create a unit vector pointing from mSolid1 to mSolid0.
                Vector3 direction = pos1 - pos2;
                float distanceSquared = direction.LengthSquared();

                if (0 != distanceSquared)
                {
                    direction.Normalize();
                }

                Force f = new Force();
                f.SingleStep = true;
                f.Type = ForceType.GlobalForce;
                f.Direction = direction;

                // Use force magnitude = (strength * m0 * m1) / distance^exponent.
                if (2.0f == data.Exponent)
                {
                    // If we know the exponent is 2, this can speed things up.
                    f.Direction *= (massConstant / distanceSquared);
                }
                else
                {
                    f.Direction *= (massConstant / ((float)Math.Pow(direction.Length(), data.Exponent)));
                }

                data.Solid1.AddForce(f);
                f.Direction *= -1.0f;
                data.Solid0.AddForce(f);
            }
        }

        protected internal override bool InternalDependsOnSolid(Solid s)
        {
            if (s == data.Solid0 || s == data.Solid1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
