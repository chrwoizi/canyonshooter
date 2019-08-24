#region Using Statements
using System;
using System.Collections.Generic;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace CanyonShooter.Menus
{
    class HSlideBar : DrawableGameComponent
    {
        #region static DataMembers
        private static Vector2 ButtonOffset = new Vector2(5,0);
        private const int BarWidth = 130;
        private const int BarHeight = 17;
        private const int ButtonWidth = 11;
        private const int ButtonHeight = 13;
        private const int BarStopperWidth = 8;
        private const int BarStopperHeight = 6;
        private const string MinusImage = "butminus";
        //private const int MinusWidht = 30;
        //private const int MinusHeight = 30;
        private const string PlusImage = "butplus";
        //private const int PlusWidht = 30;
        //private const int PlusHeight = 30;
        private const int makerwidth = 11;
        private const int makerheight = 13;
        //private const float eps = 0.5f;
        #endregion
        #region DataMembers

        private int ButtonHeightForContent;

        private ICanyonShooterGame game;
        private Button caller;

        private Vector2 StartPosition;
        private Vector2 BarPosition;
        
        private bool isActive = false;
        private bool visible = false;

        #region functionality
        private Vector2 minPushPosition;
        private Vector2 maxPushPosition;
        private float PushLenght;
        private int factor;

        private float minValue;
        private float maxValue;
        private float stepSize;

        private float currentValue;

        private Vector2 makerPosition;

        private Rectangle barRec;
        private Rectangle markerRec;

        #endregion
        #region Graphic
        private Texture2D bar;
        private Texture2D maker;
        #endregion

        public Button plus;
        public Button minus;

        private int cut;

        #region Properties
        public float CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
        }

        public Rectangle BarRec
        {
            get { return barRec; }
        }

        /*public Rectangle MarkerRec
        {
            get { return markerRec; }
        }*/
        #endregion

        #region delegates
        // ---------- Ereignis ---------- 
        public event BarEventHandler minuspressed;
        public event BarEventHandler pluspressed;
        //public event BarEventHandler pressingMarker;
        //public event BarEventHandler markerReleased;
        #endregion

        public bool Activity
        {
          get { return isActive; }
          set { isActive = value; }
        }

       

        public bool Visiblity
        {
            get { return visible; }
            set { visible = value; }
        }

        #endregion
        /// <summary>
        /// Constructor for ListBox
        /// </summary>
        /// <param name="game"></param>
        /// <param name="but"></param>
        /// <param name="pos"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="Startvalue"></param>
        /// <param name="stepSize"></param>
        /// <param name="faktor"></param>
        public HSlideBar(ICanyonShooterGame game, Button but, Vector2 pos, int Height, float minValue, float maxValue, float Startvalue,
            float stepSize, int faktor, int cut)
            : base(game as Game)
        {
            this.game = game;
            this.caller = but;
            this.StartPosition = pos;
            this.factor = faktor;
            this.currentValue = Startvalue;
            this.maxValue = maxValue;
            this.minValue = minValue;
            this.stepSize = stepSize;
            this.cut = cut;
            //this.ButtonHeightForContent = (int)Math.Ceiling((double)(but.Height - 8));
            
            this.minus = new Button(game, "Minus", calculateRectangle(new Vector2(StartPosition.X,
                StartPosition.Y + 15), Height, Height), MinusImage, true, this);
            minus.Buttonpressed += new ActionEventHandler(MinusPressed);
            this.BarPosition = new Vector2(minus.Position.Right, minus.Position.Top + (minus.Position.Height / 2) - 
                (BarHeight / 2)) + ButtonOffset;
            this.minPushPosition = this.BarPosition + new Vector2(6, 0);
            this.maxPushPosition = this.BarPosition + new Vector2(BarWidth - 6 - makerwidth, 0);
            this.PushLenght = maxPushPosition.X - minPushPosition.X;
            Vector2 pluspos = new Vector2(this.BarPosition.X + BarWidth, this.minus.Position.Y) + ButtonOffset;
            this.plus = new Button(game, "Plus", calculateRectangle(pluspos, Height, Height), PlusImage, true, this);
            plus.Buttonpressed += new ActionEventHandler(PlusPressed);
            calculatingMakerPos(Startvalue);
            float test = but.getPosition().Top;
            float test2 = but.getPosition().Bottom;
            barRec = calculateRectangle(new Vector2(this.BarPosition.X, BarPosition.Y - 10), 37, 
                (int)(BarPosition.X + (int)(BarWidth - 6 - makerwidth)) - (int)(BarPosition.X + 6) + 19);
        }

        // Konstuktor
        // Dieser Konstruktor kann von außen nicht erreicht werden.
        public HSlideBar(ICanyonShooterGame game, Button but,Vector2 pos,float minValue, float maxValue,float Startvalue, 
            float stepSize, int faktor, int cut)
            : base(game as Game)
        {            
            this.game = game;
            this.caller = but;
            this.StartPosition = pos;
            this.factor = faktor;
            this.currentValue = Startvalue;
            this.maxValue=maxValue;
            this.minValue = minValue;
            this.stepSize =stepSize;
            this.ButtonHeightForContent = (int)Math.Ceiling((double)(but.Height - 8));
            this.cut = cut;


            this.minus = new Button(game, "Minus", calculateRectangle(new Vector2(StartPosition.X, 
                StartPosition.Y + 4), ButtonHeightForContent,
                ButtonHeightForContent), MinusImage, true, this);
            minus.Buttonpressed += new ActionEventHandler(MinusPressed);

            this.BarPosition = new Vector2(minus.Position.Right, minus.Position.Top+(minus.Position.Height/2)-(BarHeight/2))+ ButtonOffset;
            this.minPushPosition = this.BarPosition+new Vector2(6,0);
            this.maxPushPosition = this.BarPosition+ new Vector2(BarWidth-6-makerwidth,0);
            this.PushLenght = maxPushPosition.X-minPushPosition.X;
            Vector2 pluspos = new Vector2(this.BarPosition.X+BarWidth,this.minus.Position.Y )+ButtonOffset;

            this.plus = new Button(game, "Plus", calculateRectangle(pluspos, ButtonHeightForContent,
                ButtonHeightForContent), PlusImage, true, this);
            plus.Buttonpressed += new ActionEventHandler(PlusPressed);

            calculatingMakerPos(Startvalue);
            barRec = calculateRectangle(new Vector2(this.BarPosition.X, BarPosition.Y - 5), 27, (int)(BarPosition.X+ (int)(BarWidth-6-makerwidth))- (int)(BarPosition.X+6) + 19); 
        }

        public void calculatingMakerPos(float currentValue) 
        {
            Vector2 PosDif = this.maxPushPosition - this.minPushPosition;
            float ValueDif = this.maxValue - this.minValue;
            if(currentValue>=this.minValue && currentValue<=this.maxValue) 
            {
                if (currentValue < 0)
                {
                    this.makerPosition = minPushPosition + new Vector2((float)Math.Ceiling((PosDif.X / maxValue) * (-currentValue)),
                    (float)Math.Ceiling((PosDif.Y / maxValue) * (-currentValue))+2);
                }
                else
                {
                    this.makerPosition = minPushPosition + new Vector2((float)Math.Ceiling((PosDif.X / maxValue) * currentValue),
                    (float)Math.Ceiling((PosDif.Y / maxValue) * currentValue) + 2);
                }
                //markerRec = calculateRectangle(new Vector2(makerPosition.X -3, makerPosition.Y - 3), makerheight + 6, makerwidth + 6);
            }
        }

        public void PlusPressed()
        {
            float value = currentValue + stepSize;
            if (value >= this.minValue && value <= this.maxValue)
            {
                value = (int)(value * this.cut);
                currentValue = value / cut;
                calculatingMakerPos(currentValue);
            }
            OptionList call = (OptionList)caller.caller;
            call.changed(this.caller.Name, true, false); 
        }

        public void MinusPressed()
        {
            float value = currentValue - stepSize;
            if (value >= this.minValue && value <= this.maxValue)
            {
                value = (int)(value * this.cut);
                currentValue = value / cut;
                calculatingMakerPos(currentValue);
            }
            OptionList call = (OptionList)caller.caller;
            call.changed(this.caller.Name, false, false); 
        }

        // Barpressed zu kleines Rec (ersma weg) und Barscroll setzen nich möglich 
          
        public void BarPressed(Vector2 Position)
        {
            if (Position.X < makerPosition.X)
            {
                float dif = (Position.X - makerPosition.X + makerwidth);
                float value = currentValue + (dif / PushLenght);
                if (value >= this.minValue && value <= this.maxValue)
                {
                    value = (int)(value * this.cut);
                    currentValue = value / cut;
                    calculatingMakerPos(currentValue);
                }
                OptionList call = (OptionList)caller.caller;
                call.changed(this.caller.Name, true, true); 
            }
            else if (Position.X > makerPosition.X + makerwidth)
            {
                float dif =(Position.X - makerPosition.X + makerwidth);
                float value = currentValue + (dif / PushLenght);
                if (value >= this.minValue && value <= this.maxValue)
                {
                    value = (int)(value * this.cut);
                    currentValue = value / cut;
                    calculatingMakerPos(currentValue);
                }
                OptionList call = (OptionList)caller.caller;
                call.changed(this.caller.Name, true,true); 
            }
        }
        /*
        public void BarScroll(int Position)
        {
            if (Position <= minPushPosition.X)
            {
                currentValue = minValue;
            }
            else if (Position >= maxPushPosition.X)
            {
                currentValue = maxValue;
            }
            else
            {
                currentValue = (Position - minPushPosition.X);
            }
        }*/

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        
        public override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime,SpriteBatch Spritebatch)
        {
            minus.Update(gameTime, Spritebatch);
            plus.Update(gameTime, Spritebatch);
            this.calculatingMakerPos(this.currentValue);

        }

        //
        // Zusammenfassung:
        //     Called when the DrawableGameComponent needs to be drawn.  Override this method
        //     with component-specific drawing code.
        //
        // Parameter:
        //   gameTime:
        //     Time passed since the last call to Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime).
        public void Draw(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            if (SpriteBatch != null)
            {
                SpriteBatch.Draw(bar,calculateRectangle(this.BarPosition,BarHeight,BarWidth),Color.White);
                SpriteBatch.Draw(maker, calculateRectangle(makerPosition, makerheight, makerwidth), Color.White);
                this.minus.Draw(gameTime, SpriteBatch);
                this.plus.Draw(gameTime, SpriteBatch);
            }
        }
        
        //
        // Summary:
        //     Called when the component needs to load graphics resources.  Override to
        //     load any component specific graphics resources.
        //
        // Parameters:
        //   loadAllContent:
        //     true if all graphics resources need to be loaded;false if only manual resources
        //     need to be loaded.
        public new void LoadContent()
        {
           this.bar = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\bar");
           this.maker = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\pushbutton");
           this.plus.LoadContent(game);
           this.minus.LoadContent(game);
        }



        private Rectangle calculateRectangle(Vector2 pos, int height, int width)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }
    }
}
