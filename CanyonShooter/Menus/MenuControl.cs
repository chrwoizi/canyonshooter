using Microsoft.Xna.Framework.Input;
using CanyonShooter.Engine.Audio;
using System;
using DifficultEnum = CanyonShooter.Menus.Menu.DifficultEnum;
using DistrictEnum = CanyonShooter.Menus.Option.DistrictEnum;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Menus
{
    /// <summary>
    /// Control of the Menu
    /// </summary>
    class MenuControl
    {
        #region Data Member

        private Menu menu;

        private ICanyonShooterGame game;

        private Prompt prompt;

        public Prompt Prompt
        {
            get { return prompt; }
            set { prompt = value; }
        }
        #endregion

        /// <summary>
        /// Constructor of the MenuControl
        /// </summary>
        public MenuControl(Menu menu, ICanyonShooterGame game)
        {
            this.menu = menu;
            this.game = game;
        }

        #region Steuerung

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
        /// Chosse Button with Keyboard
        /// </summary>
        public void changeButton(Keys taste)
        {
            if (!isMouseInShift() ||
                new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) == game.GameStates.Menu.Mouse)
            {
                if (taste == Keys.Left)
                {
                    menu.Shifts.LeftShift.SetActive();
                    menu.Shifts.RightShift.resetActive();
                    // Nice sound if the button has been pressed. by M.R.
                    PlayButtonSound();
                    // ------------------------------------------
                    menu.CurrentList.Previous();
                }
                else if (taste == Keys.Right)
                {
                    menu.Shifts.RightShift.SetActive();
                    menu.Shifts.LeftShift.resetActive();
                    // Nice sound if the button has been pressed. by M.R.
                    PlayButtonSound();
                    // ------------------------------------------
                    menu.CurrentList.Next();
                }
            }
        }

        /// <summary>
        /// Choose Button with Mouse
        /// </summary>
        public void changeButton()
        {
            if (menu.Shifts.LeftShift.getPosition() == menu.MouseInRec)
            {
                // Nice sound if the button has been pressed. by M.R.
                PlayButtonSound();
                // ------------------------------------------
                menu.CurrentList.Previous();
            }
            else if (menu.Shifts.RightShift.getPosition() == menu.MouseInRec)
            {
                // Nice sound if the button has been pressed. by M.R.
                PlayButtonSound();
                // ------------------------------------------
                menu.CurrentList.Next();
            }
        }

        /// <summary>
        /// Go back in Menu with Escape
        /// </summary>
        /// <param name="menuName">Current menu</param>
        public void Back(string menuName)
        {
            switch (menuName)
            {
                case "Optionen":
                    {
                        menu.CurrentMenu = "Hauptmenu";
                        menu.CurrentList.init("Hauptmenu", true);
                        game.GameStates.Profil.CurrentProfil.Save("Profils\\Profil"
                            + game.GameStates.Profil.CurrentNumber + ".xml");
                        break;
                    }
                case "Highscore":
                    {
                        menu.CurrentMenu = "Hauptmenu";
                        menu.CurrentList.init("Hauptmenu", true);
                        break;
                    }
                default:
                    break;
            }
        }

        public void BackHighscore() 
        {
            menu.CurrentMenu = "Highscore";
            menu.HighscoreMenu.Visiblity = false;
            menu.HighscoreMenu = null;
            menu.Visiblity = true;
        }

        /// <summary>
        /// Execution of Player´s choise in HighscoreMenu and Option
        /// </summary>
        public void selection(string ButtonName, string SubMenu)
        {
            switch (SubMenu)
            {
                #region Highscore
                case "Highscore":
                    {
                        if (ButtonName.Equals("Löschen"))
                        {
                            prompt = new Prompt(game, "Reset", game.Graphics.Device.Viewport.Width, 
                                game.Graphics.Device.Viewport.Height, 
                                new string[] { "Delete the Highscorestates?" }, false);
                            prompt.accept += new PromptEventHandler(HSReset);
                            prompt.decline += new PromptEventHandler(nothing);
                            prompt.intiateButtons();
                            menu.HighscoreMenu.Active = false;
                        }
                        else
                        {
                            menu.CurrentMenu = "Highscore";
                            menu.HighscoreMenu = null;
                            menu.Visiblity = true;
                        }
                        break;
                    }
                #endregion

                #region Optionen
                case "Optionen":
                    {
                        switch (ButtonName)
                        {
                            case "Akzeptieren":
                                {
                                     OptionList changer = (OptionList)menu.OptionMenu.Dialog.list;
                                     if (changer.controlChanges())
                                     {
                                         prompt = new Prompt(game, "Save", game.Graphics.Device.Viewport.Width,
                                             game.Graphics.Device.Viewport.Height,
                                             new string[] { "Save the values?" }, false);
                                         prompt.accept += new PromptEventHandler(Save);
                                         prompt.decline += new PromptEventHandler(nothingOpt);
                                         prompt.intiateButtons();
                                         menu.OptionMenu.Activity = false;
                                     }
                                     else
                                     {
                                        discardChanges();
                                     }
                                    break;
                                }
                            case "Ablehnen":
                                {
                                    discardChanges();
                                    break;
                                }
                            case "Playername":
                                {
                                    prompt = new Prompt(game, "Playername", game.Graphics.Device.Viewport.Width,
                                        game.Graphics.Device.Viewport.Height,
                                        new string[] { "Please insert a playername:" }, false,
                                        (string)menu.OptionMenu.Dialog.Data["Playername"], true);
                                    prompt.accept += new PromptEventHandler(SetPlayername);
                                    prompt.decline += new PromptEventHandler(nothingOpt);
                                    prompt.intiateButtons();
                                    menu.OptionMenu.Activity = false;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case "OptionenControl":
                    {
                        switch (ButtonName)
                        {
                            case "Akzeptieren":
                                {
                                    OptionList changer = (OptionList)menu.OptionMenu.Listbox.List;
                                    if (changer.controlChanges())
                                    {
                                        prompt = new Prompt(game, "Save", game.Graphics.Device.Viewport.Width,
                                            game.Graphics.Device.Viewport.Height,
                                            new string[] { "Save the values?" }, false);
                                        prompt.accept += new PromptEventHandler(SaveControlSettings);
                                        prompt.decline += new PromptEventHandler(nothingOpt);
                                        prompt.intiateButtons();
                                        menu.OptionMenu.Activity = false;
                                    }
                                    else
                                    {
                                        discardChanges();
                                    }
                                    break;
                                }
                            case "Ablehnen":
                                {
                                    discardChanges();
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                #endregion
                default:
                    break;
            }
        }

        #region EventMethods

        public void HSReset()
        {
            game.Highscores.Reset(menu.HighscoreMenu.Difficult);
            prompt.unloadPrompt();
            menu.HighscoreMenu.Active = true;
        }

        public void SaveControlSettings()
        {
            game.GameStates.Profil.writeProfilSteuerung(menu.OptionMenu.Listbox.Data);
            prompt.unloadPrompt();
            menu.CurrentMenu = "Optionen";
            menu.OptionMenu.Visiblity = false;
            menu.OptionMenu = null;
            menu.Visiblity = true;
        }

        public void SetPlayername()
        {
            prompt.SetPlayerName(menu.OptionMenu.Dialog.Data, "Playername");
            prompt.unloadPrompt();
            menu.OptionMenu.Activity = true;
        }

        public void nothing()
        {
            prompt.unloadPrompt();
            menu.HighscoreMenu.Active = true;
        }

        public void Save()
        {  
            switch (menu.OptionMenu.Dialog.District)
            {
                case (int)DistrictEnum.Grafik:
                    {
                        game.GameStates.Profil.writeProfilGrafik(menu.OptionMenu.Dialog.Data);                                                 
                        break;
                    }
                case (int)DistrictEnum.Sound:
                    {
                        game.GameStates.Profil.writeProfilSound(menu.OptionMenu.Dialog.Data);                            
                        break;
                    }
                case (int)DistrictEnum.Spielereinstellungen:
                    {
                        game.GameStates.Profil.writeProfilSpielereinstellungen(menu.OptionMenu.Dialog.Data);
                        break;
                    }
                default:
                    break;
            }
            if (prompt != null)
            {
                prompt.unloadPrompt();
            }
            menu.CurrentMenu = "Optionen";
            menu.OptionMenu.Dialog.list.adjustButtons(menu.OptionMenu.Dialog.startposition, "", false);
            menu.CurrentList.adjustButtons(new Vector2(), "Optionen", false);
            game.GameStates.Menu.Shifts.LeftShift.setPosition((new Vector2((int)((game.Graphics.Device.Viewport.Width
                        - 398 + 22)), (int)73)), 23, 72);
            game.GameStates.Menu.Shifts.RightShift.setPosition(new Vector2((int)game.Graphics.Device.Viewport.Width - 398
                + 378 - 44, (int)73), 23, 72); 
            menu.OptionMenu.Visiblity = false;
            menu.OptionMenu = null;
            menu.Visiblity = true;
        }

        public void nothingOpt()
        {
            prompt.unloadPrompt();
           /*menu.CurrentMenu = "Optionen";
            menu.OptionMenu.Visiblity = false;
            menu.OptionMenu = null;
            menu.Visiblity = true;*/
            menu.OptionMenu.Activity = true;
        }

        private void resetSettings()
        {
            if (menu.OptionMenu.Dialog != null)
            {
                switch (menu.OptionMenu.Dialog.District)
                {
                    case (int)DistrictEnum.Spielereinstellungen:
                        {
                            menu.OptionMenu.Dialog.Data["Difficult"] = game.GameStates.Profil.CurrentProfil.Difficult;
                            menu.OptionMenu.Dialog.Data["Playername"] = game.GameStates.Profil.CurrentProfil.Playername;
                            break;
                        }
                    case (int)DistrictEnum.Grafik:
                        {
                            menu.OptionMenu.Dialog.Data["Resolution"] = game.GameStates.Profil.CurrentProfil.Resolution;
                            menu.OptionMenu.Dialog.Data["Shadow"] = game.GameStates.Profil.CurrentProfil.Shadow;
                            menu.OptionMenu.Dialog.Data["Fog"] = game.GameStates.Profil.CurrentProfil.Fog;
                            menu.OptionMenu.Dialog.Data["Detail"] = game.GameStates.Profil.CurrentProfil.Detail;
                            menu.OptionMenu.Dialog.Data["Fullscreen"] = game.GameStates.Profil.CurrentProfil.Fullscreen;
                            menu.OptionMenu.Dialog.Data["Anti Aliasing"] = game.GameStates.Profil.CurrentProfil.AntiAliasing;
                            #region Resolution
                            switch (game.GameStates.Profil.CurrentProfil.Resolution)
                            {
                                case "800x600":
                                    {
                                        if (game.Graphics.SetScreenResolution(800, 600)) { }
                                            break;
                                    }
                                case "1024x600":
                                    {
                                        if (game.Graphics.SetScreenResolution(1024, 600)) { }
                                            break;
                                    }
                                case "1024x768":
                                    {
                                        if (game.Graphics.SetScreenResolution(1024, 768)) { }
                                            break;
                                    }
                                case "1280x768":
                                    {
                                        if (game.Graphics.SetScreenResolution(1280, 768)) { }
                                            break;
                                    }
                                case "1280x800":
                                    {
                                        if (game.Graphics.SetScreenResolution(1280, 800)) { }
                                            break;
                                    }
                                case "1280x960":
                                    {
                                        if (game.Graphics.SetScreenResolution(1280, 960)) { }
                                            break;
                                    }
                                case "1280x1024":
                                    {
                                        if (game.Graphics.SetScreenResolution(1280, 1024)) { }
                                            break;
                                    }
                                case "1360x768":
                                    {
                                        if (game.Graphics.SetScreenResolution(1360, 768)) { }
                                            break;
                                    }
                                case "1440x900":
                                    {
                                        if (game.Graphics.SetScreenResolution(1440, 900)) { }
                                            break;
                                    }
                                case "1600x1200":
                                    {
                                        if (game.Graphics.SetScreenResolution(1600, 1200)) { }
                                            break;
                                    }
                                case "1600x1280":
                                    {
                                        if (game.Graphics.SetScreenResolution(1600, 1280)) { }
                                            break;
                                    }
                                case "1768x992":
                                    {
                                        if (game.Graphics.SetScreenResolution(1768, 992)) { }
                                            break;
                                    }
                                case "1856x1392":
                                    {
                                        if (game.Graphics.SetScreenResolution(1856, 1392)) { }
                                            break;
                                    }
                                case "1920x1200":
                                    {
                                        if (game.Graphics.SetScreenResolution(1920, 1200)) { }
                                            break;
                                    }
                                default:
                                    break;
                            }
                            #endregion
                            #region Fullscreen
                            if (game.GameStates.Profil.CurrentProfil.Fullscreen)
                            {
                                game.Graphics.Fullscreen = true;
                            }
                            else
                            {
                                game.Graphics.Fullscreen = false;
                            }
                            #endregion
                            #region Anti Aliasing
                            switch (game.GameStates.Profil.CurrentProfil.AntiAliasing)
                            {
                                case "0x":
                                    {
                                        game.Graphics.MultiSampleType = MultiSampleType.None;
                                        break;
                                    }
                                case "2x":
                                    {
                                        game.Graphics.MultiSampleType = MultiSampleType.TwoSamples;
                                        break;
                                    }
                                case "4x":
                                    {
                                        game.Graphics.MultiSampleType = MultiSampleType.FourSamples;
                                        break;
                                    }
                                case "8x":
                                    {
                                        game.Graphics.MultiSampleType = MultiSampleType.EightSamples;
                                        break;
                                    }
                                case "16x":
                                    {
                                        game.Graphics.MultiSampleType = MultiSampleType.SixteenSamples;
                                        break;
                                    }
                                default:
                                    break;
                            }
                            #endregion
                            break;
                        }
                    case (int)DistrictEnum.Sound:
                        {
                            menu.OptionMenu.Dialog.Data["Music"] = game.GameStates.Profil.CurrentProfil.Music;
                            menu.OptionMenu.Dialog.Data["Effect"] = game.GameStates.Profil.CurrentProfil.Effect;
                            menu.OptionMenu.Dialog.Data["Sound"] = game.GameStates.Profil.CurrentProfil.Sound;
                            if ((bool)game.GameStates.Profil.CurrentProfil.Sound)
                            {
                                game.Sounds.EffectVolume = (float)game.GameStates.Profil.CurrentProfil.Effect;
                                game.Sounds.MusicVolume = (float)game.GameStates.Profil.CurrentProfil.Music;
                            }
                            else
                            {
                                game.Sounds.EffectVolume = 0;
                                game.Sounds.MusicVolume = 0;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            else if (menu.OptionMenu.Listbox != null)
            {
                menu.OptionMenu.Listbox.Data["Translation"] = game.GameStates.Profil.CurrentProfil.Translation;
                menu.OptionMenu.Listbox.Data["Acceleration Level"] = game.GameStates.Profil.CurrentProfil.Acceleration;
                menu.OptionMenu.Listbox.Data["Brake"] = game.GameStates.Profil.CurrentProfil.Brake;
                menu.OptionMenu.Listbox.Data["Banking"] = game.GameStates.Profil.CurrentProfil.Banking;
                menu.OptionMenu.Listbox.Data["Drift"] = game.GameStates.Profil.CurrentProfil.Drift;
                menu.OptionMenu.Listbox.Data["Auto Level"] = game.GameStates.Profil.CurrentProfil.AutoLevel;
                menu.OptionMenu.Listbox.Data["Rolling"] = game.GameStates.Profil.CurrentProfil.Rolling;
                menu.OptionMenu.Listbox.Data["Mouse Intensity"] = game.GameStates.Profil.CurrentProfil.mouseIntensity;

                menu.OptionMenu.Listbox.Data["Primary Fire"] = game.GameStates.Profil.CurrentProfil.PlayerFirePrim;
                menu.OptionMenu.Listbox.Data["Secondary Fire"] = game.GameStates.Profil.CurrentProfil.PlayerFireSek;
                menu.OptionMenu.Listbox.Data["Left Drift"] = game.GameStates.Profil.CurrentProfil.PlayerLeftDrift;
                menu.OptionMenu.Listbox.Data["Right Drift"] = game.GameStates.Profil.CurrentProfil.PlayerRightDrift;
                menu.OptionMenu.Listbox.Data["Brakes"] = game.GameStates.Profil.CurrentProfil.PlayerBrake;
                menu.OptionMenu.Listbox.Data["Acceleration"] = game.GameStates.Profil.CurrentProfil.PlayerAcceleration;
                menu.OptionMenu.Listbox.Data["Boost"] = game.GameStates.Profil.CurrentProfil.PlayerBoost;
                menu.OptionMenu.Listbox.Data["Primary Weapon 1"] = game.GameStates.Profil.CurrentProfil.PlayerPrimWeapon1;
                menu.OptionMenu.Listbox.Data["Primary Weapon 2"] = game.GameStates.Profil.CurrentProfil.PlayerPrimWeapon2;
                menu.OptionMenu.Listbox.Data["Secondary Weapon 1"] = game.GameStates.Profil.CurrentProfil.PlayerSekWeapon1;
                menu.OptionMenu.Listbox.Data["Secondary Weapon 2"] = game.GameStates.Profil.CurrentProfil.PlayerSekWeapon2;
            }
        }

        public void discardChanges()
        {
            resetSettings();
            menu.CurrentMenu = "Optionen";
            menu.OptionMenu.Visiblity = false;
            menu.OptionMenu = null;
            menu.Visiblity = true;
        }

        #endregion

        /// <summary>
        /// Execution of Player´s choise
        /// </summary>
        public void selection()
        {
            switch (menu.CurrentMenu)
            {
                #region Hauptmenü
                case "Hauptmenu":
                    {
                        switch (menu.CurrentButton.getButtonName())
                        {
                            case "Starten":
                                {
                                    menu.Visible = false;
                                    game.Input.Bind("Player1.Left", game.GameStates.Profil.CurrentProfil.PlayerLeftDrift);
                                    game.Input.Bind("Player1.Right", game.GameStates.Profil.CurrentProfil.PlayerRightDrift);
                                    game.Input.Bind("Player1.Up", game.GameStates.Profil.CurrentProfil.PlayerAcceleration);
                                    game.Input.Bind("Player1.Down", game.GameStates.Profil.CurrentProfil.PlayerBrake);
                                    game.Input.Bind("Player1.Boost", game.GameStates.Profil.CurrentProfil.PlayerBoost);
                                    game.Input.Bind("Player1.PrimaryFire", game.GameStates.Profil.CurrentProfil.PlayerFirePrim);
                                    game.Input.Bind("Player1.SecondaryFire", game.GameStates.Profil.CurrentProfil.PlayerFireSek);
                                    game.Input.Bind("Player1.PrimaryWeapon.Select1", game.GameStates.Profil.CurrentProfil.PlayerPrimWeapon1);
                                    game.Input.Bind("Player1.PrimaryWeapon.Select2", game.GameStates.Profil.CurrentProfil.PlayerPrimWeapon2);
                                    game.Input.Bind("Player1.SecondaryWeapon.Select3", game.GameStates.Profil.CurrentProfil.PlayerSekWeapon1);
                                    game.Input.Bind("Player1.SecondaryWeapon.Select4", game.GameStates.Profil.CurrentProfil.PlayerSekWeapon2);
                                    //TODO:LOADLEVEL
                                    //game.GameStates.SetStateGame("test");
                                    game.GameStates.SetStateLevelChoose();
                                    break;
                                }
                            case "Beenden":
                                {
                                    game.GameStates.Profil.CurrentProfil.Save("Profils\\Profil"
                                        + game.GameStates.Profil.CurrentNumber + ".xml");
                                    game.GameStates.SetStateCredits();
                                    break;
                                }
                            case "Optionen":
                                {
                                    menu.CurrentMenu = "Optionen";
                                    menu.CurrentList.init("Optionen", false);
                                    break;
                                }
                            case "Gleiterwahl":
                                {
                                    menu.Visible = false;
                                    game.Input.Bind("Player1.Left", game.GameStates.Profil.CurrentProfil.PlayerLeftDrift);
                                    game.Input.Bind("Player1.Right", game.GameStates.Profil.CurrentProfil.PlayerRightDrift);
                                    game.Input.Bind("Player1.Up", game.GameStates.Profil.CurrentProfil.PlayerAcceleration);
                                    game.Input.Bind("Player1.Down", game.GameStates.Profil.CurrentProfil.PlayerBrake);
                                    game.Input.Bind("Player1.Boost", game.GameStates.Profil.CurrentProfil.PlayerBoost);
                                    game.Input.Bind("Player1.PrimaryFire", game.GameStates.Profil.CurrentProfil.PlayerFirePrim);
                                    game.Input.Bind("Player1.SecondaryFire", game.GameStates.Profil.CurrentProfil.PlayerFireSek);
                                    game.Input.Bind("Player1.PrimaryWeapon.Select1", game.GameStates.Profil.CurrentProfil.PlayerPrimWeapon1);
                                    game.Input.Bind("Player1.PrimaryWeapon.Select2", game.GameStates.Profil.CurrentProfil.PlayerPrimWeapon2);
                                    game.Input.Bind("Player1.SecondaryWeapon.Select3", game.GameStates.Profil.CurrentProfil.PlayerSekWeapon1);
                                    game.Input.Bind("Player1.SecondaryWeapon.Select4", game.GameStates.Profil.CurrentProfil.PlayerSekWeapon2);

                                    game.GameStates.SetStateChooser();
                                    break;
                                }
                            case "Highscore":
                                {
                                    menu.CurrentMenu = "Highscore";
                                    menu.CurrentList.init("Highscore", true);
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                #endregion

                #region Optionen
                case "Optionen":
                    {
                        switch (menu.CurrentButton.getButtonName())
                        {
                            case "Zurück":
                                {
                                    menu.CurrentMenu = "Hauptmenu";
                                    menu.CurrentList.init("Hauptmenu", true);
                                    game.GameStates.Profil.CurrentProfil.Save("Profils\\Profil"
                                        + game.GameStates.Profil.CurrentNumber + ".xml");
                                    break;
                                }
                            case "Profil":
                                {
                                    menu.Visible = false;
                                    game.GameStates.SetStateProfil();
                                    break;
                                }
                            case "Spielereinstellungen":
                                {
                                    menu.Visiblity = false;
                                    menu.OptionMenu = new Option(game, menu.MenuXml);
                                    menu.OptionMenu.initializeDistrict((int)Option.DistrictEnum.Spielereinstellungen);
                                    break;
                                }
                            case "Grafik":
                                {
                                    menu.Visiblity = false;
                                    menu.OptionMenu = new Option(game, menu.MenuXml);
                                    menu.OptionMenu.initializeDistrict((int)Option.DistrictEnum.Grafik);
                                    break;
                                }
                            case "Sound":
                                {
                                    menu.Visiblity = false;
                                    menu.OptionMenu = new Option(game, menu.MenuXml);
                                    menu.OptionMenu.initializeDistrict((int)Option.DistrictEnum.Sound);
                                    break;
                                }
                            case "Steuerung":
                                {
                                    menu.Visiblity = false;
                                    menu.OptionMenu = new Option(game, menu.MenuXml);
                                    menu.OptionMenu.initializeDistrict((int)Option.DistrictEnum.Steuerung);
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                #endregion

                #region Highscore
                case "Highscore":
                    {
                        switch (menu.CurrentButton.getButtonName())
                        {
                            case "Zurück":
                                {
                                    menu.CurrentMenu = "Hauptmenu";
                                    menu.CurrentList.init("Hauptmenu", true);
                                    break;
                                }
                            case "Amateur":
                                {
                                    menu.HighscoreMenu = new HighscoreMenu(game, "Score", (int)DifficultEnum.leicht);
                                    menu.Visiblity = false;
                                    break;
                                }
                            case "Profi":
                                {
                                    menu.HighscoreMenu = new HighscoreMenu(game, "Score", (int)DifficultEnum.mittel);
                                    menu.Visiblity = false;
                                    break;
                                }
                            case "Elite":
                                {
                                    menu.HighscoreMenu = new HighscoreMenu(game, "Score", (int)DifficultEnum.schwer);
                                    menu.Visiblity = false;
                                    break;
                                }
                            case "ResetAll":
                                {                                    
                                    menu.CurrentButton.ButtonPressed();
                                    menu.Action = false;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                #endregion

                default:
                    break;
            }
        }

        /// <summary>
        /// Return true if the mousePosition is in the Box of a Button of the Buttonlist
        /// </summary>
        public bool isMouseInBox()
        {
            foreach (Button g in menu.CurrentList)
            {
                if ((menu.MousePosition.X >= g.getPosition().X) &&
                    (menu.MousePosition.X <= g.getPosition().Right) &&
                    (menu.MousePosition.Y >= g.getPosition().Y) &&
                    (menu.MousePosition.Y <= g.getPosition().Bottom))
                {
                    menu.MouseInRec = g.getPosition();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return true if the mousePosition is in the Box of the Shifts and set Buttonvalue active = true
        /// </summary>
        public bool isMouseInShift()
        {
            if ((menu.MousePosition.X >= menu.Shifts.LeftShift.getPosition().X) &&
                (menu.MousePosition.X <= menu.Shifts.LeftShift.getPosition().Right) &&
                (menu.MousePosition.Y >= menu.Shifts.LeftShift.getPosition().Y) &&
                (menu.MousePosition.Y <= menu.Shifts.LeftShift.getPosition().Bottom))
            {
                menu.MouseInRec = menu.Shifts.LeftShift.getPosition();
                menu.Shifts.LeftShift.SetActive();
                menu.Shifts.RightShift.resetActive();
                return true;
            }
            else if ((menu.MousePosition.X >= menu.Shifts.RightShift.getPosition().X) &&
                (menu.MousePosition.X <= menu.Shifts.RightShift.getPosition().Right) &&
                (menu.MousePosition.Y >= menu.Shifts.RightShift.getPosition().Y) &&
                (menu.MousePosition.Y <= menu.Shifts.RightShift.getPosition().Bottom))
            {
                menu.MouseInRec = menu.Shifts.RightShift.getPosition();
                menu.Shifts.RightShift.SetActive();
                menu.Shifts.LeftShift.resetActive();
                return true;
            }
            else
            {
                menu.Shifts.LeftShift.resetActive();
                menu.Shifts.RightShift.resetActive();
                return false;
            }
        }

        #endregion
    }
}
