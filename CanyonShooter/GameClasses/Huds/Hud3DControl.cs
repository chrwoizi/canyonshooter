using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.Huds
{
    class Hud3DControl : HudControl, IHud3DControl
    {
        string model;
        #region IHudTextControl Members

        public Hud3DControl(string name, GameTime gameTime, String model, Vector2 from, Vector2 to, float timeToLive, Anchor anchor, HUDEffectType effect)
            : base(name, gameTime, from, to, timeToLive, anchor, effect)
        {
            this.model = model;
        }

        public void Draw(SpriteBatch sb)
        {
            throw new Exception("Not Implemented");
        }

        public void Draw()
        {
            throw new Exception("Not Implemented");
        }

        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion
    }
}
