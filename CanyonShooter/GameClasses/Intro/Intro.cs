using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DescriptionLibs.Intro;
using CanyonShooter.Engine.Audio;

namespace CanyonShooter.GameClasses.Intro
{
    /// <summary>
    /// Class Intro manages and starts the Intro
    /// </summary>
    class Intro : DrawableGameComponent
    {
        #region Data Member

        private bool refresh = false;
        private int maxlength;

        
        private SpriteBatch SpriteBatch;
        private SpriteFont font;
        private Texture2D back;

        private IntroDescription introtext;
        private float X;

        private ICanyonShooterGame game;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">game is the Canyonshootergame</param>
        public Intro(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
            this.Visible = true;
            introtext = game.Content.Load<IntroDescription>("Content\\Intro\\Intro");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            int test = 0;
            int test2 = 0;
            for (int i = 0; i < introtext.Zeile.Count; i++)
            {
                test2 = (int)font.MeasureString(introtext.Zeile[i].Text).X;
                if (test < test2)
                {
                    test = test2;
                }
            }
            maxlength = test;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!refresh)
            {
                X = game.Graphics.Device.Viewport.Width / 2;
            }

            if (introtext.Zeile[0].Y != game.Graphics.Device.Viewport.Height + 5 && refresh == false)
            {
                refresh = true;
                for (int i = 0; i < introtext.Zeile.Count; i++)
                {
                    introtext.Zeile[i].Y = game.Graphics.Device.Viewport.Height + i *
                        (introtext.Zeile[i].Scale * font.MeasureString(introtext.Zeile[i].Text).Y);
                }
            }

            #region Input
            if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
            {
                //game.GameStates.SetStateProfil();
                game.GameStates.SetStateMenu();
                font.Spacing = 0;
            }
            else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
            {
                //game.GameStates.SetStateProfil();
                game.GameStates.SetStateMenu();
                font.Spacing = 0;
            }
            else if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
            {
                //game.GameStates.SetStateProfil();
                game.GameStates.SetStateMenu();
                font.Spacing = 0;
            }
            #endregion

            #region Aktualisierung des Y und Scale Wertes
            for (int i = 0; i < introtext.Zeile.Count; i++)
            {
                if (introtext.Zeile[i].Y > -30f)
                { 
                    introtext.Zeile[i].Y -= 0.5f;
                }
                if (introtext.Zeile[i].Y <= game.Graphics.Device.Viewport.Height && introtext.Zeile[i].Scale >= 0)
                {
                    //introtext.Zeile[i].Scale -= 0.0016f;
                    introtext.Zeile[i].Scale -= 0.0023f;
                }
            }
            #endregion

            if (introtext.Zeile[introtext.Zeile.Count - 1].Scale <= 0)
            {
                this.Visible = false;
                //game.GameStates.SetStateProfil();
                game.GameStates.SetStateMenu();
                font.Spacing = 0;
            }
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.  
        /// Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to 
        /// Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime)
        /// </param>
        public override void Draw(GameTime gameTime)
        {
            if (SpriteBatch != null)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(back, new Rectangle(0, 0, game.Graphics.Device.Viewport.Width,
                    game.Graphics.Device.Viewport.Height), Color.White);
                for (int i = 0; i < introtext.Zeile.Count; i++)
                {
                    font.Spacing = 0f;
                    int currentlength = (int)font.MeasureString(introtext.Zeile[i].Text).X;
                    if (currentlength < maxlength && currentlength != 0)
                    {
                        while ((maxlength - currentlength) > 15)
                        {
                            font.Spacing += (float)(maxlength / currentlength);
                            currentlength = (int)font.MeasureString(introtext.Zeile[i].Text).X;
                        }
                    }

                    Vector2 test = font.MeasureString(introtext.Zeile[i].Text);
                    SpriteBatch.DrawString(font, introtext.Zeile[i].Text,
                        new Vector2((X - (introtext.Zeile[i].Scale * test.X / 2)), introtext.Zeile[i].Y),
                        Color.Yellow, 0, new Vector2(0, 0), introtext.Zeile[i].Scale, SpriteEffects.None, 0);
                }
                SpriteBatch.End();
            }
        }

        /// <summary>
        /// Called when the component needs to load graphics resources.  Override to
        /// load any component specific graphics resources.
        /// 
        /// Parameters:
        ///  loadAllContent:
        ///    true if all graphics resources need to be loaded;false if only manual resources
        ///    need to be loaded.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(game.Graphics.Device);
            font = game.Content.Load<SpriteFont>("Arial");
            back = game.Content.Load<Texture2D>("Content\\Textures\\HistoryBackground");
        }
    }
}