using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses
{
    public class Finish : Static
    {
        private ICanyonShooterGame game;

        public Finish(ICanyonShooterGame game)
            : base(game, "Final")
        {
            this.game = game;
            SetModel("Final");
            LocalScale = new Vector3(10, 10, 10);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
