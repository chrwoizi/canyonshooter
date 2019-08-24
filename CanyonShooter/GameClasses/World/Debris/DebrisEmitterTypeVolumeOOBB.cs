using System;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Debris
{
    /// <summary>
    /// emits from any position in an OOBB with no velocity
    /// </summary>
    public class DebrisEmitterTypeVolumeOOBB : Transformable, IDebrisEmitterType
    {
        private Random random = new Random();

        private Vector3 size;

        public DebrisEmitterTypeVolumeOOBB(ICanyonShooterGame game, Vector3 size)
            : base(game)
        {
            this.size = size;
        }

        #region IDebrisEmitterType Members

        public void Apply(ITransformable obj)
        {
            float dx = size.X * (float)random.NextDouble() - 0.5f * size.X;
            float dy = size.Y * (float)random.NextDouble() - 0.5f * size.Y;
            float dz = size.Z * (float)random.NextDouble() - 0.5f * size.Z;

            obj.LocalPosition = GlobalPosition + Vector3.Transform(new Vector3(dx, dy, dz), GlobalRotation);
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
