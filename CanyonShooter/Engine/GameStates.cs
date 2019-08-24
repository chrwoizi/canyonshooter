using System;
using System.Diagnostics;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.Huds;
using CanyonShooter.GameClasses.ModelChooser;
using CanyonShooter.GameClasses.World;
using CanyonShooter.GameClasses.Scores;
using CanyonShooter.GameClasses.Intro;
using CanyonShooter.GameClasses.Credits;
using CanyonShooter.Menus;
using CanyonShooter.Engine.Audio;
using CanyonShooter.Profils;
using Microsoft.Xna.Framework;
using CanyonShooter.GameClasses;

namespace CanyonShooter.Engine
{
    public class GameStates
    {
        private ICanyonShooterGame game = null;
        private GameComponentCollection components = null;

        private Object inputFocus = null;
        public Object InputFocus { get { return inputFocus; } set { inputFocus = value; } }

        private IHud hud = null;
        private IGraphicalConsole console = null;
        private IMenu menu = null;
        private IProfil profil = null;
        private Chooser chooser = null;
        private Intro intro = null;
        private Credits credits = null;
        private IScore score = null;
        private Splash splash = null;

        private LevelChooser level = null;

        public bool Gameplayed = false;
        public bool Gamepaused = false;

        public GameStates(ICanyonShooterGame Game, GameComponentCollection Components)
        {
            game = Game;
            components = Components;
        }

        public void Reset()
        {
            if(welcomeSound != null)
                welcomeSound.Stop();

            //components.Remove(profil); profil = null;
            components.Remove(chooser); if (chooser != null) chooser.Dispose(); chooser = null;
            components.Remove(intro); if (intro != null) intro.Dispose(); intro = null;
            components.Remove(credits); if (credits != null) credits.Dispose(); credits = null;
            components.Remove(menu); menu = null;
            components.Remove(level); level = null;
            
            components.Remove(hud); hud = null;
            components.Remove(console); console = null;

            components.Clear();

            if (game.World != null)
            {
                //components.Remove(game.World);
                game.World.Dispose();
                game.World = null;
            }

            /*
            if (score != null)
            {
                score.Reset();
            } 
            score = null;*/
            Material.ClearSharedPool();
        }

        public void SetStateStart()
        {
            SetStateProfil();
            //SetStateIntro();
        }

        public void SetStateProfil()
        {
            Reset();
            if (profil == null)
            {
                profil = new Profil(game);
                components.Add(profil);
            }
            else if (!components.Contains(profil))
            {
                profil = null;
                profil = new Profil(game);
                components.Add(profil);
                profil.Menü = true;
                profil.Opt = true;
                profil.Esc = true;
            }
            inputFocus = profil;
        }

        public void SetStateMenu()
        {
            Reset();
            if (Gameplayed)
            {
                menu = new Menu(game, "Hauptmenu", true);
            }
            else
            {
                menu = new Menu(game, "Hauptmenu");
            }
            if (Profil.Opt)
            {
                menu.Shifts.init("Optionen", true);
                menu.CurrentMenu = "Optionen";
                Profil.Opt = false;
            }
            components.Add(menu);
            game.GameStates.Menu.Action = true;
            inputFocus = menu;
            Gameplayed = false;
        }

        public void SetStateChooser()
        {
            Reset();
            game.World = new World(game, "", components);
            chooser = new Chooser(game);
            components.Add(chooser);
            inputFocus = chooser;
        }

        public void SetStateLevelChoose()
        {
            Reset();
            level = new LevelChooser(game);
            components.Add(level);
        }

        public void SetStateSplash()
        {
            Reset();
            splash = new Splash(game);
            components.Add(splash);
        }

        public void SetStateIntro()
        {
            Reset();
            intro = new Intro(game);
            components.Add(intro);
            //inputFocus = chooser;
            game.Sounds.MusicBox(MusicBoxStatus.Play);
        }

        public void SetStateCredits()
        {
            Reset();
            credits = new Credits(game);
            components.Add(credits);
            //inputFocus = chooser;
            game.Sounds.MusicBox(MusicBoxStatus.Play);
        }

        public void SetStateGame(string levelname)
        {
            if (score != null)
            {
                score.Reset();
            }
            score = null;

            game.Graphics.ShadowMappingSupportedOverride = !profil.CurrentProfil.Shadow;
            game.Graphics.AllowFogShaders = profil.CurrentProfil.Fog;

            Reset();
            
            
            components.Add(game.Physics);

            game.Input.MouseMovement = new Vector2(0,0);

            hud = new Hud(game, "test");
            components.Add(hud);
            components.Add(GraphicalConsole.GetSingleton(game));
            GraphicalConsole.GetSingleton(game).RegisterObjectFunction(game, "Game", "Exit");
            GraphicalConsole.GetSingleton(game).RegisterObjectFunction(game.Input, "Input", "Bind");
            
            GraphicalConsole.GetSingleton(game).WriteLine(string.Format("Loading Level: {0}", levelname), 0);
            game.World = new World(game, levelname, components);
            
            components.Add(game.World);
            

            GraphicalConsole.GetSingleton(game).RegisterObjectFunction(game, "Game", "LoadLevel");


            Cheats cheats = new Cheats(game);
            components.Add(cheats);
            GraphicalConsole.GetSingleton(game).RegisterObjectProperty(cheats, "OneBananaProblem", "GodMode");
            score = new Score();
            
            // set post processing
            game.Renderer.PostProcessType = PostProcessType.BloomAndBlur;

            game.Renderer.Camera = game.World.Players[0].Camera;
            inputFocus = game.World;
            if(welcomeSound == null)
                welcomeSound = game.Sounds.CreateSound("Welcome");
            welcomeSound.Play();
            game.Sounds.MusicBox(MusicBoxStatus.Play);

            TrafficLight trafficLight = new TrafficLight(game, TrafficLightGreenCallback);
            trafficLight.LocalPosition = game.World.Players[0].GlobalPosition + new Vector3(0, 130, -200);
            trafficLight.CanyonSegment = 0;
            game.World.AddObject(trafficLight);
            trafficLight.Start();
            //Gameplayed = true;

            // player shall wait until the trafficLight shows green
            game.World.Players[0].Enabled = false;
        }

        /// <summary>
        /// called by traffic light when it becomes green
        /// </summary>
        public void TrafficLightGreenCallback()
        {
            game.World.Players[0].Enabled = true;
        }

        private ISound welcomeSound;
        public void SetStateDebugMode()
        {
            Reset();

            // Hier können eigene Debug Objekte zum Testen angelegt werden
        }


        // ---------------------------------------------------------------------------
        public IHud Hud
        {
            get { return hud; }
        }

        public IMenu Menu
        {
            get { return menu; }
        }

        public IProfil Profil
        {
            get { return profil; }
        }
        
        public IScore Score
        {
            get { return score; }
        }
    }
}
