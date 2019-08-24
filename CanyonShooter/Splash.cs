using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter
{
    class Splash : DrawableGameComponent
    {
        private Texture2D background;
        private ICanyonShooterGame game;
        private SpriteBatch spriteBatch;

        public Splash(ICanyonShooterGame game)
            : base(game as Game) 
        {
            this.game = game;
            this.Visible = true;
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
            if (spriteBatch != null)
            {
                spriteBatch.Begin();
                spriteBatch.GraphicsDevice.Clear(Color.Black);
                spriteBatch.Draw(background, new Rectangle((game.Graphics.Device.Viewport.Width/2)-(background.Width/2) , (game.Graphics.Device.Viewport.Height/2)-(background.Height/2), 
                    640, 383), Color.White);
                spriteBatch.End();
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
            spriteBatch = new SpriteBatch(game.Graphics.Device);
            background = game.Content.Load<Texture2D>("Content\\Textures\\Splash");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Helper.WaitFor("Splash", TimeSpan.FromSeconds(3)))
            {
                this.Visible = false;
                Helper.ResetWait("Splash");
                game.GameStates.SetStateIntro();
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
