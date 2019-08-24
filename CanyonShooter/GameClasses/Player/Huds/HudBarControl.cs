using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.GameClasses.Huds;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Huds
{
    class HudBarControl : HudControl, IHudBarControl
    {
        private Rectangle propMinRect = new Rectangle(0, 0, 0, 0);
        private Rectangle propMaxRect = new Rectangle(0, 0, 100, 100);

        private string propText;
        private int propValue;

        private HudTextControl tc;
        private HudSpriteControl sc;

        #region IHudBarControl Member

        public string Text
        {
            get
            {
                return propText;
            }
            set
            {
                propText = value;
            }
        }

        public int Value
        {
            get
            {
                return propValue;
            }
            set
            {
                if (value > 100)
                    propValue = 100;
                else if (value < 0)
                    propValue = 0;
                else
                    propValue = value;
                sc.Rect = new Rectangle((int)((float)propMinRect.X + ((float)propMaxRect.X - (float)propMinRect.X) * (float)value / 100.0f),
                                        (int)((float)propMinRect.Y + ((float)propMaxRect.Y - (float)propMinRect.Y) * (float)value / 100.0f),
                                        (int)((float)propMinRect.Width + ((float)propMaxRect.Width - (float)propMinRect.Width) * (float)value / 100.0f),
                                        (int)((float)propMinRect.Height + ((float)propMaxRect.Height - (float)propMinRect.Height) * (float)value / 100.0f));
            }
        }

        /// <summary>
        /// Maximal Visible Rect. Values capped between 0 and 100
        /// </summary>
        public Rectangle MinRect
        {
            get
            {
                return propMinRect;
            }
            set
            {
                propMinRect = new Rectangle(Math.Max(Math.Min(value.X, 100), 0),
                                            Math.Max(Math.Min(value.Y, 100), 0),
                                            Math.Max(Math.Min(value.Width, 100), 0),
                                            Math.Max(Math.Min(value.Height, 100), 0));
            }
        }

        /// <summary>
        /// Maximal Visible Rect. Values capped between 0 and 100
        /// </summary>
        public Rectangle MaxRect
        {
            get
            {
                return propMaxRect;
            }
            set
            {
                propMaxRect = new Rectangle(Math.Max(Math.Min(value.X, 100), 0),
                                            Math.Max(Math.Min(value.Y, 100), 0),
                                            Math.Max(Math.Min(value.Width, 100), 0),
                                            Math.Max(Math.Min(value.Height, 100), 0));
            }
        }

        #endregion

        public new Vector2 Resolution
        {
            set
            {
                propResolution = value;
                if (tc != null)
                    tc.Resolution = value;
                if (sc != null)
                    sc.Resolution = value;
            }
        }

        public HudBarControl(string name, GameTime gameTime, ICanyonShooterGame game, SpriteFont font, String background, Vector2 pos, Vector2 size, float timeToLive, Anchor anchor, HUDEffectType effect, string propertyName, object propertyObject, string formatting)
            : base(name, gameTime, pos, pos, timeToLive, anchor, effect, propertyName, propertyObject, formatting)
        {
            tc = new HudTextControl(name + "_text", gameTime, font, new Color(), pos, timeToLive, anchor, effect, propertyName, propertyObject, formatting);
            sc = new HudSpriteControl(name + "_sprite", gameTime, game, size, background, pos, timeToLive, anchor, effect);
            Value = 100;
        }

        public HudBarControl(string name, GameTime gameTime, ICanyonShooterGame game, String text, SpriteFont font, String background, Vector2 pos, Vector2 size, float timeToLive, Anchor anchor, HUDEffectType effect)
            : base(name, gameTime, pos, pos, timeToLive, anchor, effect)
        {
            tc = new HudTextControl(name + "_text", gameTime, text, font, new Color(), pos, timeToLive, anchor, effect);
            sc = new HudSpriteControl(name + "_sprite", gameTime, game, size, background, pos, timeToLive, anchor, effect);
            Value = 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (fetchData)
            {
                object methodResult = getMethodInfo.Invoke(propertyObject, new object[] { });
                if (methodResult.GetType() == typeof(int))
                    Value = (int)methodResult;
                else if (methodResult.GetType() == typeof(float))
                    Value = (int)(float)methodResult;
                else if (methodResult.GetType() == typeof(bool))
                    Value = ((bool)methodResult)?100:0;
            }
            tc.Update(gameTime);
            sc.Update(gameTime);
        }

        public void Draw(SpriteBatch sb, SpriteFont font)
        {
            sc.Draw(sb);
            tc.Draw(sb);
        }

    }
}
