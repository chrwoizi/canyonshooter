using System;
using System.Collections.Generic;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Menus
{
    //Eventhändler
    public delegate void BarEventHandler();
    
    public class Bar
    {
        #region Data Members
        private ICanyonShooterGame game;
        private ScrollBarDescription scXml;

        private Vector2 currentPosition;
        private Rectangle position;

        private int BarWidth;
        #region Events
        // ---------- Ereignis ---------- 
        public event BarEventHandler BarPress;
        public event BarEventHandler BarRelease;
        #endregion

        #region Textures
        private Texture2D scrollButTop;
        private Texture2D scrollButMain;
        private Texture2D scrollButBottom;
        #endregion
        #region Messures
        private int TopWidth;
        private int TopHeight;
        private int MainWidth;
        private int MainHeight;
        private int BottomWidth;
        private int BottomHeight;

        private int relatedLength; //Gesamtlänge des Balkens
        #endregion
        #endregion

        #region Properties
        public Vector2 CurrentPosition
        {
            get { return currentPosition; }
            set { currentPosition = value; }
        }
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }
        #endregion

        public Bar(ICanyonShooterGame game, ScrollBarDescription scxml, int barlength, Vector2 Startposition)
        {
            this.game = game;
            this.scXml = scxml;
            this.relatedLength = barlength;
            this.currentPosition = Startposition;
            this.getMessures();
        }

        private void getMessures()
        {
            foreach (TextureDescription a in scXml.Textures)
            {
                switch (a.Name)
                {
                    case "ScrollButTop":
                        this.TopHeight = a.Height;
                        this.TopWidth = a.Width;
                        if (a.Width > BarWidth)
                        {
                            this.BarWidth = a.Width;
                        }
                        break;
                    case "ScrollButMain":
                        this.MainHeight = a.Height;
                        this.MainWidth = a.Width;
                        if (a.Width > BarWidth)
                        {
                            this.BarWidth = a.Width;
                        }
                        break;
                    case "ScrollButBottom":
                        this.BottomHeight = a.Height;
                        this.BottomWidth = a.Width;
                        if (a.Width > BarWidth)
                        {
                            this.BarWidth = a.Width;
                        }
                        break;
                }
            }  
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public void Initialize()
        {
            // TODO: Add your initialization code here
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, Vector2 Position)
        {
            this.currentPosition = Position;
            updatePosition();
        }

        public void Update(Vector2 Position)
        {
            this.currentPosition = Position;
            updatePosition();
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
                SpriteBatch.Draw(scrollButTop, new Rectangle((int)currentPosition.X,(int) currentPosition.Y,
                    this.TopWidth, this.TopHeight), Color.White);
                SpriteBatch.Draw(scrollButMain, new Rectangle((int)currentPosition.X,(int)( currentPosition.Y
                    + this.TopHeight), this.MainWidth, relatedLength - TopHeight - BottomHeight),
                    Color.White);
                SpriteBatch.Draw(scrollButBottom, new Rectangle((int)currentPosition.X,(int)( currentPosition.Y +
                    relatedLength - this.BottomHeight), this.BottomWidth, this.BottomHeight), Color.White);
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
        public void LoadContent()
        {
            foreach (TextureDescription a in scXml.Textures)
            {
                switch (a.Name)
                {
                    case "ScrollButTop":
                        scrollButTop = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\scbuttop");
                        break;
                    case "ScrollButMain":
                        scrollButMain = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\scbutmid");
                        break;
                    case "ScrollButBottom":
                        scrollButBottom = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\scbutdown");
                        break;

                }
            }   
        }

        private Rectangle calculateRectangle(Vector2 pos, int height, int width)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }

        public void updatePosition()
        {
            this.Position = calculateRectangle(this.currentPosition, relatedLength, BarWidth);
        }

    }
}
