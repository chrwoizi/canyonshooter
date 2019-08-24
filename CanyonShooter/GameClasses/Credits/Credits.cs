using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DescriptionLibs.Credits;

namespace CanyonShooter.GameClasses.Credits
{
    /// <summary>
    /// Class Credits manages and starts the Credits
    /// </summary>
    class Credits : DrawableGameComponent
    {
        #region Data Member

        private bool refresh = false;

        private Texture2D last;
        private Texture2D black;
        private SpriteBatch SpriteBatch;
        private SpriteFont font;
        private Texture2D back;
        private Texture2D bild;

        private CreditDescription creditstext;
        private float X;

        private ICanyonShooterGame game;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">game is the Canyonshootergame</param>
        public Credits(ICanyonShooterGame game)
            : base(game as Game)
        {
            Game.Exit(); //For Presentation
            this.game = game;
            this.Visible = true;
            creditstext = game.Content.Load<CreditDescription>("Content\\Credits\\Credits");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            if (!refresh)
            {
                X = game.Graphics.Device.Viewport.Width / 2;
            }
            if (creditstext.Zeile[0].Y != game.Graphics.Device.Viewport.Height + 5 && refresh == false)
            {
                creditstext.Zeile[0].Y = game.Graphics.Device.Viewport.Height + 5;
                refresh = true;
                for (int i = 1; i < creditstext.Zeile.Count; i++)
                {
                    if (creditstext.Zeile[i].Scale == 1.5f && creditstext.Zeile[i - 1].Scale == 2)
                    {
                        creditstext.Zeile[i].Y = creditstext.Zeile[i - 1].Y +
                            (creditstext.Zeile[i].Scale * font.MeasureString(creditstext.Zeile[i].Text).Y + 5);
                    }
                    else if (creditstext.Zeile[i].Scale == 2f)
                    {
                        creditstext.Zeile[i].Y = creditstext.Zeile[i - 1].Y +
                            (creditstext.Zeile[i].Scale * font.MeasureString(creditstext.Zeile[i].Text).Y);
                    }
                    else if (creditstext.Zeile[i].Text.Equals(""))
                    {
                        creditstext.Zeile[i].Y = creditstext.Zeile[i - 1].Y + 24;
                    }
                    else
                    {
                        creditstext.Zeile[i].Y = creditstext.Zeile[i - 1].Y +
                            (creditstext.Zeile[i].Scale * font.MeasureString(creditstext.Zeile[i].Text).Y);
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            #region Input
            if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
            {
                Game.Exit();
            }
            else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
            {
                Game.Exit();
            }
            else if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
            {
                Game.Exit();
            }
            #endregion

            #region Aktualisierung des Y und Scale Wertes
            for (int i = 0; i < creditstext.Zeile.Count; i++)
            {
                if (creditstext.Zeile[i].Y > -75f)
                {
                    creditstext.Zeile[i].Y -= 0.5f;
                }
            }
            #endregion

            if (creditstext.Zeile[creditstext.Zeile.Count - 1].Y == -75)
            {
                this.Visible = false;
                Game.Exit();
            }
        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.  Override this method
        /// with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last 
        /// call to Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime)
        /// </param>
        public override void Draw(GameTime gameTime)
        {
            if (SpriteBatch != null)
            {
                SpriteBatch.Begin();

                SpriteBatch.Draw(back, new Rectangle(0, 0, game.Graphics.Device.Viewport.Width,
                    game.Graphics.Device.Viewport.Height), Color.White);
                for (int i = 0; i < creditstext.Zeile.Count; i++)
                {

                    SpriteBatch.DrawString(font, creditstext.Zeile[i].Text,
                        new Vector2(X, creditstext.Zeile[i].Y), Color.Yellow, 0,
                        new Vector2(0, 0), creditstext.Zeile[i].Scale, SpriteEffects.None, 0);
                    
                    //Load next Picture
                    if ((creditstext.Zeile[i].Y <= game.Graphics.Device.Viewport.Height
                        - game.Graphics.Device.Viewport.Height / 4) 
                        && creditstext.Zeile[i].Load)
                    {
                        creditstext.Zeile[creditstext.CurrentLine].Start = false;
                        creditstext.CurrentLine = i;
                        Load();
                    }
                    //Draw Picture
                    if (creditstext.Zeile[creditstext.CurrentLine].Start)
                    {
                        SpriteBatch.Draw(black, new Rectangle((game.Graphics.Device.Viewport.Width / 4) -
                       195, (game.Graphics.Device.Viewport.Height / 2) - 195, 390, 390), Color.White);
                        SpriteBatch.Draw(bild, new Rectangle((game.Graphics.Device.Viewport.Width / 4) -
                            creditstext.Zeile[creditstext.CurrentLine].Pwidth / 2,
                            (game.Graphics.Device.Viewport.Height / 2) -
                            creditstext.Zeile[creditstext.CurrentLine].Pheight / 2,
                            creditstext.Zeile[creditstext.CurrentLine].Pwidth,
                            creditstext.Zeile[creditstext.CurrentLine].Pheight), Color.White);
                    }

                    if (creditstext.Zeile[creditstext.Zeile.Count-1].Y <= 30)
                        {
                            SpriteBatch.Draw(back, new Rectangle(0, 0, game.Graphics.Device.Viewport.Width,
                        game.Graphics.Device.Viewport.Height), Color.White);
                            SpriteBatch.DrawString(font, creditstext.Zeile[i].Text,
                            new Vector2(X, creditstext.Zeile[i].Y), Color.Yellow, 0,
                            new Vector2(0, 0), creditstext.Zeile[i].Scale, SpriteEffects.None, 0);
                            SpriteBatch.Draw(last, new Rectangle((game.Graphics.Device.Viewport.Width / 2) -
                                last.Width / 2,
                                (game.Graphics.Device.Viewport.Height / 2) -
                                last.Height / 2, last.Width,last.Height), Color.White);
                        }
                }
                SpriteBatch.End();
            }
        }

        /// <summary>
        /// Load next Picture
        /// </summary>
        private void Load()
        {
            if (creditstext.Zeile[creditstext.CurrentLine].Load)
            {
                bild = game.Content.Load<Texture2D>("Content\\Textures\\Credits\\" +
                    creditstext.Zeile[creditstext.CurrentLine].Picture);
                creditstext.Zeile[creditstext.CurrentLine].Start = true;
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
            black = game.Content.Load<Texture2D>("Content\\Textures\\Credits\\Black");
            back = game.Content.Load<Texture2D>("Content\\Textures\\HistoryBackground");
            last = game.Content.Load<Texture2D>("Content\\Textures\\Credits\\svn");
        }
    }
}