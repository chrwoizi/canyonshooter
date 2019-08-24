using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Menus
{
    class LevelChooser : DrawableGameComponent
    {
        private SpriteBatch SpriteBatch;
        private Texture2D background;
        private Texture2D backgroundText;
        private SpriteFont font;
        private string[] levels;

        private int currentLevel;

        private ICanyonShooterGame game;

        public LevelChooser(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
            levels = new string[4];
            levels[0] = "The Race";
            levels[1] = "The Corkscrew";
            levels[2] = "The Way goes Up and Down";
            levels[3] = "The Hell of Tunnels";
            
            currentLevel = 0;
        }

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
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
            {
                game.GameStates.SetStateMenu();
            }
            else if (game.Input.HasKeyJustBeenPressed("Menu.Up"))
            {
                if (currentLevel == 0)
                {
                    currentLevel = 3;
                }
                else
                {
                    currentLevel -= 1;
                }
            }
            else if (game.Input.HasKeyJustBeenPressed("Menu.Down"))
            {
                if (currentLevel == 3)
                {
                    currentLevel = 0;
                }
                else
                {
                    currentLevel += 1;
                }
            }
            else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
            {
                game.GameStates.SetStateGame(levels[currentLevel]);
            }
        }

        //
        // Zusammenfassung:
        //     Called when the DrawableGameComponent needs to be drawn.  Override this method
        //     with component-specific drawing code.
        //
        // Parameter:
        //   gameTime:
        //     Time passed since the last call to Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime).
        public override void Draw(GameTime gameTime)
        {
            if (SpriteBatch != null)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(background, new Rectangle(0, 0, game.Graphics.Device.Viewport.Width,
                    game.Graphics.Device.Viewport.Height), Color.White);
                SpriteBatch.Draw(backgroundText, new Rectangle(game.Graphics.Device.Viewport.Width/2 - 300,
                    game.Graphics.Device.Viewport.Height/2 -200, 600, 400), Color.White);
                SpriteBatch.DrawString(font, "Please choose a level:",
                    new Vector2(game.Graphics.Device.Viewport.Width / 2 - 300 + 30,
                            game.Graphics.Device.Viewport.Height / 2 - 200 + 25), Color.Black);
                for (int i = 0; i < levels.Length; i++)
                {
                    if (i == currentLevel)
                    {
                        SpriteBatch.DrawString(font, levels[i], 
                            new Vector2(game.Graphics.Device.Viewport.Width / 2 - 300 + 30,
                            game.Graphics.Device.Viewport.Height / 2 - 200 + 80 + 60 * i), Color.Red);
                    }
                    else
                    {
                        SpriteBatch.DrawString(font, levels[i], 
                            new Vector2(game.Graphics.Device.Viewport.Width / 2 - 300 + 30,
                            game.Graphics.Device.Viewport.Height / 2 - 200 + 80 + 60 * i), Color.Black);
                    }
                }
                SpriteBatch.End();
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
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(game.Graphics.Device);
            background = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\Profile");
            backgroundText = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\ProfilTextBack");
            font = game.Content.Load<SpriteFont>("Arial");
        }
    }
}
