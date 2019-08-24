using System.Collections.Generic;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Graphics.Lights;
using CanyonShooter.Engine.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Model = CanyonShooter.Engine.Graphics.Models.Model;
using Microsoft.Xna.Framework.Input;

namespace CanyonShooter.GameClasses.ModelChooser
{
    /// <summary>
    /// Class Chooser manage the choice of the Model for playing the game
    /// </summary>
    public class Chooser : DrawableGameComponent
    {
        #region Data Member

        #region Model & Camera
        private Dictionary<string, Model> models = new Dictionary<string, Model>();
        private Dictionary<string, Vector3> modposition;
        private PerspectiveCamera camera;
        private string currentModel;
        private Vector2 modelRotation = new Vector2(0, 0);
        private bool load = false;
        #endregion

        #region Draw
        private SpriteFont font;
        private SpriteBatch spriteBatch;
        private Texture2D right;
        private Texture2D left;
        private Texture2D shieldpower;
        private Texture2D healthpower;
        private Texture2D speed;
        private Texture2D fuel;
        private Texture2D background;
        private Texture2D paramback;
        private Texture2D modelName;

        private int shieldparam;
        private int speedparam;
        private int healthparam;
        private int fuelparam;
        #endregion

        private int cheat;
        private float modelRot = 0f;
        private bool pressed = false;

        private ICanyonShooterGame game;
        #endregion

        /// <summary>
        /// Constructor of the Modelchooser
        /// </summary>
        public Chooser(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
            this.Visible = true;
            modelRotation = new Vector2(0, 0);
        }

        /// <summary>
        /// Nice sound if the button has been pressed. by M.R.
        /// </summary>
        private void PlayButtonSound()
        {
            ISound snd = game.Sounds.CreateSound("Button");
            snd.Play();
            snd.Dispose();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            camera = new PerspectiveCamera(game);
            game.Renderer.Camera = camera;
            game.World.Sky.Sunlight = new Sunlight(game, new Color(255, 255, 255), new Vector3(0, 0, -1));
            camera.LocalPosition = new Vector3(0, 0, 3);
            camera.LocalRotation = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
            camera.Fov = 90; //Öffnungswinkel

            //For better Rotation
            Mouse.SetPosition(game.Graphics.Device.Viewport.Width / 2,
                        game.Graphics.Device.Viewport.Height / 2);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            currentModel = (string)game.GameStates.Profil.CurrentProfil.Model;
            shieldparam = (int)game.GameStates.Profil.CurrentProfil.Shield;
            speedparam = (int)game.GameStates.Profil.CurrentProfil.Speed;
            healthparam = (int)game.GameStates.Profil.CurrentProfil.Health;
            fuelparam = (int)game.GameStates.Profil.CurrentProfil.Fuel;
            Load(currentModel);
            getFighterParameter(currentModel);
            spriteBatch = new SpriteBatch(game.Graphics.Device);
            modposition = new Dictionary<string, Vector3>();
            font = game.Content.Load<SpriteFont>("Arial");
            //getFighterParameter(currentModel);

            game.Graphics.ShadowMappingSupportedOverride = true;
            game.Graphics.ShadowMappingSupportedOverrideValue = false;

            #region Starfighter
            // Load the Starfighter model
            Model m = new Model(game, "Starfighter");
            m.ShowAfterBurner = false;
            models.Add("Starfighter", m);
            modposition.Add("Starfighter", new Vector3(0, -1, -12));
            #endregion

            #region Drone
            // Load the Drone model
            m = new Model(game, "Drone");
            m.ShowAfterBurner = false;
            models.Add("Drone", m);
            modposition.Add("Drone", new Vector3(0, -2, -18));
            #endregion

            #region F35eger
            // Load F35eger model
            m = new Model(game, "F35eger");
            m.ShowAfterBurner = false;
            models.Add("F35eger", m);
            modposition.Add("F35eger", new Vector3(0, -2, -16));
            #endregion

            #region Fighter
            // Load Fighter model
            m = new Model(game, "Fighter");
            m.ShowAfterBurner = false;
            models.Add("Fighter", m);
            modposition.Add("Fighter", new Vector3(0, -3, -19));
            #endregion

            #region G35Hopper
            // Load G35Hopper model
            m = new Model(game, "G35Hopper");
            m.ShowAfterBurner = false;
            models.Add("G35Hopper", m);
            modposition.Add("G35Hopper", new Vector3(0, -5, -20));
            #endregion

            game.Graphics.ShadowMappingSupportedOverride = false;

            #region Draw
            left = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\" + "ArrowLeft");
            right = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\" + "ArrowRight");
            paramback = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\ProfilTextBack");
            shieldpower = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\" + "BalkenSchild");
            speed = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\" + "BalkenSpeed");
            background = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\Chooser");
            healthpower = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\BalkenHealth");
            fuel = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\BalkenFuel");
            #endregion
        }

