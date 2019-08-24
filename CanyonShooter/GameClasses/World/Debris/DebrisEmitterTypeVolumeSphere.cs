using System;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Debris
{
    /// <summary>
    /// emits from any position in a sphere with no velocity
    /// </summary>
    public class DebrisEmitterTypeVolumeSphere : Transformable, IDebrisEmitterType
    {
        private Random random = new Random();

        private float radius;

        public DebrisEmitterTypeVolumeSphere(ICanyonShooterGame game, float radius)
            : base(game)
        {
            this.radius = radius;
        }

        #region IDebrisEmitterType Members

        public void Apply(ITransformable obj)
        {
            float x = GlobalPosition.X + radius * (float)random.NextDouble() * 2 - radius;
            float y = GlobalPosition.Y + radius * (float)random.NextDouble() * 2 - radius;
            float z = GlobalPosition.Z + radius * (float)random.NextDouble() * 2 - radius;

            obj.LocalPosition = new Vector3(x, y, z);
        }

        public float MinVelocity
        {
            get
            {
                throw new Exception("Volume emitters don't set the velocity of the debris.");
            }
            set
            {
                throw new Exception("Volume emitters don't set the velocity of the debris.");
            }
        }

        public float MaxVelocity
        {
            get
            {
                throw new Exception("Volume emitters don't set the velocity of the debris.");
            }
            set
            {
                throw new Exception("Volume emitters don't set the velocity of the debris.");
            }
        }

        #endregion
    }
}
