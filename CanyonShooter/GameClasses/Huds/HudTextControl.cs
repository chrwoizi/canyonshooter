using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.Huds
{
    class HudTextControl : HudControl, IHudTextControl
    {
        #region HudTextControl Members

        SpriteFont font;
        string propText;

        #endregion

        #region Properties

        public string Text
        {
            get
            {
                return propText;
            }
            set
            {
                propText = value;
                this.UpdateAnchor();
            }
        }

        #endregion

        #region HudControl

        /// <summary>
        /// UpdateAnchor-Method changes vecAnchor to adjust Anchor
        /// </summary>
        protected override void UpdateAnchor()
        {
            if (this.font != null)
            {
                Vector2 vec = font.MeasureString(this.Text);
                switch (this.Anchor)
                {
                    case Anchor.TOP_LEFT:
                        this.vecAnchor = new Vector2(0, 0);
                        break;
                    case Anchor.TOP_CENTER:
                        this.vecAnchor = new Vector2(vec.X / 2, 0);
                        break;
                    case Anchor.TOP_RIGHT:
                        this.vecAnchor = new Vector2(vec.X, 0);
                        break;
                    case Anchor.LEFT:
                        this.vecAnchor = new Vector2(0, vec.Y / 2);
                        break;
                    case Anchor.CENTER:
                        this.vecAnchor = new Vector2(vec.X / 2, vec.Y / 2);
                        break;
                    case Anchor.RIGHT:
                        this.vecAnchor = new Vector2(vec.X, vec.Y / 2);
                        break;
                    case Anchor.BOTTOM_LEFT:
                        this.vecAnchor = new Vector2(0, vec.Y);
                        break;
                    case Anchor.BOTTOM_CENTER:
                        this.vecAnchor = new Vector2(vec.X / 2, vec.Y);
                        break;
                    case Anchor.BOTTOM_RIGHT:
                        this.vecAnchor = new Vector2(vec.X, vec.Y);
                        break;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Unique identifier</param>
        /// <param name="gameTime">actual GameTime (containing timegap between last and current frame)</param>
        /// <param name="text">Text to render</param>
        /// <param name="font">SpriteFont-handle</param>
        /// <param name="pos">Position</param>
        /// <param name="timeToLive">time to display (in seconds), set 0 to render forever</param>
        /// <param name="anchor">Anchor</param>
        public HudTextControl(string name, GameTime gameTime, String text, SpriteFont font, Vector2 pos, float timeToLive, Anchor anchor, HUDEffectType effect)
            : this(name, gameTime, text, font, pos, pos, timeToLive, anchor, effect)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Unique identifier</param>
        /// <param name="gameTime">actual GameTime (containing timegap between last and current frame)</param>
        /// <param name="text">Text to render</param>
        /// <param name="font">SpriteFont-handle</param>
        /// <param name="from">Position to blend from</param>
        /// <param name="to">Position to blend to</param>
        /// <param name="timeToLive">time to display (in seconds), set 0 to render forever</param>
        /// <param name="anchor">Anchor</param>
        public HudTextControl(string name, GameTime gameTime, String text, SpriteFont font, Vector2 from, Vector2 to, float timeToLive, Anchor anchor, HUDEffectType effect)
            : base(name, gameTime, from, to, timeToLive, anchor, effect)
        {
            this.font = font;
            this.Text = text;
        }

        /// <summary>
        /// Draw Method called from Hud! DO NOT CALL THIS METHOD!
        /// </summary>
        /// <param name="sb">SpriteBatch for rendering</param>
        /// <param name="font">SpriteFont for rendering</param>
        public void Draw(SpriteBatch sb, SpriteFont font)
        {
            if (Visible)
                sb.DrawString(font, Text, Vector2.Multiply(propResolution, Position), new Color(128, 128, 128, alpha), rotation, vecAnchor, Scaling, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Update Method called from Hud. DO NOT CALL THIS METHOD!
        /// </summary>
        /// <param name="gameTime">actual GameTime (containing timegap between last frame and current frame)</param>
        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion
    }
}
