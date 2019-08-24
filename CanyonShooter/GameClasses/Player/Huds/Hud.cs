// Zuständigkeit: Martin

#region Using Statements

using System;
using System.Collections.Generic;
using CanyonShooter.Engine;
using CanyonShooter.GameClasses.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CanyonShooter.GameClasses.Weapons;
using CanyonShooter.Engine.Helper;
using CanyonShooter.Engine.Audio;

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
        private ISound alert;
        private readonly Dictionary<string,IHudControl> controls = new Dictionary<string,IHudControl>();

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
            alert = game.Sounds.CreateSound("Alert");
            
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
                #region oldControls
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

                //controls.Add(new HudBarControl("testbar1", gameTime, game, font, "Content\\Textures\\Hud\\bar", new Vector2(0.75f, 0.99f), new Vector2(0.266666f, 0.2f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE, "Health", game.World.Players[0], "Health: {0}"));
                //HudBarControl hBC = ((HudBarControl)this.GetControl("testbar1"));
                //if (hBC != null)
                //    hBC.MinRect = new Rectangle(0,0,0,100);


                //controls.Add(new HudTextControl("Highscore", gameTime, font, new Color(255, 255, 0), new Vector2(0.8f, 0.15f), 0, Anchor.TOP_LEFT, HUDEffectType.NONE, "Highscore", game.GameStates.Score, "Highscore: {0}"));
                ////controls.Add(new HudTextControl("Highscore", gameTime, "", font, new Color(255,255,0), new Vector2(0.8f, 0.15f), 0, Anchor.TOP_LEFT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("Multiplikator", gameTime, font, new Color(255, 255, 0), new Vector2(0.8f, 0.20f), 0, Anchor.TOP_LEFT, HUDEffectType.NONE, "Multiplikator", game.GameStates.Score, "Multiplikator: {0}x"));
                
                //controls.Add(new HudTextControl("debug1", gameTime, "debug1", font, new Color(255,0,0), new Vector2(0.05f, 0.25f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("debug4", gameTime, "debug4", font, new Color(255,0,0), new Vector2(0.05f, 0.30f), 0, Anchor.TOP_LEFT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("fps", gameTime, "fps", font, new Color(255,255,0), new Vector2(0.95f, 0.25f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));

                //controls.Add(new HudSpriteControl("health_sprite", gameTime, this.game, new Vector2(0.1f, 0.13333f), "Content\\Textures\\Hud\\health", new Vector2(0.15f, 0.99f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("health_value", gameTime, font, new Color(0,0,255), new Vector2(0.15f, 0.99f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE, "Health", game.World.Players[0], "HealthXX: {0}"));
                //controls.Add(new HudSpriteControl("shield_sprite", gameTime, this.game, new Vector2(0.1f, 0.13333f), "Content\\Textures\\Hud\\shield", new Vector2(0.35f, 0.99f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("shield_value", gameTime, "100%", font, new Color(0,0,255), new Vector2(0.35f, 0.99f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                
                //// Current Weapon
                //controls.Add(new HudTextControl("weapon1", gameTime, "WAFFE", font, new Color(127,0,0), new Vector2(0.65f, 0.99f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("weapon2", gameTime, "WAFFE", font, new Color(127, 0, 0), new Vector2(0.65f, 0.99f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("weapon1_ammo", gameTime, "", font, new Color(127, 0, 0), new Vector2(0.02f, 0.79f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("weapon1_magazine", gameTime, "", font, new Color(127, 0, 0), new Vector2(0.02f, 0.82f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("weapon2_ammo", gameTime, "", font, new Color(127, 0, 0), new Vector2(0.02f, 0.85f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));
                //controls.Add(new HudTextControl("weapon2_magazine", gameTime, "", font, new Color(127, 0, 0), new Vector2(0.02f, 0.88f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE));

                //controls.Add(new HudTextControl("MusicTitle", gameTime, game.Sounds.GetMusicName(), font, new Color(255, 255, 0), new Vector2(0.89f, 0.97f), 10, Anchor.CENTER, HUDEffectType.GROW | HUDEffectType.FADE_OUT));
                
                
                // funzt nicht so wie es soll.
                //controls.Add(new HudBarControl("Health.Bar", gameTime, game, font, "Content/Textures/Hud/HUD2-HEALTHBAR", new Vector2(0.26f, 0.965f), new Vector2(0.1f, 0.1f), 0, Anchor.BOTTOM_LEFT, HUDEffectType.NONE, "Health", game.World.Players[0], "Health: {0}"));
                //HudBarControl healthBar = ((HudBarControl)this.GetControl("Health.Bar"));
                //if (healthBar != null)
                //{
                //    healthBar.MinRect = new Rectangle(0, 0, 100, 0);
                //    healthBar.MaxRect = new Rectangle(0,0,100,100);
                //}
                #endregion

                #region HUD2: Init

                // Misc
                AddControl(new HudTextControl("FPS", gameTime, "0", hudFont, Color.Gray, new Vector2(0.95f, 0.25f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));

                // Player
                AddControl(new HudTextControl("Ammo.LaserCells",gameTime,"999999",hudFont,hudLcdColor,new Vector2(0.345f,0.951f),0,Anchor.BOTTOM_LEFT,HUDEffectType.NONE ));
                AddControl(new HudTextControl("Ammo.Rockets", gameTime, "999999", hudFont, hudLcdColor, new Vector2(0.442f, 0.951f), 0, Anchor.BOTTOM_CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("Ammo.Bullets", gameTime, "999999", hudFont, hudLcdColor, new Vector2(0.542f, 0.951f), 0, Anchor.BOTTOM_CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("Ammo.PlasmaBalls", gameTime, "999999", hudFont, hudLcdColor, new Vector2(0.641f, 0.951f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));

                // Score
                AddControl(new HudTextControl("Score.Highscore", gameTime, "0", hudFontHuge, new Color(255, 255, 255), new Vector2(0.975f, 0.014f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));
                AddControl(new HudTextControl("Score.Kills", gameTime, "0", hudFont, new Color(255, 255, 255), new Vector2(0.746f, 0.012f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));
                AddControl(new HudTextControl("Score.Multiplikator", gameTime, "0", hudFont, new Color(255, 255, 255), new Vector2(0.746f, 0.0568f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));

                // Player
                AddControl(new HudTextControl("Player.Distance", gameTime, "0", hudFont, new Color(255, 255, 255), new Vector2(0.682f, 0.012f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));
                AddControl(new HudTextControl("Player.Time", gameTime, "0", hudFont, new Color(255, 255, 255), new Vector2(0.682f, 0.0568f), 0, Anchor.TOP_RIGHT, HUDEffectType.NONE));
                AddControl(new HudTextControl("Player.Speed", gameTime, "0MPH", hudFontSpeed, hudLcdColor, new Vector2(0.255f, 0.898f), 0, Anchor.BOTTOM_RIGHT, HUDEffectType.NONE));
                AddControl(new HudTextControl("Player.Health", gameTime, "0", hudFont, Color.Gray, new Vector2(0.038f, 0.852f), 0, Anchor.BOTTOM_CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("Player.Shield", gameTime, "0", hudFont, Color.Gray, new Vector2(0.1025f, 0.852f), 0, Anchor.BOTTOM_CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("Player.Lifes", gameTime, "0", hudFontMedium, Color.Gray, new Vector2(0.957f, 0.97f), 0, Anchor.BOTTOM_CENTER, HUDEffectType.NONE));

                AddControl(new HudTextControl("Weapon1KeyInfo", gameTime, "1", hudFont, Color.Gray, new Vector2(0.78f, 0.99f), 0, Anchor.CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("Weapon2KeyInfo", gameTime, "2", hudFont, Color.Gray, new Vector2(0.799f, 0.99f), 0, Anchor.CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("Weapon3KeyInfo", gameTime, "3", hudFont, Color.Gray, new Vector2(0.895f, 0.99f), 0, Anchor.CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("Weapon4KeyInfo", gameTime, "4", hudFont, Color.Gray, new Vector2(0.914f, 0.99f), 0, Anchor.CENTER, HUDEffectType.NONE));

                AddControl(new HudTextControl("PrimaryWeapon.MagazineInfo", gameTime, "", hudFont, Color.Gray, new Vector2(0.756f, 0.974f), 0, Anchor.CENTER, HUDEffectType.NONE));
                AddControl(new HudTextControl("SecondaryWeapon.MagazineInfo", gameTime, "", hudFont, Color.Gray, new Vector2(0.869f, 0.974f), 0, Anchor.CENTER, HUDEffectType.NONE));
                
                RescaleFonts();
                #endregion

                once = true;

                foreach (IHudControl x in controls.Values)
                {
                    x.Resolution = new Vector2(game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height);
                }
            }


            #region HUD2: Update

            // Update Ammo-Data:
            
            ((HudTextControl) controls["Ammo.LaserCells"]).Text =
                ((Player2) game.World.Players[0]).Weapons.GetAmmo(AmmoType.LASERCELLS).ToString();
            ((HudTextControl)controls["Ammo.Rockets"]).Text =
                ((Player2)game.World.Players[0]).Weapons.GetAmmo(AmmoType.ROCKETS).ToString();
            ((HudTextControl)controls["Ammo.Bullets"]).Text =
                ((Player2)game.World.Players[0]).Weapons.GetAmmo(AmmoType.BULLETS).ToString();
            ((HudTextControl) controls["Ammo.PlasmaBalls"]).Text =
                ((Player2) game.World.Players[0]).Weapons.GetAmmo(AmmoType.PLASMABALLS).ToString();

            //TODO: Display Primary/Secondary Weapon
            //TODO: Display max and current ammo in weapon

            //TODO: ...

            // Update Player-Data:

            ((HudTextControl)controls["Player.Health"]).Text =
                ((IPlayer)game.World.Players[0]).Health.ToString();
            ((HudTextControl)controls["Player.Shield"]).Text =
                ((IPlayer)game.World.Players[0]).Shield.ToString();
            ((HudTextControl)controls["Player.Time"]).Text =
                string.Format("{0} : {1}", ((Player2)game.World.Players[0]).RemainingTime / 60, ((Player2)game.World.Players[0]).RemainingTime % 60);
            ((HudTextControl)controls["Player.Speed"]).Text =
                ((IPlayer)game.World.Players[0]).Speed.ToString() + "MPH";
            ((HudTextControl)controls["Player.Distance"]).Text = String.Format("{0:F1}", game.World.Players[0].Distance);
            ((HudTextControl)controls["Player.Lifes"]).Text =
                ((IPlayer)game.World.Players[0]).Lifes.ToString();

            // Update CrossHairs:
            if (((Player2)game.World.Players[0]).Weapons.PrimaryWeapon != null)
                currentCrossHair = ((Player2) game.World.Players[0]).Weapons.PrimaryWeapon.GetCrossHair();


            // Update Score-Data:
            ((HudTextControl) controls["Score.Highscore"]).Text =
                game.GameStates.Score.Highscore.ToString();
            ((HudTextControl)controls["Score.Multiplikator"]).Text =
                 string.Format("{0}x", game.GameStates.Score.Multiplikator);
            ((HudTextControl)controls["Score.Kills"]).Text =
                 game.GameStates.Score.KilledEnemy.ToString();

            // Update Booster Warning:
            if (((IPlayer)game.World.Players[0]).BoosterHeat >= 100)
                showWarning = true;
            else if (((IPlayer)game.World.Players[0]).BoosterHeat <= 80)
                showWarning = false;


            #region Update WeaponInfos:
            // Weapon1
            if (((Player2)game.World.Players[0]).Weapons.HasWeapon(WeaponType.ULTRA_PHASER))
                ((HudTextControl) controls["Weapon1KeyInfo"]).Visible = true;
            else
                ((HudTextControl) controls["Weapon1KeyInfo"]).Visible = false;
            if (((Player2)game.World.Players[0]).Weapons.PrimaryWeapon != null)
            if (((Player2)game.World.Players[0]).Weapons.PrimaryWeapon.GetWeaponType() == WeaponType.ULTRA_PHASER)
                ((HudTextControl)controls["Weapon1KeyInfo"]).TextColor = Color.White;
            else
                ((HudTextControl)controls["Weapon1KeyInfo"]).TextColor = Color.Gray;

            // Weapon2
            if (((Player2)game.World.Players[0]).Weapons.HasWeapon(WeaponType.MINIGUN))
                ((HudTextControl)controls["Weapon2KeyInfo"]).Visible = true;
            else
                ((HudTextControl)controls["Weapon2KeyInfo"]).Visible = false;
            if (((Player2)game.World.Players[0]).Weapons.PrimaryWeapon != null)
            if (((Player2)game.World.Players[0]).Weapons.PrimaryWeapon.GetWeaponType() == WeaponType.MINIGUN)
                ((HudTextControl)controls["Weapon2KeyInfo"]).TextColor = Color.White;
            else
                ((HudTextControl)controls["Weapon2KeyInfo"]).TextColor = Color.Gray;
            
            // Weapon2
            if (((Player2)game.World.Players[0]).Weapons.HasWeapon(WeaponType.ROCKET_STINGER))
                ((HudTextControl)controls["Weapon3KeyInfo"]).Visible = true;
            else
                ((HudTextControl)controls["Weapon3KeyInfo"]).Visible = false;
            if (((Player2)game.World.Players[0]).Weapons.SecondaryWeapon != null)
            if (((Player2)game.World.Players[0]).Weapons.SecondaryWeapon.GetWeaponType() == WeaponType.ROCKET_STINGER)
                ((HudTextControl)controls["Weapon3KeyInfo"]).TextColor = Color.White;
            else
                ((HudTextControl)controls["Weapon3KeyInfo"]).TextColor = Color.Gray;

            // Weapon4
            if (((Player2)game.World.Players[0]).Weapons.HasWeapon(WeaponType.PLASMAGUN))
                ((HudTextControl)controls["Weapon4KeyInfo"]).Visible = true;
            else
                ((HudTextControl)controls["Weapon4KeyInfo"]).Visible = false;
            if (((Player2)game.World.Players[0]).Weapons.SecondaryWeapon != null)
            if (((Player2)game.World.Players[0]).Weapons.SecondaryWeapon.GetWeaponType() == WeaponType.PLASMAGUN)
                ((HudTextControl)controls["Weapon4KeyInfo"]).TextColor = Color.White;
            else
                ((HudTextControl)controls["Weapon4KeyInfo"]).TextColor = Color.Gray;

            if (((Player2)game.World.Players[0]).Weapons.PrimaryWeapon != null)
                ((HudTextControl) controls["PrimaryWeapon.MagazineInfo"]).Text =
                    ((Player2) game.World.Players[0]).Weapons.PrimaryWeapon.GetMagazineInfo();

            if (((Player2)game.World.Players[0]).Weapons.SecondaryWeapon != null)
                ((HudTextControl)controls["SecondaryWeapon.MagazineInfo"]).Text =
                    ((Player2)game.World.Players[0]).Weapons.SecondaryWeapon.GetMagazineInfo();

            #endregion

            #endregion

            #region hmmmmmm
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

            #endregion

            #region FPS
            if (controls.ContainsKey("FPS"))
            {
                HudTextControl text = controls["FPS"] as HudTextControl;
                if(text == null)
                    return;
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
            }
            #endregion

            foreach (HudTextControl x in scrollingText)
            {
                x.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private void RescaleFonts()
        {
            foreach (IHudControl control in controls.Values)
            {
                if(control is HudTextControl)
                    ((HudTextControl)control).Scaling = game.Graphics.ScreenHeight / 768f;
            }

        }

        #region HUD2: Vars

        private Texture2D currentCrossHair;


        private Texture2D hudBack;
        private Texture2D hudFront;
        private Texture2D hudHealthBar;
        private Texture2D hudShieldBar;
        private Texture2D hudBoostBar;
        private Texture2D hudBoosterWarning;
        private Texture2D hudFuelBar;
        private bool showWarning = false;
        private bool warningVisible = false;
        private int warningFpsCount = 0;
        private int warningGlowCount = 3;

        private SpriteFont hudFont;
        private SpriteFont hudFontMedium;
        private SpriteFont hudFontHuge;
        private SpriteFont hudFontSpeed;

        private Color hudLcdColor = new Color(41,213,16);

        #endregion



        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.  Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime).</param>
        public override void Draw(GameTime gameTime)
        {
            if (spriteBatch != null)
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState, 
                    Matrix.CreateScale(game.Graphics.ScreenWidth, game.Graphics.ScreenHeight, 1));
             
                // DRAW HUD2 - BACK LAYER
                spriteBatch.Draw(hudBack,new Rectangle(0,0,1,1),Color.White);
                spriteBatch.End();

                
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None);
                // HEALTH, SHIELD 
                DrawVerticalBarAt(hudHealthBar, new Vector2(0.027f, 0.829f), game.World.Players[0].Health,100);
                DrawVerticalBarAt(hudShieldBar, new Vector2(0.09f, 0.829f), game.World.Players[0].Shield,250);
                // BOOST
                DrawHorizontalBarAt(hudBoostBar, new Vector2(0.144f, 0.932f), game.World.Players[0].BoosterHeat, 100);
                // FUEL
                DrawVerticalBarAt(hudFuelBar, new Vector2(0.007f, 0.128f), (int)((Player2)game.World.Players[0]).Fuel, (int)((Player2)game.World.Players[0]).MaxFuel);

                #region HUD-Controls

                // DRAW CONTROLS
                foreach (HudControl x in controls.Values)
                {
                    HudTextControl text = x as HudTextControl;
                    HudSpriteControl sprite = x as HudSpriteControl;
                    Hud3DControl model = x as Hud3DControl;
                    HudBarControl bar = x as HudBarControl;
                    if (text != null)
                        text.Draw(spriteBatch);
                    if (sprite != null)
                        sprite.Draw(spriteBatch);
                    if (model != null)
                        model.Draw(spriteBatch);
                    if (bar != null)
                        bar.Draw(spriteBatch, font);
                   
                }
                foreach (HudTextControl x in this.scrollingText)
                {
                    if (x != null)
                        x.Draw(spriteBatch);
                }

                spriteBatch.End();
                #endregion

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState,
                   Matrix.CreateScale(game.Graphics.ScreenWidth, game.Graphics.ScreenHeight, 1));




                // DRAW HUD2 - FRONT LAYER
                spriteBatch.Draw(hudFront, new Rectangle(0, 0, 1,1), Color.White);

                if (currentCrossHair != null)
                {
                    float scale = 4f;
                    Vector2 crossHairPosition = new Vector2(
                        0.5f - ((currentCrossHair.Width) * (0.05f * scale / game.Graphics.ScreenWidth)),
                        0.55f - ((currentCrossHair.Height) * (0.05f * scale / game.Graphics.ScreenHeight)));

                    spriteBatch.Draw(currentCrossHair,crossHairPosition,
                                     new Rectangle(0, 0, currentCrossHair.Width, currentCrossHair.Height), Color.White,
                                     0, new Vector2(0.5f, 0.5f),
                                     new Vector2(0.1f * scale / game.Graphics.ScreenWidth, 0.1f * scale / game.Graphics.ScreenHeight),
                                     SpriteEffects.None, 0.5f);
                }


                if (((Player2)game.World.Players[0]).Weapons.PrimaryWeapon != null)
                {
                    Texture2D icon = ((Player2) game.World.Players[0]).Weapons.PrimaryWeapon.Icon;
                    spriteBatch.Draw(icon,new Vector2(0.703f,0.924f),
                                     new Rectangle(0, 0, icon.Width, icon.Height), Color.White,
                                     0, new Vector2(0.5f, 0.5f),
                                     new Vector2(1f / game.Graphics.ScreenWidth, 1f / game.Graphics.ScreenHeight),
                                     SpriteEffects.None, 0.5f);
                }

                if (((Player2)game.World.Players[0]).Weapons.SecondaryWeapon != null)
                {
                    Texture2D icon = ((Player2)game.World.Players[0]).Weapons.SecondaryWeapon.Icon;
                    spriteBatch.Draw(icon, new Vector2(0.818f, 0.924f),
                                     new Rectangle(0, 0, icon.Width, icon.Height), Color.White,
                                     0, new Vector2(0.5f, 0.5f),
                                     new Vector2(1f / game.Graphics.ScreenWidth, 1f / game.Graphics.ScreenHeight),
                                     SpriteEffects.None, 0.5f);
                }

                #region BOOSTER-WARNING

                if (showWarning)
                {
                    if(!alert.Playing()) alert.Play();
                    warningGlowCount--;

                    if (warningGlowCount <= 0)
                        warningFpsCount++;
                    if (warningFpsCount > 8)
                    {
                        warningVisible = !warningVisible;
                        warningFpsCount = 0;
                        warningGlowCount = 3;
                    }

                    if (warningVisible)
                    {
                        if (warningGlowCount > 0)
                            spriteBatch.Draw(hudBoosterWarning,
                                         new Rectangle(0, 0, 1, 1),
                                         new Color(255, 255, 255, (byte)(255 / warningGlowCount)));
                        else
                            spriteBatch.Draw(hudBoosterWarning,
                                         new Rectangle(0, 0, 1, 1),
                                         new Color(255, 255, 255, 255));
                    }
                }
                else 
                {
                    alert.Stop();
                }
                #endregion

                spriteBatch.End();

                


            }

        }

        private void DrawVerticalBarAt(Texture2D barTexture, Vector2 at, int value, int maxvalue)
        {
            if (value > maxvalue)
                value = maxvalue;

            int resX = game.Graphics.ScreenWidth;
            int resY = game.Graphics.ScreenHeight;

            float percent = maxvalue * 0.01f;
            float value2 = (value/percent);

            float scaledBarSizeX = (barTexture.Width * (resX / 1024f));
            float scaledBarSizeY = (barTexture.Height * (resY / 768f));

            float posX = (at.X*resX);
            float posY = (at.Y*resY) + (scaledBarSizeY *((100-value2)*0.01f));
            float width = scaledBarSizeX;
            float height = (scaledBarSizeY * (value2 * 0.01f));

            spriteBatch.Draw(barTexture,
                new Rectangle((int)posX,(int)posY,(int) width,(int) height),
                new Rectangle(0, barTexture.Height - (int)(barTexture.Height * (value2 / 100f)), barTexture.Width, (int)(barTexture.Height * (value2 / 100f))),
                Color.White);

            //spriteBatch.Draw(barTexture,
            //    new Rectangle((int)(at.X * resX),(int)(barTexture.Height *(resY / 768f) +((at.Y * resY) * (resY / 768f)) + (int)(barTexture.Height - (int)(barTexture.Height * (value2 / 100)) * (resY / 768f))), (int)(barTexture.Width * (resX / 1024f)), (int)((barTexture.Height * (value2 / 100)) * (resY / 768f))),
            //    new Rectangle(0, barTexture.Height - (int)(barTexture.Height * (value2 / 100)), barTexture.Width, (int)(barTexture.Height * (value2 / 100))),
            //    Color.White);

        }

        private void DrawHorizontalBarAt(Texture2D barTexture, Vector2 at, int value, int maxvalue)
        {

            if (value > maxvalue)
                value = maxvalue;

            int resX = game.Graphics.ScreenWidth;
            int resY = game.Graphics.ScreenHeight;

            float percent = maxvalue / 100f;
            float value2 = value / percent;

            spriteBatch.Draw(barTexture,
                new Rectangle((int)(at.X * resX), (int)(at.Y * resY), (int)(((barTexture.Width * (resX / 1024f)) / 100) * (int)value2), (int)(barTexture.Height * (resY / 768f))),
                new Rectangle(0, 0, (int)(barTexture.Width * (value2 / 100)), barTexture.Height),
                Color.White);

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

            hudBack = game.Content.Load<Texture2D>("Content/Textures/Hud/HUD2-BACK");
            hudFront = game.Content.Load<Texture2D>("Content/Textures/Hud/HUD2-FRONT");
            hudHealthBar = game.Content.Load<Texture2D>("Content/Textures/Hud/HUD2-HEALTHBAR");
            hudShieldBar = game.Content.Load<Texture2D>("Content/Textures/Hud/HUD2-SHIELDBAR");
            hudBoostBar = game.Content.Load<Texture2D>("Content/Textures/Hud/HUD2-BOOSTBAR");
            hudBoosterWarning = game.Content.Load<Texture2D>("Content/Textures/Hud/HUD2-BOOSTER-WARNING");
            hudFuelBar = game.Content.Load<Texture2D>("Content/Textures/Hud/HUD2-FUELBAR");
            hudFont = game.Content.Load<SpriteFont>("HudFont.Small");
            hudFontMedium = game.Content.Load<SpriteFont>("HudFont.Medium");
            hudFontHuge = game.Content.Load<SpriteFont>("HudFont.Huge");
            hudFontSpeed = game.Content.Load<SpriteFont>("HudFont.Speed");

            // Rescale all Fonts to the current resolution
            RescaleFonts();
        }
       
        #region IHud Member

        public string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IHudControl GetControl(string name)
        {
            if(controls.ContainsKey(name))
                return controls[name];
            return null;
        }

        public bool ExistsControl(string name)
        {
            return controls.ContainsKey(name);
        }

        public void AddControl(IHudControl control)
        {
            controls.Add(control.Name,control);
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
            AddControl(sc);
            sc.Resolution = new Vector2(game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height);
            if(controls.ContainsKey(sc.Name))
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
            HudTextControl tc = new HudTextControl("ScrollingText#" + scrollingText.Count, time, text, font, new Color(255,255,255),new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.2f), 4, Anchor.CENTER, HUDEffectType.NONE);
            tc.Resolution = new Vector2(game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height);
            scrollingText.Add(tc);
        }

        public void DisplayCountdown(int number)
        {
        }

        #endregion
    }
}


