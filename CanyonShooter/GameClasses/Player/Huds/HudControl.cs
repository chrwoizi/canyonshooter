using System;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace CanyonShooter.GameClasses.Huds
{
    /// <summary>
    /// Base Class for Hud elements like text, image, 3D object...
    /// </summary>
    class HudControl : IHudControl
    {
        protected MethodInfo getMethodInfo;
        protected object propertyObject;
        protected string formattingString;
        protected bool fetchData = false;

        #region HudControl Member

        protected bool propVisible;
        protected string propName;

        protected Vector2 propPos;

        private float TimeToLive;
        private float propTimeLiving;
        GameTime CreateTime;

        // Scrolling Options
        private Anchor propAnchor;
        protected Vector2 vecAnchor;
        private Boolean Scrolling = false;
        private Vector2 PositionFrom;
        private Vector2 PositionTo;

        protected float propScaling;
        protected float rotation;
        protected byte alpha;
        protected float propEffectPulseDuration;
        protected float propEffectParam2;

        protected Vector2 propResolution;

        private HUDEffectType effect;

        protected ICanyonShooterGame Game;
        #endregion

        #region Propertys

        public Vector2 Resolution
        {
            set
            {
                propResolution = value;
            }
        }


        public Anchor Anchor
        {
            get
            {
                return propAnchor;
            }
            set
            {
                propAnchor = value;
            }
        }

        public string Name
        {
            get
            {
                return this.propName;
            }
            set
            {
                this.propName = value;
            }
        }
        public bool Visible
        {
            get
            {
                return this.propVisible;
            }
            set
            {
                this.propVisible = value;
            }
        }
        public float EffectPulseDuration
        {
            get
            {
                return propEffectPulseDuration;
            }
            set
            {
                propEffectPulseDuration = value;
            }
        }
        public float EffectParam2
        {
            get
            {
                return propEffectParam2;
            }
            set
            {
                propEffectParam2 = value;
            }
        }
        public float Scaling
        {
            get
            {
                return propScaling;
            }
            set
            {
                propScaling = value;
                UpdateAnchor();
            }
        }

        public Vector2 Position
        {
            get
            {
                return propPos;
            }
            set
            {
                propPos = value;
            }
        }

        public HUDEffectType Effect
        {
            get { return effect; }
            set { effect = value;}
        }
        public float TimeLiving
        {
            get { return propTimeLiving;  }
            set { propTimeLiving = value; }
        }

        #endregion

        #region IHudControl

        protected HudControl(string name, HUDEffectType effect, string propertyName, object propertyObject, string formatting)
            : this (name, effect)
        {
            // Prepare with reflection
            Type type = propertyObject.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            this.getMethodInfo = propertyInfo.GetGetMethod();

            this.propertyObject = propertyObject;
            this.formattingString = formatting;

            fetchData = true;
        }

        protected HudControl(string name, GameTime gameTime, Vector2 pos, float timeToLive, Anchor anchor, HUDEffectType effect, string propertyName, object propertyObject, string formatting)
            : this(name, gameTime, pos, pos, timeToLive, anchor, effect, propertyName, propertyObject, formatting)
        {
        }

        protected HudControl(string name, GameTime gameTime, Vector2 from, Vector2 to, float timeToLive, Anchor anchor, HUDEffectType effect, string propertyName, object propertyObject, string formatting)
            : this(name, effect, propertyName, propertyObject, formatting)
        {
            this.CreateTime = gameTime;

            // Scrolling informations
            this.propPos = from;
            this.PositionFrom = from;
            this.PositionTo = to;
            this.Scrolling = (this.PositionTo == PositionFrom) ? false : true;

            // Timing Informations
            this.TimeToLive = timeToLive;

            this.Anchor = anchor;
        }

        protected HudControl(string name, HUDEffectType effect)
        {
            this.Name = name;
            this.effect = effect;
            this.Visible = true;
            this.propTimeLiving = 0;
            this.Scaling = 1.0f;
            this.alpha = 255;

            this.EffectPulseDuration = 0.5f;
            this.rotation = 0;
        }

        protected HudControl(string name, GameTime gameTime, Vector2 pos, float timeToLive, Anchor anchor, HUDEffectType effect)
            : this(name, gameTime, pos, pos, timeToLive, anchor, effect)
        {
        }

        protected HudControl(string name, GameTime gameTime, Vector2 from, Vector2 to, float timeToLive, Anchor anchor, HUDEffectType effect)
            : this(name, effect)
        {
            this.CreateTime = gameTime;

            // Scrolling informations
            this.propPos = from;
            this.PositionFrom = from;
            this.PositionTo = to;
            this.Scrolling = (this.PositionTo == PositionFrom) ? false : true;

            // Timing Informations
            this.TimeToLive = timeToLive;

            this.Anchor = anchor;
        }

        /// <summary>
        /// Update Anchor, overwrite funktion in derived classes to offer anchor functionality
        /// </summary>
        protected virtual void UpdateAnchor()
        {
        }

        /// <summary>
        /// Update position, visibility, scaling, rotation
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            this.propTimeLiving += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Nur skalieren, wenn zeitlich beschränkt
            if (TimeToLive > 0)
            {
                if ((this.effect & (HUDEffectType.GROW | HUDEffectType.SHRINK)) == (HUDEffectType.GROW | HUDEffectType.SHRINK))
                {
                    Scaling = (propTimeLiving * 2 <= TimeToLive) ? (0.8f * (propTimeLiving / TimeToLive)) + 0.6f : 0.6f + (0.8f * (1 - (propTimeLiving / TimeToLive)));
                }
                else
                {
                    if ((this.effect & HUDEffectType.GROW) > 0)
                        Scaling = (0.4f * (propTimeLiving / TimeToLive)) + 0.6f;
                    else if ((this.effect & HUDEffectType.SHRINK) > 0)
                        Scaling = 1.0f - (0.4f * (propTimeLiving / TimeToLive));
                }
            }

            // Nur ein-/ausfaden wenn zeitlich beschränkt
            if (TimeToLive > 0)
            {
                if ((this.effect & (HUDEffectType.FADE_IN | HUDEffectType.FADE_OUT)) == (HUDEffectType.FADE_IN | HUDEffectType.FADE_OUT))
                {
                    alpha = (propTimeLiving * 2 > TimeToLive) ? (byte)(255 * (1 - (propTimeLiving * 2 / TimeToLive))) : (byte)(255 * ((propTimeLiving / TimeToLive) - TimeToLive * 0.5f));
                }
                else
                {
                    if ((this.effect & HUDEffectType.FADE_OUT) > 0)
                        alpha = (byte)(255 * (1 - (propTimeLiving / TimeToLive)));
                    if ((this.effect & HUDEffectType.FADE_IN) > 0)
                        alpha = (byte)(255 * (propTimeLiving / TimeToLive));
                }
            }

            if ((this.effect & HUDEffectType.PULSE) > 0)
                Scaling = (0.2f * (((propTimeLiving % EffectPulseDuration) <= EffectPulseDuration * 0.5f) ? (propTimeLiving % EffectPulseDuration) * 2 : (EffectPulseDuration - (propTimeLiving % EffectPulseDuration)) * 2)) + 0.8f;

            if ((this.effect & HUDEffectType.SHACKE) > 0)
                rotation = ((propTimeLiving % EffectPulseDuration)*2 <= EffectPulseDuration)
                    ? 0.1f - 0.4f * (    (propTimeLiving % EffectPulseDuration) / EffectPulseDuration)
                    : 0.1f - 0.4f * (1 - (propTimeLiving % EffectPulseDuration) / EffectPulseDuration);

            if (TimeToLive != 0)
            {
                if (this.propTimeLiving > this.TimeToLive)
                    this.Visible = false;
                else
                    if (this.Scrolling)
                        this.Position = this.PositionFrom + ((this.PositionTo - this.PositionFrom) * this.propTimeLiving / this.TimeToLive);
            }
        }

        #endregion
    }

    #region Anchor/HUDEffectType
    
    public enum Anchor
    {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        LEFT,
        CENTER,
        RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT
    }

    [FlagsAttribute]
    public enum HUDEffectType : short
    {
        NONE = 0x00,
        PULSE = 0x01,
        SHACKE = 0x02,
        GROW = 0x04,
        SHRINK = 0x08,
        FADE_IN = 0x10,
        FADE_OUT = 0x20
    }

    #endregion

}
