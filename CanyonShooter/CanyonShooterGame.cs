// Zuständigkeit: Richard, Christian, Florian

#region Using Statements

using System.Diagnostics;
using CanyonShooter.Engine;
using CanyonShooter.Engine.Audio;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Input;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.Scores;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CanyonShooter.Tests;
using System;
using System.Threading;
using CanyonShooter.Engine.Helper;
using System.IO;
using CanyonShooter.GameClasses;
#endregion

namespace CanyonShooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CanyonShooterGame : Game, ICanyonShooterGame
    {
        #region Private Data Members
        private Config config;
        private GameStates states;
        private ContentManager content;
        private GraphicsDeviceManager graphicsDeviceManager;

        private IGraphics graphics;
        private IRenderer renderer;
        private IEffectFactory effects;
        private IWorld world;
        private IPhysics physics;
        // Sound - M.Rodriguez -------------
        private ISoundSystem sounds;
        
        // ---------------------------------
        private IInput input;
        private IHighscores highscores;

        private bool debugMode;


        #endregion

        public TDxInput.Device device;

        private bool hasSpaceMouse = false;
        public bool HasSpaceMouse
        {
            get
            {
                return hasSpaceMouse;
            }
            set
            {
                hasSpaceMouse = value;
            }
        }

        public Arguments Args = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="CanyonShooterGame"/> class.
        /// </summary>
        public CanyonShooterGame(string[] args)
        {
            Args = new Arguments(args);
            states = new GameStates(this,Components);
            config = new Config();
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            graphics = new Graphics(this, graphicsDeviceManager);
            this.Window.Title = " CanyonShooter";
            
            Intercom.Game = this;

            #region Commandline Parameter Settings:

            #region Parameter: --setShaderModel

            if (Args["setShaderModel"] == "2")
                graphics.ShaderModel3SupportedOverride = true;
            #endregion

            #region Parameter: --debug

            if (Args["debug"] == "1")
                debugMode = true;
            #endregion

            #region Parameter: --multiThreaded
            // Physik Intialisierung
            /*if (Args["multiThreaded"] != null)
            {
                if(Args["multiThreaded"] == "1")
                    physics = new Physics(this, true);
                else
                {
                    physics = new Physics(this, false);
                    Components.Add(physics);
                }
            }
            else*/ // automatically set threading-mode
            {
                /*if (Environment.ProcessorCount > 1)
                {
                    physics = new Physics(this, true);
                }
                else*/
                {
                    physics = new Physics(this, false);
                    Components.Add(physics);
                }
            }
            #endregion

            #region Parameter: --testAudio
            // Test Audio Framework *******************************
            if (Args["testAudio"] == "1")
            {
                TestAudioPlayback test1 = new TestAudioPlayback();
                test1.SetUp(this);
                test1.TestMinigunPlayback();
                test1.TearDown();

                TestAudio3D test2 = new TestAudio3D();
                test2.SetUp(this);
                test2.TestPlayback3D();
                test2.TearDown();
            }
            #endregion

            #endregion

            // Create sound system by M.Rodriguez
            sounds = new SoundSystem(this);

            // Set global volumes by M.Rodriguez
            sounds.EffectVolume = 1.0f;
            sounds.MusicVolume = 0.3f;

            // Initialisieren der einzelnen Komponenten:
            input = new Input(this);
            renderer = new Renderer(this, Components);
            effects = new EffectFactory(this);
            highscores = new Highscores(this);

            GraphicalConsole.GetSingleton(this).RegisterObjectProperty(graphics, "Graphics", "Fullscreen");
            
            GraphicalConsole.GetSingleton(this).RegisterObjectProperty(renderer, "Renderer", "DrawCollisionShapes");

            GraphicalConsole.GetSingleton(this).RegisterObjectFunction(Args, "Args", "ListParameters");
            if (Args["enable3DMouse"] == "1")
            {
                hasSpaceMouse = true;
                device = new TDxInput.Device();
            }
            states.SetStateStart();
            
            // states.SetStateDebugMode();

        }

        protected override void Dispose(bool disposing)
        {
            //physics.Dispose();

            base.Dispose(disposing);

            // Unload Sound by M.Rodriguez
            //sounds.Shutdown();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            input.Init();

            base.Initialize();
        }


        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: Load any ResourceManagementMode.Manual content
            renderer.Camera.OnDeviceChanged();

            // BlueScreen
            blueScreen = new SpriteBatch(Graphics.Device); 
            blueScreenTexture = content.Load<Texture2D>("Content/Textures/MenuEasterEgg");
        }

        private Texture2D blueScreenTexture;
        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
        }

        private bool cmdlineUpdate = true;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if(cmdlineUpdate)
            {
                #region Parameter: --fullscreen

                if (Args["fullscreen"] == "1")
                    graphics.Fullscreen = true;
                #endregion

                #region Parameter: --screenResolution

                if (Args["screenResolution"] != null)
                {
                    // feel free to add more, if you like.
                    if (Args["screenResolution"] == "640x480")
                        graphics.SetScreenResolution(640, 480);
                    if (Args["screenResolution"] == "800x600")
                        graphics.SetScreenResolution(800, 600);
                    if (Args["screenResolution"] == "1024x768")
                        graphics.SetScreenResolution(1024, 768);
                    if (Args["screenResolution"] == "1280x1024")
                        graphics.SetScreenResolution(1280, 1024);
                    if (Args["screenResolution"] == "1920x1200")
                        graphics.SetScreenResolution(1920, 1200);
                }

                #endregion

                #region Parameter: --quickGame

                if (Args["quickGame"] != null)
                {
                    this.GameStates.Profil.Quick = true;
                    this.GameStates.Profil.QuickLevel = Args["quickGame"];
                }

                #endregion

                cmdlineUpdate = false;
            }

            if (Input.HasKeyJustBeenPressed("Post.Bug"))
            {
                //Post a bug!
                try
                {
                    Process.Start(@"http://www.cellrays.de/csbugs/bug_report_page.php");
                }
                catch
                {
                    
                }
            }

            

            // Allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (IsActive) input.Update();

            sounds.Update();
    
            base.Update(gameTime);

            if (physics.MultiThreading)
            {
                //Debug.Print("game");
                // synchronize with physics-thread's main loop
                lock (physics.MayPhysicsContinueSignal)
                {
                    physics.MayPhysicsContinue = true;
                    Monitor.PulseAll(physics.MayPhysicsContinueSignal);
                }
                /*lock (physics.MayGameContinueSignal)
                {
                    if (!mayContinue) Monitor.Wait(physics.MayGameContinueSignal);
                    physics.MayGameContinue = false;
                }*/
            }
        }

        private SpriteBatch blueScreen;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.Device.Clear(Color.CornflowerBlue);

            DrawBlueScreen();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawBlueScreen()
        {
            // BlueScreen to Fix blue menu bug.
            if (blueScreen != null)
            {
                blueScreen.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState,
                                 Matrix.CreateScale(Graphics.ScreenWidth, Graphics.ScreenHeight, 1));
                blueScreen.Draw(blueScreenTexture, new Rectangle(0, 0, 1, 1), Color.White);
                blueScreen.End();
            }
        }


        public void LoadLevel(string levelname)
        {
            if (File.Exists(string.Format("GameClasses\\World\\Levels\\{0}.csl", levelname)))
                GameStates.SetStateGame(levelname);
            else
                GraphicalConsole.GetSingleton(this).WriteLine("Error: Level not found!", 0);
        }


        #region ICanyonShooterGame Members

        ContentManager ICanyonShooterGame.Content
        {
            get { return content; }
        }

        Config ICanyonShooterGame.Config
        {
            get { return config; }
        }

        public IGraphics Graphics
        {
            get { return graphics; }
        }

        public IWorld World
        {
            get { return world; }
            set { world = value; } // Damit GameObjects direkt hinzugefügt werden können
        }

        public IPhysics Physics
        {
            get { return physics; }
        }

        public IRenderer Renderer
        {
            get { return renderer; }
        }

        public IEffectFactory Effects
        {
            get { return effects;}
        }

        public IInput Input
        {
            get { return input; }
        }

        public IHighscores Highscores
        {
            get { return highscores; }
        }

        public GameStates GameStates
        {
            get { return states; }
        }

        public ISoundSystem Sounds
        {
            get { return sounds;}
        }

        #endregion



        #region ICanyonShooterGame Members


        public bool DebugMode
        {
            get { return debugMode; }
        }

        #endregion
    }

}
