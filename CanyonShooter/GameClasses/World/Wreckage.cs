using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter.GameClasses.World
{
    public class Wreckage : GameObject
    {
        private TimeSpan splitProbability = TimeSpan.FromSeconds(100);
        private TimeSpan spawnTime = TimeSpan.FromTicks(0);
        private Random random = new Random();
        private bool split = true;

        public Wreckage(ICanyonShooterGame game) 
            : base(game, "wreckage")
        {
            ConnectedToXpa = true;
            InfluencedByGravity = true;
        }

        public TimeSpan ExplodeProbability
        {
            get
            {
                return splitProbability;
            }
            set
            {
                splitProbability = value;
                spawnTime = TimeSpan.FromTicks(0);
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            if(spawnTime.Ticks == 0)
            {
                spawnTime = gameTime.TotalGameTime;
            }

            TimeSpan difference = gameTime.TotalGameTime - spawnTime;

            double p = difference.TotalSeconds/splitProbability.TotalSeconds;
            double r = random.NextDouble();

            if(r < p)
            {
                Explode();
            }
        }
    }
}
