using System;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Debris
{
    /// <summary>
    /// emits from one point in one distinct direction
    /// </summary>
    public class DebrisEmitterTypeDirectional : Transformable, IDebrisEmitterType
    {
        private Random random = new Random();

        private Vector3 direction;
        private float minVelocity;
        private float maxVelocity;

        public DebrisEmitterTypeDirectional(ICanyonShooterGame game, Vector3 direction, float minVelocity, float maxVelocity)
            : base(game)
        {
            this.direction = direction;
            this.minVelocity = minVelocity;
            this.maxVelocity = maxVelocity;

            direction.Normalize();
        }

        #region IDebrisEmitterType Members

        public void Apply(ITransformable obj)
        {
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
