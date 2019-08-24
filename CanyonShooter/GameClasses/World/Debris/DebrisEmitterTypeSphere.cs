using System;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Debris
{
    /// <summary>
    /// emits from one point in all directions
    /// </summary>
    public class DebrisEmitterTypeSphere : Transformable, IDebrisEmitterType
    {
        private Random random = new Random();

        private float minVelocity;
        private float maxVelocity;

        public DebrisEmitterTypeSphere(ICanyonShooterGame game, float minVelocity, float maxVelocity)
            : base(game)
        {
            this.minVelocity = minVelocity;
            this.maxVelocity = maxVelocity;
        }

        #region IDebrisEmitterType Members

        public void Apply(ITransformable obj)
        {
            float x = (float)random.NextDouble() * 2 - 1.0f;
            float y = (float)random.NextDouble() * 2 - 1.0f;
            float z = (float)random.NextDouble() * 2 - 1.0f;
            Vector3 direction = new Vector3(x, y, z);
            direction.Normalize();

            float velocity = minVelocity + (maxVelocity - minVelocity)*(float)random.NextDouble();

            obj.LocalPosition = GlobalPosition;
            obj.Velocity = velocity * Vector3.Transform(direction, GlobalRotation);
        }

        public float MinVelocity
        {
            get
            {
                return minVelocity;
            }
            set
            {
                minVelocity = value;
            }
        }

        public float MaxVelocity
        {
            get
            {
                return maxVelocity;
            }
            set
            {
                maxVelocity = value;
            }
        }

        #endregion
    }
}