        /// <summary>
        /// Loads next Modelnametexture
        /// </summary>
        /// <param name="Name">Name of the Model which to load</param>
        private void Load(string Name)
        {
            modelName = game.Content.Load<Texture2D>("Content\\Textures\\Chooser\\" + Name);
            load = true;
        }

        /// <summary>
        /// Set the params Speed and Shield
        /// </summary>
        private void getFighterParameter(string name)
        {
            switch (name)
            {
                case "Starfighter":
                    {
                        shieldparam = 80;          // Shield power
                        speedparam = 410;            // Speed
                        healthparam = 100;          // Health
                        fuelparam = 500;            //Fuel
                        cheat = 95;
                        break;
                    }
                case "F35eger":
                    {
                        shieldparam = 65;
                        speedparam = 470;
                        healthparam = 100;
                        fuelparam = 700;
                        cheat = 124;
                        break;
                    }
                case "Fighter":
                    {
                        shieldparam = 50;
                        speedparam = 370;
                        healthparam = 100;
                        fuelparam = 400;
                        cheat = 80;
                        break;
                    }
                case "G35Hopper":
                    {
                        shieldparam = 70;
                        speedparam = 400;
                        healthparam = 100;
                        fuelparam = 700;
                        cheat = 105;
                        break;
                    }
                case "Drone":
                    {
                        shieldparam = 50;
                        speedparam = 320;
                        healthparam = 70;
                        fuelparam = 250;
                        cheat = 55;
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            string test = currentModel;

            #region Key Right
            if (game.Input.HasKeyJustBeenPressed("Menu.Right"))
            {
                if (currentModel.Equals("Drone"))
                {
                    currentModel = "Starfighter";
                    load = false;
                }
                else if (currentModel.Equals("Starfighter"))
                {
                    currentModel = "F35eger";
                    load = false;
                }
                else if (currentModel.Equals("F35eger"))
                {
                    currentModel = "Fighter";
                    load = false;
                }
                else if (currentModel.Equals("Fighter"))
                {
                    currentModel = "G35Hopper";
                    load = false;
                }
                else if (currentModel.Equals("G35Hopper"))
                {
                    currentModel = "Drone";
                    load = false;
                }
                // Nice sound if the button has been pressed. by M.R.
                PlayButtonSound();
                // ------------------------------------------
            }
            #endregion

            #region Button Right
            else if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryFire"))
            {
                if (pressed)
                {
                    pressed = false;
                }
                else
                {
                    pressed = true;
                }
            }
            #endregion

            #region Key Left
            else if (game.Input.HasKeyJustBeenPressed("Menu.Left"))
            {
                if (currentModel.Equals("Drone"))
                {
                    currentModel = "G35Hopper";
                    load = false;
                }
                else if (currentModel.Equals("G35Hopper"))
                {
                    currentModel = "Fighter";
                    load = false;
                }
                else if (currentModel.Equals("Fighter"))
                {
                    currentModel = "F35eger";
                    load = false;
                }
                else if (currentModel.Equals("F35eger"))
                {
                    currentModel = "Starfighter";
                    load = false;
                }
                else if (currentModel.Equals("Starfighter"))
                {
                    currentModel = "Drone";
                    load = false;
                }
                // Nice sound if the button has been pressed. by M.R.
                PlayButtonSound();
                // ------------------------------------------
            }
            #endregion

            #region Key Enter & Button Left
            else if (game.Input.HasKeyJustBeenPressed("Menu.Ok") || game.Input.HasKeyJustBeenPressed("Menu.Select"))
            {
                this.Visible = false;
                game.GameStates.Profil.writeProfilGleiter(currentModel, shieldparam, speedparam, healthparam, fuelparam);
                //game.GameStates.SetStateGame("test");
                game.GameStates.SetStateLevelChoose();
            }
            #endregion

            #region Key Escape
            else if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
            {
                this.Visible = false;
                game.GameStates.SetStateMenu();
            }
            #endregion

            if (!currentModel.Equals(test))
            {
                getFighterParameter(currentModel);
                pressed = false;
                modelRotation = new Vector2(0, 0);
                modelRot = 0f;
            }
            if (!load)
            {
                Load(currentModel);
            }

            if (pressed)
            {
                modelRotation = new Vector2((game.Input.MousePosition.X - game.Graphics.Device.Viewport.Width / 2),
                    (game.Input.MousePosition.Y - game.Graphics.Device.Viewport.Height / 2));
            }
            else
            {
                modelRot += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.06f);
                if (modelRot == 10000)
                {
                    modelRot = 0;
                    modelRot += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.06f);
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (spriteBatch != null)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(background, new Rectangle(0, 0, game.Graphics.Device.Viewport.Width,
                    game.Graphics.Device.Viewport.Height), Color.White);
                spriteBatch.Draw(paramback, new Rectangle(game.Graphics.Device.Viewport.Width - 300,
                    10, (int)(10 + font.MeasureString("Speed").X + font.MeasureString("Shield").X +
                    font.MeasureString("Health").X + font.MeasureString("Fuel").X + 10 + 35 + 35), 200), Color.White);
                spriteBatch.Draw(paramback, new Rectangle(5, 10, 475, 130), Color.White);

                #region Text
                spriteBatch.DrawString(font, "Please select a Fighter:",
                    new Vector2(15, 20), Color.White);
                spriteBatch.DrawString(font, "Enter/Leftbutton - Play",
                    new Vector2(25, 40), Color.White);
                spriteBatch.DrawString(font, "Esc - Menu",
                    new Vector2(25, 60), Color.White);
                spriteBatch.DrawString(font, "Arrows - Change Fighter",
                    new Vector2(25, 80), Color.White);
               // spriteBatch.DrawString(font, "Mousemovement - Rotation des Gleiters",
                 //   new Vector2(25, 100), Color.White);
                 spriteBatch.DrawString(font, "Rightbutton + Mousemovement - Rotation On/Off",
                    new Vector2(25, 100), Color.White);
                #endregion

                #region Richtungsanweisungen
                spriteBatch.Draw(right, new Rectangle(game.Graphics.Device.Viewport.Width - 100,
                    game.Graphics.Device.Viewport.Height / 2, 32, 32), Color.White);
                spriteBatch.DrawString(font, "Next Fighter",
                    new Vector2(game.Graphics.Device.Viewport.Width - 145,
                    game.Graphics.Device.Viewport.Height / 2 + 35), Color.White);
                spriteBatch.Draw(left, new Rectangle(68, game.Graphics.Device.Viewport.Height / 2,
                    32, 32), Color.White);
                spriteBatch.DrawString(font, "Previous Fighter",
                    new Vector2(20, game.Graphics.Device.Viewport.Height / 2 + 35), Color.White);
                #endregion

                #region Gleiterparameter anzeigen
                spriteBatch.Draw(speed, new Rectangle(game.Graphics.Device.Viewport.Width - 260, 30,
                    8, cheat), Color.White);
                spriteBatch.Draw(shieldpower, new Rectangle(game.Graphics.Device.Viewport.Width - 200, 30,
                    8, shieldparam), Color.White);
                spriteBatch.Draw(healthpower, new Rectangle(game.Graphics.Device.Viewport.Width - 140, 30,
                    8, healthparam), Color.White);
                spriteBatch.Draw(fuel, new Rectangle(game.Graphics.Device.Viewport.Width - 80, 30,
                    8, fuelparam / 10), Color.White);

                spriteBatch.DrawString(font, "Speed", new Vector2(game.Graphics.Device.Viewport.Width - 290, 163),
                    Color.White);
                spriteBatch.DrawString(font, "Shield", new Vector2(game.Graphics.Device.Viewport.Width - 225, 163),
                    Color.White);
                spriteBatch.DrawString(font, "Health", new Vector2(game.Graphics.Device.Viewport.Width - 160, 163),
                    Color.White);
                spriteBatch.DrawString(font, "Fuel", new Vector2(game.Graphics.Device.Viewport.Width - 95, 163),
                    Color.White);
                #endregion

                spriteBatch.Draw(modelName, new Rectangle(30, game.Graphics.Device.Viewport.Height - 70,
                    300, 50), Color.White);
                spriteBatch.End();

                #region Models
                models[currentModel].LocalPosition = modposition[currentModel];


                models[currentModel].LocalRotation = Quaternion.CreateFromYawPitchRoll(-3.15f +
                    (float)(modelRotation.X / 100) - modelRot, -0.4f + (float)(modelRotation.Y / 100), 0);


                models[currentModel].Draw(null);
                #endregion
            }
        }
    }
}
