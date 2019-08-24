// Zuständigkeit: Martin

#region Using Statements

using System;
using System.Collections.Generic;
using CanyonShooter.Engine;
using CanyonShooter.GameClasses.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion


namespace CanyonShooter.GameClasses.Huds
{
    /// <summary>
    /// Head Up Display. Zeigt Punktestand und eingesammelte Items für alle Spieler an.
    /// Es sollten Sprites verwendet werden. Eine globale Fonts-Klasse zum anzeigen von
    /// Texten wird den Projekt noch hinzugefügt.
    /// </summary>
    public class Hud : DrawableGameComponent, IHud
    {
        private ICanyonShooterGame game;

        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private List<IHudControl> controls = new List<IHudControl>();

        private List<IHudControl> scrollingText = new List<IHudControl>();

        static bool once = false;
        /// <summary>
        /// loads this from a hud description file
        /// </summary>
        /// <param name="name">Which Hud file to load from.</param>
        public Hud(ICanyonShooterGame game, string name)
            : base(game as Game)
        {
            this.game = game;

            this.DrawOrder = (int)DrawOrderType.Hud;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            once = false;

            base.Initialize();
        }

        private int frameCounter = 0;
        private double lastFrame = 0;
        private double minFPScount = 0;
        private double frameCounterStartTime = 0;


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (once == false)
            {
                /*controls.Add(new HudTextControl("debug2", gameTime, "debug2", font, new Vector2(25, 25), 0, Anchor.TOP_LEFT, HUDEffectType.GROW_SHRINK));
                 * */
                // Fadenkreuz
                //controls.Add(new HudSpriteControl("crosshair", gameTime, this.game, new Vector2(32, 32), "Content\\Textures\\Hud\\health", new Vector2(400, 300), 0, Anchor.CENTER, HUDEffectType.NONE));

                //controls.Add(new HudTextControl("TestScrolling0", gameTime, "HUD-CONTROL-STYLE: NONE"    , font, new Vector2(150, 500), new Vector2(150,   0), 25, Anchor.CENTER, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("TestScrolling1", gameTime, "HUD-CONTROL-STYLE: FADE_IN" , font, new Vector2(150, 520), new Vector2(150,  20), 25, Anchor.CENTER, HUDEffectType.FADE_IN));
                //controls.Add(new HudTextControl("TestScrolling2", gameTime, "HUD-CONTROL-STYLE: FADE_OUT", font, new Vector2(150, 540), new Vector2(150,  40), 25, Anchor.CENTER, HUDEffectType.FADE_OUT));
                //controls.Add(new HudTextControl("TestScrolling3", gameTime, "HUD-CONTROL-STYLE: GROW"    , font, new Vector2(150, 560), new Vector2(150,  60), 25, Anchor.CENTER, HUDEffectType.GROW));
                //controls.Add(new HudTextControl("TestScrolling4", gameTime, "HUD-CONTROL-STYLE: SHACKLE" , font, new Vector2(150, 580), new Vector2(150,  80), 25, Anchor.CENTER, HUDEffectType.SHACKE));
                //controls.Add(new HudTextControl("TestScrolling5", gameTime, "HUD-CONTROL-STYLE: SHRINK"  , font, new Vector2(150, 600), new Vector2(150, 100), 25, Anchor.CENTER, HUDEffectType.SHRINK));
                //controls.Add(new HudTextControl("TestScrolling6", gameTime, "HUD-CONTROL-STYLE: PULSE"   , font, new Vector2(150, 620), new Vector2(150, 120), 25, Anchor.CENTER, HUDEffectType.PULSE));


                controls.Add(new HudTextControl("Highscore", gameTime, "", font, new Vector2(0.8f, 0.15f), 0, Anchor.TOP_LEFT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("Multiplikator", gameTime, "", font, new Vector2(0.8f, 0.20f), 0, Anchor.TOP_LEFT, HUDEffectType.NONE));
                
                controls.Add(new HudTextControl("debug1", gameTime, "debug1", font, new Vector2(0.05f, 0.25f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("debug4", gameTime, "debug4", font, new Vector2(0.05f, 0.30f), 0, Anchor.TOP_LEFT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("fps", gameTime, "fps", font, new Vector2(0.95f, 0.25f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));

                controls.Add(new HudSpriteControl("life_sprite", gameTime, this.game, new Vector2(64, 64), "Content\\Textures\\Hud\\health", new Vector2(0.15f, 0.99f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("life_value", gameTime, "100%", font, new Vector2(0.15f, 0.99f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                controls.Add(new HudSpriteControl("life_sprite", gameTime, this.game, new Vector2(64, 64), "Content\\Textures\\Hud\\shield", new Vector2(0.35f, 0.99f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("life_value", gameTime, "100%", font, new Vector2(0.35f, 0.99f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                
                // Current Weapon
                controls.Add(new HudTextControl("weapon1", gameTime, "WAFFE", font, new Vector2(0.65f, 0.99f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("weapon2", gameTime, "WAFFE", font, new Vector2(0.65f, 0.99f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("weapon1_ammo", gameTime, "", font, new Vector2(0.02f, 0.79f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("weapon1_magazine", gameTime, "", font, new Vector2(0.02f, 0.82f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("weapon2_ammo", gameTime, "", font, new Vector2(0.02f, 0.85f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                controls.Add(new HudTextControl("weapon2_magazine", gameTime, "", font, new Vector2(0.02f, 0.88f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));

                controls.Add(new HudTextControl("MusicTitle", gameTime, game.Sounds.GetMusicName(), font, new Vector2(0.89f, 0.97f), 10, Anchor.CENTER, HUDEffectType.GROW | HUDEffectType.FADE_OUT));
                
                once = true;

                foreach (IHudControl x in controls)
                {
                    x.Resolution = new Vector2(game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height);
                }
            }


            if (game.Input.IsKeyDown("Menu.Ok"))
            {
                this.DisplayScrollingText("VERSUCH JO!", gameTime);
            }

                /*
                // Objekt finden und durch hSC referenzieren:
                IHudSpriteControl hSC = ((IHudSpriteControl)this.GetControl("crosshair")); // das "this" anpassen!
                // Wirklich gefunden?
                if (hSC != null)
                {
                    // Position verändern:
                    hSC.Position += new Vector2(1, 0);
                }

                HudTextControl hTC = ((HudTextControl)this.GetControl("debug1"));
                if (hTC != null)
                    hTC.Text = "Menu.Ok: down";
                GraphicalConsole.GetSingleton(game).WriteLine("Menu.Ok down", 1);
            }
            else
            {
                HudTextControl hTC = ((HudTextControl)this.GetControl("debug1"));
                if (hTC != null)
                    hTC.Text = "Menu.Ok: up";
            }*/

            //  Highscore und FPS setzen
            foreach (HudControl x in controls)
            {
                x.Update(gameTime);
                HudTextControl text = x as HudTextControl;
                if (text != null)
                {
                    if (text.Name == "Highscore")
                    {
                        text.Text = "Highscore: " + game.GameStates.Score.Highscore;
                    }
                    if (text.Name == "Multiplikator")
                    {
                        text.Text = "Multiplikator: " + game.GameStates.Score.Multiplikator + "x";
                    }

                    if (text.Name == "fps")
                    {
                        // Min-FPS Counter
                        double time = gameTime.TotalRealTime.TotalSeconds;
                        double FPScount = 0;
                        double mdiff;
                        if (lastFrame != 0)
                        {
                            mdiff = time - lastFrame;
                            FPScount = 1 / mdiff;
                            if (mdiff != 0 && FPScount < minFPScount) minFPScount = FPScount;
                        }
                        lastFrame = time;

                        // init
                        if (frameCounterStartTime == 0) frameCounterStartTime = gameTime.TotalRealTime.TotalSeconds;

                        // frames zählen
                        frameCounter++;

                        // sind 0.5 sekunden vergangen?
                        double diff = gameTime.TotalRealTime.TotalSeconds - frameCounterStartTime;
                        if (diff > 0.5)
                        {
                            // messergebnis ausgeben
                            //    text.Text = Math.Round((frameCounter / diff), 2) + " fps (" + Math.Round(minFPScount,1)+ " min)";
                            text.Text = Math.Round(minFPScount, 1) + " FPS";    // Changed by Flo

                            minFPScount = double.PositiveInfinity; // Min-FPS Counter

                            // zähler zurücksetzen
                            frameCounter = 0;

                            // startzeit für die nächste messung
                            frameCounterStartTime = gameTime.TotalRealTime.TotalSeconds;
                        }
                    } // fps
                }
            }

            foreach (HudTextControl x in scrollingText)
            {
                x.Update(gameTime);
            }

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
        public override void Draw(GameTime gameTime)
        {
            if (spriteBatch != null)
            {
                spriteBatch.Begin();
                foreach (HudControl x in this.controls)
                {
                    HudTextControl text = x as HudTextControl;
                    HudSpriteControl sprite = x as HudSpriteControl;
                    Hud3DControl model = x as Hud3DControl;
                    if (text != null)
                        text.Draw(spriteBatch, font);
                    if (sprite != null)
                        sprite.Draw(spriteBatch);
                    if (model != null)
                        model.Draw(spriteBatch);
                }
                foreach (HudTextControl x in this.scrollingText)
                {
                    if (x != null)
                        x.Draw(spriteBatch, font);
                }
                spriteBatch.End();
            }

        }

        //
        // Zusammenfassung:
        //     Called when the component needs to load graphics resources.  Override this
        //     method to load any component-specific graphics resources.
        //
        // Parameter:
        //   loadAllContent:
        //     true if all graphics resources need to be loaded; false if only manual resources
        //     need to be loaded.
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(game.Graphics.Device);

            font = game.Content.Load<SpriteFont>("Arial");
        }
       
        #region IHud Member

        public string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IHudControl GetControl(string name)
        {
            foreach (IHudControl x in this.controls)
            {
                if (x.Name == name)
                    return x;
            }
            return null;
        }

        public bool ExistsControl(string name)
        {
            foreach (IHudControl x in this.controls)
            {
                if (x.Name == name)
                    return true;
            }
            return false;
        }

        public bool CreateTextControl(string name, Vector2 pos)
        {
            throw new Exception("Not Implemented yet!");
            //return true;
        }

        public bool CreateSpriteControl(string name, string texture, Vector2 pos, Vector2 size)
        {
            // Temporary solution by Manuel Rodriguez.
            IHudSpriteControl sc = new HudSpriteControl(name, new GameTime(), this.game, size, texture, pos, 0, Anchor.CENTER, HUDEffectType.NONE);
            controls.Add(sc);
            if(controls.Contains(sc))
            {
                return true;
            }
            return false;
            //throw new Exception("Not Implemented yet!");
            //return true;
        }

        public void DisplaySoundTitle(string name)
        {
            IHudTextControl hTC = ((IHudTextControl)this.GetControl("MusicTitle"));
            if (hTC != null)
            {
                hTC.Text = name;
                hTC.TimeLiving = 0;
                hTC.Visible = true;
            }
        }

        public void DisplayScrollingText(string text, GameTime time)
        {
            foreach (IHudTextControl x in scrollingText)
            {
                if (x.Visible == false)
                {
                    x.Text = text;
                    x.Visible = true;
                    x.TimeLiving = 0;
                    return;
                }
            }
            HudTextControl tc = new HudTextControl("ScrollingText#" + scrollingText.Count, time, text, font, new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.2f), 4, Anchor.CENTER, HUDEffectType.NONE);
            tc.Resolution = new Vector2(game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height);
            scrollingText.Add(tc);
        }

        public void DisplayCountdown(int number)
        {
        }

        #endregion
    }
}


