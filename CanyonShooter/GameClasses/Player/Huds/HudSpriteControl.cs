using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.Huds
{
    class HudSpriteControl : HudControl, IHudSpriteControl
    {
        #region IHudSpriteControl Members

        Texture2D Texture;
        Vector2 Size;
        Rectangle propRect;

        public Rectangle Rect
        {
            set
            {
                propRect = new Rectangle(value.X, value.Y, (int) (((float)value.Right / 100.0f)*(float)Texture.Width), (int) ((float)Texture.Height * (float)value.Bottom / 100.0f));
            }
        }

        #endregion

        #region HudControl

        public HudSpriteControl(string name, GameTime gameTime, ICanyonShooterGame game, Vector2 size, String texture, Vector2 pos, float timeToLive, Anchor anchor, HUDEffectType effect)
            : this(name, gameTime, game, size, texture, pos, pos, timeToLive, anchor, effect)
        {
        }

        public HudSpriteControl(string name, GameTime gameTime, ICanyonShooterGame game, Vector2 size, String texture, Vector2 from, Vector2 to, float timeToLive, Anchor anchor, HUDEffectType effect)
            : base(name, gameTime, from, to, timeToLive, anchor, effect)
        {
            this.Game = game;
            this.Size = size;
            this.Texture = game.Content.Load<Texture2D>(texture);
            this.UpdateAnchor();
            Rect = new Rectangle(0, 0, 100, 100);
        }

        public void Draw(SpriteBatch sb)
        {
            if (Visible)
                sb.Draw(this.Texture,
                        Vector2.Multiply(propResolution,propPos),
                        propRect,
                        Color.White,
                        rotation,
                        vecAnchor,
                        /*Scaling */Vector2.Divide(Vector2.Multiply(Size, propResolution), new Vector2(Texture.Width, Texture.Height)),
                        SpriteEffects.None,
                        0);
        }

        protected override void UpdateAnchor()
        {
            switch (Anchor)
            {
                case Anchor.TOP_LEFT:
                    vecAnchor = new Vector2(0, 0);
                    break;
                case Anchor.TOP_CENTER:
                    vecAnchor = new Vector2(this.Texture.Height / 2, 0);
                    break;
                case Anchor.TOP_RIGHT:
                    vecAnchor = new Vector2(this.Texture.Height, 0);
                    break;
                case Anchor.LEFT:
                    vecAnchor = new Vector2(0, this.Texture.Height / 2);
                    break;
                case Anchor.CENTER:
                    vecAnchor = new Vector2(this.Texture.Height / 2, this.Texture.Height / 2);
                    break;
                case Anchor.RIGHT:
                    vecAnchor = new Vector2(this.Texture.Height, this.Texture.Height / 2);
                    break;
                case Anchor.BOTTOM_LEFT:
                    vecAnchor = new Vector2(0, this.Texture.Height);
                    break;
                case Anchor.BOTTOM_CENTER:
                    vecAnchor = new Vector2(this.Texture.Height / 2, this.Texture.Height);
                    break;
                case Anchor.BOTTOM_RIGHT:
                    vecAnchor = new Vector2(this.Texture.Height, this.Texture.Height);
                    break;
            }
        }


        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion
    }
}
