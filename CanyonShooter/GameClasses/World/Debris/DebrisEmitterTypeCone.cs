using System;
using System.Diagnostics;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Debris
{
    /// <summary>
    /// emits from one point in any direction that lies inside a given cone
    /// </summary>
    public class DebrisEmitterTypeCone : Transformable, IDebrisEmitterType
    {
        private Random random = new Random();

        private Vector3 direction;
        private Vector3 perpendicularDirection;
        private float angleAsRadiants;
        private float minVelocity;
        private float maxVelocity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="direction"></param>
        /// <param name="angle">half opening angle in degrees (180 -> sphere)</param>
        /// <param name="minVelocity"></param>
        /// <param name="maxVelocity"></param>
        public DebrisEmitterTypeCone(ICanyonShooterGame game, Vector3 direction, float angle, float minVelocity, float maxVelocity)
            : base(game)
        {
            Debug.Assert(!float.IsNaN(direction.X));

            this.direction = direction;
            this.angleAsRadiants = MathHelper.ToRadians(angle);
            this.minVelocity = minVelocity;
            this.maxVelocity = maxVelocity;

            // compute a vector which is perpendicular to direction
            // find a direction which is NOT nearly colinear to direction (otherwise cross product will be too small)
            if(Math.Abs(Vector3.Dot(direction, new Vector3(1,0,0))) < 0.9f)
            {
                // use vector 1,0,0
                perpendicularDirection = Vector3.Cross(direction, new Vector3(1, 0, 0));
            }
            else
            {
                // use vector 0,1,0
                perpendicularDirection = Vector3.Cross(direction, new Vector3(0, 1, 0));
            }

            direction.Normalize();
            perpendicularDirection.Normalize();
        }

        #region IDebrisEmitterType Members

        public void Apply(ITransformable obj)
        {
            float perpendicularAngle = (float)random.NextDouble() * angleAsRadiants * 2 - angleAsRadiants;
            float parallelAngle = (float)(random.NextDouble() * Math.PI);
            Quaternion r1 = Quaternion.CreateFromAxisAngle(perpendicularDirection, perpendicularAngle);
            Quaternion r2 = Quaternion.CreateFromAxisAngle(direction, parallelAngle);
            Quaternion r = Quaternion.Concatenate(r1, r2);

            Vector3 finalDirection = Vector3.Transform(direction, Quaternion.Concatenate(r, GlobalRotation));

            float velocity = minVelocity + (maxVelocity - minVelocity)*(float)random.NextDouble();

            obj.LocalPosition = GlobalPosition;
            obj.Velocity = velocity * finalDirection;
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
