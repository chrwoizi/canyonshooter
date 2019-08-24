// 
//  @ Project : CanyonShooter
//  @ File Name : ProfilControl.cs
//  @ Date : 16.03.2008
//  @ Author : Sascha Lity
//

using DescriptionLibs.Profil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CanyonShooter.Engine.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Profils
{
    /// <summary>
    /// Control of the profilchoice
    /// </summary>
    class ProfilControl
    {
        #region Data Member

        private Profil profil;

        private ICanyonShooterGame game;

        #endregion

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
        /// Constructor
        /// </summary>
        /// <param name="profil">Controling Profil</param>
        /// <param name="game">Current Canyonshootergame</param>
        public ProfilControl(Profil profil, ICanyonShooterGame game)
        {
            this.profil = profil;
            this.game = game;
        }

        #region Steuerung

        /// <summary>
        /// Buttonchoice with Keyboard
        /// </summary>
        /// <param name="taste">Pressed Key</param>
        public void changeButton(Keys taste)
        {
            bool test = (new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) == profil.mouseMovement);
            if (!isMouseInBox() ||
               new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) == profil.mouseMovement)
            {
                if (taste == Keys.Left)
                {
                    // Nice sound if the button has been pressed. by M.R.
                    PlayButtonSound();
                    // ------------------------------------------
                    profil.CurrentList.Previous();
                }
                else if (taste == Keys.Right)
                {
                    // Nice sound if the button has been pressed. by M.R.
                    PlayButtonSound();
                    // ------------------------------------------
                    profil.CurrentList.Next();
                }
            }

        }

        /// <summary>
        /// Profilchoice with Keyboard
        /// </summary>
        /// <param name="taste">Presses Key</param>
        public void changeProfil(Keys taste)
        {
            if (taste == Keys.Up)
            {
                if (profil.CurrentNumber == 1)
                {
                    profil.CurrentNumber = 5;
                }
                else
                {
                    profil.CurrentNumber -= 1;
                }
            }
            else if (taste == Keys.Down)
            {
                if (profil.CurrentNumber == 5)
                {
                    profil.CurrentNumber = 1;
                }
                else
                {
                    profil.CurrentNumber += 1;
                }
            }
        }

        /// <summary>
        /// Buttonchoice with Mouse
        /// </summary>
        public void changeButton()
        {
            if (isMouseInBox() && new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) != profil.mouseMovement)
            {
                foreach (Button i in profil.CurrentList)
                {
                    if (i.Position == profil.MouseInRec)
                    {
                        if (i == profil.CurrentButton)
                        {
                            continue;
                        }
                        else
                        {
                            for (int m = 0; m < profil.CurrentList.Count; m++)
                            {
                                if (i != profil.CurrentButton)
                                {
                                    // Nice sound if the button has been pressed. by M.R.
                                    PlayButtonSound();
                                    // ------------------------------------------
                                    profil.CurrentList.Next();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the current Profil
        /// </summary>
        public void activate()
        {
            foreach (ProfilProperties i in profil.ProfilXml)
            {
                if (profil.MouseInRec.Equals(new Rectangle(i.ActionrecX, i.ActionrecY,
                    i.ActionrecWidth, i.ActionrecHeigth)))
                {
                    profil.CurrentNumber = i.ProfilNummer;
                }
            }
        }

        /// <summary>
        /// Execution of Player´s choise
        /// </summary>
        public void selection()
        {
            switch (profil.CurrentButton.getButtonName())
            {
                case "Erstellen":
                    {
                        profil.Writing = true;
                        break;
                    }
                case "Ok":
                    {
                        if (profil.CurrentProfil.ProfilName.Equals("---"))
                        {
                            profil.Wahl = true;
                        }
                        else if (!profil.CurrentProfil.OnlineQuestion)
                        {
                            profil.question = true;
                        }
                        else
                        {
                            profil.Visible = false;
                            profil.makesDic(profil.CurrentProfil);
                            #region Resoultion
                            switch (profil.CurrentProfil.Resolution)
                            {
                                case "800x600":
                                    {
                                        game.Graphics.SetScreenResolution(800, 600);
                                        break;
                                    }
                                case "1024x600":
                                    {
                                        game.Graphics.SetScreenResolution(1024, 600);
                                        break;
                                    }
                                case "1024x768":
                                    {
                                        game.Graphics.SetScreenResolution(1024, 768);
                                        break;
                                    }
                                case "1280x768":
                                    {
                                        game.Graphics.SetScreenResolution(1280, 768);
                                        break;
                                    }
                                case "1280x800":
                                    {
                                        game.Graphics.SetScreenResolution(1280, 800);
                                        break;
                                    }
                                case "1280x960":
                                    {
                                        game.Graphics.SetScreenResolution(1280, 960);
                                        break;
                                    }
                                case "1280x1024":
                                    {
                                        game.Graphics.SetScreenResolution(1280, 1024);
                                        break;
                                    }
                                case "1360x768":
                                    {
                                        game.Graphics.SetScreenResolution(1360, 768);
                                        break;
                                    }
                                case "1440x900":
                                    {
                                        game.Graphics.SetScreenResolution(1360, 768);
                                        break;
                                    }
                                case "1600x1200":
                                    {
                                        game.Graphics.SetScreenResolution(1600, 1200);
                                        break;
                                    }
                                case "1600x1280":
                                    {
                                        game.Graphics.SetScreenResolution(1600, 1280);
                                        break;
                                    }
                                case "1768x992":
                                    {
                                        game.Graphics.SetScreenResolution(1768, 992);
                                        break;
                                    }
                                case "1856x1392":
                                    {
                                        game.Graphics.SetScreenResolution(1856, 1392);
                                        break;
                                    }
                                case "1920x1200":
                                    {
                                        game.Graphics.SetScreenResolution(1920, 1200);
                                        break;
                                    }
                                default:
                                    break;
                            }
                            #endregion

                            #region Fullscreen
                            if (profil.CurrentProfil.Fullscreen)
                            {
                                game.Graphics.Fullscreen = true;
                            }
                            else
                            {
                                game.Graphics.Fullscreen = false;
                            }
                            #endregion

                            #region Anti Aliasing
                            switch ((string)profil.CurrentProfil.AntiAliasing)
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

                            #region Sound
                            if (!(bool)profil.CurrentProfil.Sound)
                            {
                                game.Sounds.EffectVolume = 0f;
                                game.Sounds.MusicVolume = 0f;
                            }
                            else
                            {
                                game.Sounds.MusicVolume = (float)profil.CurrentProfil.Music;
                                game.Sounds.EffectVolume = (float)profil.CurrentProfil.Effect;
                            }
                            #endregion
                            if (profil.Opt)
                            {
                                profil.Menü = false;
                                game.GameStates.SetStateMenu();
                                
                            }
                            else if (!profil.Opt)
                            {
                                game.GameStates.SetStateMenu();
                                /*if (profil.quick)
                                {
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
                                    game.GameStates.SetStateGame(profil.QuickLevel);
                                }
                                else
                                {
                                    game.GameStates.SetStateSplash();
                                    //game.GameStates.SetStateIntro();   
                                }*/
                            }
                        }
                        break;
                    }
                case "Löschen":
                    {
                        profil.deleteProfil();
                        break;
                    }
            }
        }

        /// <summary>
        /// Check if the mousePosition is in the Rectangle of the Profil
        /// </summary>
        /// <returns>true if is in Rectangle</returns>
        public bool isMouseInRec()
        {
            bool result = false;

            foreach (ProfilProperties g in profil.ProfilXml)
            {
                if ((profil.MousePosition.X >= g.ActionrecX) &&
                    (profil.MousePosition.X <= (g.ActionrecX + g.ActionrecWidth)) &&
                    (profil.MousePosition.Y >= g.ActionrecY) &&
                    (profil.MousePosition.Y <= (g.ActionrecY + g.ActionrecHeigth)))
                {
                    profil.MouseInRec = new Rectangle(g.ActionrecX, g.ActionrecY,
                        g.ActionrecWidth, g.ActionrecHeigth);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the mousePosition is in Box of Buttons
        /// </summary>
        /// <returns>true if is in Box</returns>
        public bool isMouseInBox()
        {
            bool result = false;

            foreach (Button g in profil.CurrentList)
            {
                if ((profil.MousePosition.X >= g.Position.X) &&
                    (profil.MousePosition.X <= g.Position.Right) &&
                    (profil.MousePosition.Y >= g.Position.Y) &&
                    (profil.MousePosition.Y <= g.Position.Bottom))
                {
                    profil.MouseInRec = g.Position;
                    result = true;
                }
            }

            return result;
        }

        #endregion
    }
}
