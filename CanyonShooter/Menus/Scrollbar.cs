#region Using Statements
using System;
using System.Collections.Generic;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace CanyonShooter.Menus
{
    /// <summary>
    /// Diese Klasse soll zeigen, dass die Highscore-Liste in der Komponente Highscore gespeichert werden 
    /// soll und durch die Klasse HighscoreMenu angeteigt wird.
    /// </summary>
    public class Scrollbar : DrawableGameComponent
    {

        #region DataMembers
        private ICanyonShooterGame game;

        private ScrollBarDescription scXml;

        private int activeElement; //Das Element, dass aktiv ist
        private int factor;

        #region Textures
        private Texture2D scrollBar;
        private Bar bar;

        public Button scrollUp;
        public Button scrollDown;
        #endregion

        private Vector2 StartingPoint;
        private Rectangle Position;
        private int UnitWidth;

        private float distance; //die totale Größe der Einheit
        private float maxScrolling; //die Länge des Scollbalken
        private float minScrolling; // die größe eines Buttons
        private Vector2 minScrollingPosition; //zugehöriger Vektor zu dem int-Wert
        private Vector2 maxScrollingPosition; //zugehöriger Vektor zu dem int-Wert

        private Rectangle backBar;
        private bool backBarActive;

        #region ScrollButton
        private Vector2 ScButPos;
        private float ScButSize;
        private bool mouseMode = false;
        #endregion


        #region Properties
        public Vector2 ButtonPosition
        {
            get { return ScButPos; }
            set { ScButPos = value; }
        }

        public int Unitwidth
        {
            get { return UnitWidth; }
            set { UnitWidth = value; }
        }
        private int UnitHeight;

        public int Unitheight
        {
            get { return UnitHeight; }
            set { UnitHeight = value; }
        }

        public Bar Bar
        {
            get { return bar; }
            set { bar = value; }
        }

        public Rectangle BackBar
        {
            get { return backBar; }
            set { backBar = value; }
        }

        public bool BackBarActive
        {
            get { return backBarActive; }
            set { backBarActive = value; }
        }
        #endregion

        private float Contentdistance;// die totale Länge des tatsächlichen Contents - alle Buttons
        private float ContentBoxSize; // Größe aller sichtbaren Elemente

        #endregion

        public Scrollbar(ICanyonShooterGame game, Vector2 Position, int Unitheight, int Contentdistance, int ContentBoxSize, int MinumumScrolling, int activeElement, int factor)
            : base(game as Game)
        {
            this.game = game;
            this.activeElement = activeElement;
            this.factor = factor;
           
            StartingPoint = Position;
            this.UnitHeight = Unitheight;
            this.distance = Unitheight;
            

            loadScrollbar(Contentdistance, ContentBoxSize, MinumumScrolling);
            this.Position = calculateRectangle(Position, Unitheight, UnitWidth);           
        }

        public Scrollbar(ICanyonShooterGame game, Rectangle Position, int Contentdistance, int ContentBoxSize, int MinumumScrolling,int activeElement,int factor)
            : base(game as Game)
        {
            this.Position = Position;
            this.activeElement = activeElement;
            this.factor = factor;
            this.StartingPoint = new Vector2(Position.X, Position.Y);
            this.distance = -(Position.Top - Position.Bottom);
            

            loadScrollbar(Contentdistance, ContentBoxSize, MinumumScrolling);
        }

        private void loadScrollbar(int Contentdistance, int ContentBoxSize,int MinimumScrolling)
        {
            scXml = game.Content.Load<ScrollBarDescription>("Content\\Menu\\ScrollBarXML");
            
            this.Contentdistance = Contentdistance;
            this.ContentBoxSize = ContentBoxSize;
            this.UnitWidth = scXml.UnitWidth;

            foreach (ButtonDescription i in scXml.Buttons)
            {
                switch(i.ButtonName) {
                    case "ScrollUp":
                        scrollUp = new Button(game, i.ButtonName, new Rectangle((int)this.StartingPoint.X, (int)this.StartingPoint.Y, (int)i.width, (int)i.height), i.ButtonImage, false, this);
                        scrollUp.Buttonpressed += new ActionEventHandler(scrollUp_Buttonpressed);
                        break;
                    case "ScrollDown":
                        scrollDown = new Button(game, i.ButtonName, new Rectangle((int)this.StartingPoint.X, (int)(this.StartingPoint.Y + this.distance - i.height), (int)i.width, (int)i.height), i.ButtonImage, false, this);
                        scrollDown.Buttonpressed += new ActionEventHandler(scrollDown_Buttonpressed);
                        break;
                     default:
                        break;

                }
            }
            this.backBar = calculateRectangle(new Vector2(StartingPoint.X,
                           StartingPoint.Y + scrollUp.Height), (int)distance - 15 - scrollDown.Height, UnitWidth);

            for (int i = 0; i < 50; i++)
            {
                this.maxScrolling = distance - this.scrollUp.Height - this.scrollDown.Height - this.ScButSize;
                float delta = (float) ((float)MinimumScrolling /(float) Contentdistance);
                this.minScrolling =delta * maxScrolling;                
                this.ScButSize = calculateButtonSize(this.Contentdistance, maxScrolling, ContentBoxSize);
                this.minScrollingPosition = new Vector2(StartingPoint.X, StartingPoint.Y + scrollUp.Height);
                this.maxScrollingPosition = new Vector2(StartingPoint.X, StartingPoint.Y + this.maxScrolling);
            }
            this.ScButPos = minScrollingPosition + new Vector2(0,activeElement* minScrolling);
            this.bar = new Bar(game, scXml, (int)Math.Ceiling(ScButSize), ScButPos);
            this.bar.BarPress += new BarEventHandler(Bar_BarPress);
            this.bar.BarRelease += new BarEventHandler(Bar_BarRelease);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime,int currentNumber)
        {
            ScButPos = minScrollingPosition + new Vector2(0,currentNumber * minScrolling);
            bar.Update(gameTime,ScButPos);
            base.Update(gameTime);
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
                foreach (TextureDescription a in scXml.Textures)
                {
                    if (a.Name == "ScrollBar")
                    {
                        SpriteBatch.Draw(scrollBar, backBar, Color.White);                        
                    }
                }
                scrollUp.Draw(gameTime, SpriteBatch);
                scrollDown.Draw(gameTime, SpriteBatch);
                bar.Draw(gameTime, SpriteBatch);

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
            foreach (TextureDescription a in scXml.Textures)
            {
                if (a.Name == "ScrollBar")
                {  
                    scrollBar = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\scroll");
                }
            }          
            scrollUp.LoadContent(game);
            scrollDown.LoadContent(game);
            bar.LoadContent();
        }

        private float calculateButtonSize(float realContentsize, float ScrollBarSize, float ContentBoxSize) 
        {
            float relation = ContentBoxSize / realContentsize;
            return relation * ScrollBarSize;
        }

        //Teil 1 des Abgleich von Scrollbar und ButtonListe
        public int checkPosition()
        {
            if (mouseMode)
            {
                Vector2 Position = new Vector2(bar.CurrentPosition.X, game.Input.MousePosition.Y);
                bar.CurrentPosition = Position;
                Vector2 far = Position - minScrollingPosition;
                double value = (far.Y / minScrolling);
                double lowerBound = Math.Floor(far.Y / minScrolling);
                double upperBound = Math.Ceiling(far.Y / minScrolling);
                double lowerDif = value - lowerBound;
                double upperDif = upperBound - value;
                if (lowerDif<=upperDif)
                {
                    return (int) lowerBound;
                }
                else if (upperDif<lowerDif)
                {
                    return (int) upperBound;
                }
            }
            else
            {
                Vector2 far = bar.CurrentPosition - minScrollingPosition;
                int number = (int)(far.Y / minScrolling);             
            }
            return 99999;
        }
        #region EventHandler
        public void scrollDown_Buttonpressed()
        {
            this.ScButPos += new Vector2(0, minScrolling);
            bar.Update(ScButPos);
            //checkPosition(true);
        }

        public void scrollUp_Buttonpressed()
        {
            this.ScButPos -= new Vector2(0, minScrolling);
            bar.Update(ScButPos);
            //checkPosition(true);
        }

        public void Bar_BarRelease()
        {
            this.mouseMode = false;
            //checkPosition(true);
        }

        public void Bar_BarPress()
        {
            this.mouseMode = true;
            //checkPosition(true);
        }

        public void BackBarPressed(Vector2 MousePosition)
        {
            if (MousePosition.Y < bar.Position.Top)
            {
                this.ScButPos -= new Vector2(0, factor*minScrolling);
                bar.Update(ScButPos);
            }
            else if (MousePosition.Y > bar.Position.Bottom)
            {
                this.ScButPos += new Vector2(0, factor*minScrolling);
                bar.Update(ScButPos);
            }
        }
        #endregion

        private Rectangle calculateRectangle(Vector2 pos, int height, int width)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }

    }
}
