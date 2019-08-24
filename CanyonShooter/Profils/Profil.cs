#region Using Statements

using System;
using System.Collections.Generic;
using DescriptionLibs.Profil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CanyonShooter.Engine;
using CanyonShooter.Engine.Audio;
using CanyonShooter.Engine.Input;
using System.Reflection.Emit;

#endregion

namespace CanyonShooter.Profils
{
    /// <summary>
    /// Class Profil manage the Profilchoice
    /// </summary>
    public class Profil : DrawableGameComponent, IProfil
    {
        #region Data Member

        #region Input
        private int counter;
        private ProfilControl steuerung;
        private String profilname = "";
        private Rectangle mouseInRec;
        public bool question;
        private string answer = "";
        private bool player;
        private string pname;
        #endregion

        private static Vector2 mousepos = new Vector2(0, 0);
        public Vector2 mouseMovement;
        public bool quick = false;
        private string quickLevel = "Level1";
        private bool writing = false;
        private bool wahl = false;
        private int currentNumber;
        private bool menü = false;
        private bool esc = false;
        private bool opt = false;

        //private bool soundPlay = true;

        #region XML und aktuelles Profil
        private ProfilDescription bildXml;
        private List<ProfilProperties> profilXml;
        private ProfilProperties currentProfil;
        private Dictionary<string, object> grafik;
        private Dictionary<string, object> sound;
        private Dictionary<string, object> steuerungen;
        private Dictionary<string, object> spielereinstellungen;
        private Dictionary<string, string> grafikDat;
        private Dictionary<string, string> soundDat;
        private Dictionary<string, string> steuerungDat;
        private Dictionary<string, string> spielereinstellungenDat;
        private List<string> namenKeysSound;
        private List<string> namenKeysGrafik;
        private List<string> namenKeysSteuerung;
        private List<string> namenKeysSpielereinstellungen;

        #endregion

        #region Draw
        private Texture2D background;
        private Texture2D buttonback;
        private Texture2D buttonbackbarCreate;
        private Texture2D buttonbackbarDelete;
        private Texture2D buttonbackbarOk;
        private Texture2D mouse;
        private Texture2D textfield;
        private Texture2D marker;
        private SpriteBatch SpriteBatch;
        private SpriteFont font;
        private ButtonList currentList;
        private Texture2D textback;
        #endregion

        private ICanyonShooterGame game;
        #endregion

        #region Properties

        public bool Quick
        {
            get { return quick; }
            set { quick = value; }
        }

        public string QuickLevel
        {
            get { return quickLevel; }
            set { quickLevel = value; }
        }

        public Dictionary<string, string> GrafikDat
        {
            get { return grafikDat; }
        }

        public Dictionary<string, string> SoundDat
        {
            get { return soundDat; }
        }

        public Dictionary<string, string> SteuerungDat
        {
            get { return steuerungDat; }
        }

        public Dictionary<string, string> SpielereinstellungenDat
        {
            get { return spielereinstellungenDat; }
        }

        public List<string> NamenKeysSound
        {
            get { return namenKeysSound; }
        }

        public List<string> NamenKeysGrafik
        {
            get { return namenKeysGrafik; }
        }

        public List<string> NamenKeysSteuerung
        {
            get { return namenKeysSteuerung; }
        }

        public List<string> NamenKeysSpielereinstellungen
        {
            get { return namenKeysSpielereinstellungen; }
        }

        public bool Menü
        {
            get { return menü; }
            set { menü = value; }
        }

        public bool Opt
        {
            get { return opt; }
            set { opt = value; }
        }

        public bool Esc
        {
            get { return esc; }
            set { esc = value; }
        }

        public bool Writing
        {
            get { return writing; }
            set { writing = value; }
        }

        public Dictionary<string, Object> GrafikList
        {
            get { return grafik; }
            set { grafik = value; }
        }

        public Dictionary<string, Object> SoundList
        {
            get { return sound; }
            set { sound = value; }
        }

        public Dictionary<string, Object> SteuerungList
        {
            get { return steuerungen; }
            set { steuerungen = value; }
        }

        public Dictionary<string, Object> SpielereinstellungenList
        {
            get { return spielereinstellungen; }
            set { spielereinstellungen = value; }
        }

        public bool Wahl
        {
            get { return wahl; }
            set { wahl = value; }
        }

        public ProfilProperties CurrentProfil
        {
            get { return currentProfil; }
            set { currentProfil = value; }
        }

        public List<ProfilProperties> ProfilXml
        {
            get { return profilXml; }
            set { profilXml = value; }
        }

        public ProfilDescription BildXml
        {
            get { return bildXml; }
            set { bildXml = value; }
        }

        public Rectangle MouseInRec
        {
            get { return mouseInRec; }
            set { mouseInRec = value; }
        }

        public ButtonList CurrentList
        {
            get { return currentList; }
            set { currentList = value; }
        }

        public Button CurrentButton
        {
            get { return CurrentList.currentNode.Value; }
        }

        public Vector2 MousePosition
        {
            get { return new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y); }
        }

        public int CurrentNumber
        {
            get { return currentNumber; }
            set { currentNumber = value; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">current Canyonshootergame</param>
        public Profil(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
            question = false;

            BildXml = game.Content.Load<ProfilDescription>("Content\\Profil\\" + "Profil");
            ProfilXml = new List<ProfilProperties>();
            GrafikList = new Dictionary<string, object>();
            SoundList = new Dictionary<string, object>();
            SteuerungList = new Dictionary<string, object>();
            SpielereinstellungenList = new Dictionary<string, object>();

            grafikDat = new Dictionary<string, string>();
            soundDat = new Dictionary<string, string>();
            steuerungDat = new Dictionary<string, string>();
            spielereinstellungenDat = new Dictionary<string, string>();
            namenKeysSound = new List<string>();
            namenKeysGrafik = new List<string>();
            namenKeysSteuerung = new List<string>();
            namenKeysSpielereinstellungen = new List<string>();

            this.Visible = true;

            steuerung = new ProfilControl(this, game);

            CurrentList = new ButtonList(game, BildXml);

            counter = 0;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            //Load Profils 
            ProfilProperties profile = new ProfilProperties();
            for (int i = 1; i <= 5; i++)
            {
                profile = ProfilProperties.Load("Profils\\Profil" + i + ".xml");
                ProfilXml.Add(profile);
            }

            //Is there an active Profil at beginning
            int test = 0;
            foreach (ProfilProperties i in ProfilXml)
            {
                if (i.Active == true)
                {
                    test += 1;
                    if (test == 1)
                    {
                        CurrentNumber = i.ProfilNummer;
                    }
                }
            }
            if (test == 0)
            {
                CurrentNumber = 1;
                Menü = true;
            }
            CurrentProfil = ProfilXml[CurrentNumber - 1];

            //StartButton if there is an active Profil
            if (CurrentProfil.Active == true)
            {
                while (!CurrentButton.getButtonName().Equals("Ok"))
                {
                    CurrentList.Next();
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
            //if (!Opt && soundPlay)
            //{
            // Start music box machine by M.Rodriguez
            //    game.Sounds.MusicBox(MusicBoxStatus.Play);
            //    soundPlay = false;
            //}
            if (!Menü && CurrentProfil.OnlineQuestion)
            {
                makesDic(CurrentProfil);
                #region Resolution
                switch (CurrentProfil.Resolution)
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
                if (CurrentProfil.Fullscreen)
                {
                    game.Graphics.Fullscreen = true;
                }
                else
                {
                    game.Graphics.Fullscreen = false;
                }
                #endregion

                #region Anti Aliasing
                switch ((string)CurrentProfil.AntiAliasing)
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
                if (!(bool)CurrentProfil.Sound)
                {
                    game.Sounds.EffectVolume = 0f;
                    game.Sounds.MusicVolume = 0f;
                }
                else
                {
                    game.Sounds.MusicVolume = (float)CurrentProfil.Music;
                    game.Sounds.EffectVolume = (float)CurrentProfil.Effect;
                }
                #endregion
                this.Visible = false;
                game.GameStates.SetStateMenu();
                /*if (quick)
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
                    game.GameStates.SetStateGame(quickLevel);
                }
                else
                {
                    game.GameStates.SetStateMenu();
                    //game.GameStates.SetStateSplash();
                }+*/
            }
            if (Visible)
            {
                #region Eingabe
                if (Writing)
                {
                    if (!CurrentProfil.Active)
                    {
                        if ((counter < 20) || (game.Input.HasKeyJustBeenPressed("Menu.Back")))
                        {
                            game.Input.HandleKeyboardInput(ref profilname);
                            counter = profilname.Length;
                        }
                        if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                        {
                            Writing = false;
                            counter = 0;
                            if (!profilname.Equals(""))
                            {
                                writeProfilName(profilname);
                                profilname = "";
                            }
                        }
                    }
                    else
                    {
                        if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                        {
                            writing = false;
                            counter = 0;
                        }
                    }
                }
                else if (question)
                {
                    if ((counter < 1) || (game.Input.HasKeyJustBeenPressed("Menu.Back")))
                    {
                        game.Input.HandleKeyboardInput(ref answer);
                        counter = answer.Length;
                    }
                    if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                    {
                        if (answer.Equals("y"))
                        {
                            CurrentProfil.OnlineHighscore = true;
                            question = false;
                            player = true;
                            pname = CurrentProfil.Playername;
                            writeOnline();
                            counter = 0;
                        }
                        else if (answer.Equals("n"))
                        {
                            CurrentProfil.OnlineHighscore = false;
                            question = false;
                            player = true;
                            pname = CurrentProfil.Playername;
                            writeOnline();
                            counter = 0;
                        }
                        else
                        {
                            answer = "";
                            counter = 0;
                        }
                    }
                }
                else if (player)
                {
                    if ((counter < 15) || (game.Input.HasKeyJustBeenPressed("Menu.Back")))
                        {
                            game.Input.HandleKeyboardInput(ref pname);
                            counter = pname.Length;
                        }
                        if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                        {
                            if (!pname.Equals(""))
                            {
                                CurrentProfil.Playername = pname;
                                player = false;
                                CurrentProfil.OnlineQuestion = true;
                                writePlayer();
                                counter = 0;
                            }
                        }
                }
                #endregion

                else if (Wahl)
                {
                    if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                    {
                        Wahl = false;
                    }
                }

                #region Input
                else
                {
                    #region Mouse
                    steuerung.changeButton();
                    if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
                    {
                        if (steuerung.isMouseInBox())
                        {
                            if (MouseInRec == this.CurrentButton.Position)
                            {
                                steuerung.selection();
                            }
                        }
                        else if (steuerung.isMouseInRec())
                        {
                            steuerung.activate();
                            CurrentProfil = ProfilXml[CurrentNumber - 1];
                            Wahl = false;
                        }
                    }
                    #endregion

                    #region Tastatur
                    if (game.Input.HasKeyJustBeenPressed("Menu.Left"))
                    {
                        steuerung.changeButton(Keys.Left);
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Right"))
                    {
                        steuerung.changeButton(Keys.Right);
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                    {
                        steuerung.selection();
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Down"))
                    {
                        steuerung.changeProfil(Keys.Down);
                        CurrentProfil = ProfilXml[CurrentNumber - 1];
                        Wahl = false;
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Up"))
                    {
                        steuerung.changeProfil(Keys.Up);
                        CurrentProfil = ProfilXml[CurrentNumber - 1];
                        Wahl = false;
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
                    {
                        if (!Esc)
                        {
                            Game.Exit();
                        }
                    }
                    #endregion
                }
                #endregion
            }
            if (mousepos != MousePosition)
            {
                mouseMovement = MousePosition;
            }

        }

        /// <summary>
        /// Called when the DrawableGameComponent needs to be drawn.  Override this method
        /// with component-specific drawing code.
        /// </summary>
        /// <parameters>
        /// gameTime:
        ///     Time passed since the last call to 
        ///     Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime).
        /// </parameters>
        public override void Draw(GameTime gameTime)
        {
            if (!Menü && CurrentProfil.OnlineQuestion)
            {
                if (SpriteBatch != null)
                {
                    SpriteBatch.Begin();
                    SpriteBatch.GraphicsDevice.Clear(Color.Black);
                    SpriteBatch.End();
                }
            }
            else
            {
                if (Visible)
                {
                    if (SpriteBatch != null)
                    {
                        SpriteBatch.Begin();
                        SpriteBatch.Draw(background, new Rectangle(0, 0,
                            game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height),
                            Color.White);
                        SpriteBatch.Draw(textback, BildXml.TextbackRec, Color.White);
                        SpriteBatch.Draw(buttonback, new Rectangle(game.Graphics.Device.Viewport.Width - 40
                            - BildXml.ButtonBackRec.Width, BildXml.ButtonBackRec.Y, BildXml.ButtonBackRec.Width,
                            BildXml.ButtonBackRec.Height), Color.White);

                        #region Draw Bars
                        switch (CurrentButton.getButtonName())
                        {
                            case "Erstellen":
                                {
                                    SpriteBatch.Draw(buttonbackbarCreate, new Rectangle(game.Graphics.Device.Viewport.Width
                                        - 50 - BildXml.barRec.Width, BildXml.barRec.Y, BildXml.barRec.Width,
                                        BildXml.barRec.Height), Color.White);
                                    break;
                                }
                            case "Löschen":
                                {
                                    SpriteBatch.Draw(buttonbackbarDelete, new Rectangle(game.Graphics.Device.Viewport.Width
                                        - 50 - BildXml.barRec.Width, BildXml.barRec.Y, BildXml.barRec.Width,
                                        BildXml.barRec.Height), Color.White);
                                    break;
                                }
                            case "Ok":
                                {
                                    SpriteBatch.Draw(buttonbackbarOk, new Rectangle(game.Graphics.Device.Viewport.Width
                                        - 50 - BildXml.barRec.Width, BildXml.barRec.Y, BildXml.barRec.Width,
                                        BildXml.barRec.Height), Color.White);
                                    break;
                                }
                            default:
                                break;
                        }
                        #endregion

                        CurrentList.Draw(gameTime, SpriteBatch);

                        #region Profilnamen und Marker

                        foreach (ProfilProperties j in ProfilXml)
                        {
                            if (j.ProfilNummer == CurrentNumber)
                            {
                                SpriteBatch.Draw(marker, new Rectangle(j.ActionrecX, j.ActionrecY, j.ActionrecWidth,
                                    j.ActionrecHeigth), Color.White);
                            }
                            SpriteBatch.DrawString(font, j.ProfilName, new Vector2(j.PositionX, j.PositionY),
                                Color.Black);
                        }

                        #endregion

                        SpriteBatch.DrawString(font, "Please select: ", new Vector2(50, 104), Color.Black);

                        #region Eingabe
                        if (Writing)
                        {
                            SpriteBatch.Draw(textfield, new Rectangle(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2,
                                textfield.Width, textfield.Height), Color.White);
                            if (!CurrentProfil.Active)
                            {
                                if (counter == 20)
                                {
                                    SpriteBatch.DrawString(font, "Please enter profilename:\n\n" + profilname,
                                    new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                    game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40),
                                    Color.White);
                                }
                                else
                                {
                                    SpriteBatch.DrawString(font, "Please enter profilename:\n\n" + profilname + "_",
                                    new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                    game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40),
                                    Color.White);
                                }
                            }
                            else
                            {
                                SpriteBatch.DrawString(font, "No changes to a existing \nprofile \n" +
                                    "Press any Key to return...",
                                    new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                    game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40),
                                    Color.White);
                            }
                        }
                        #endregion
                        else if (question)
                        {
                            SpriteBatch.Draw(textfield, new Rectangle(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2,
                                textfield.Width, textfield.Height), Color.White);
                            if (counter == 1)
                            {
                                SpriteBatch.DrawString(font, "Would you like to send\n" +
                                "your score to the Online \nHighscore? (y - yes/n - no)\n" + answer,
                                new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40),
                                Color.White);
                            }
                            else
                            {
                                SpriteBatch.DrawString(font, "Would you like to send\n" +
                                "your score to the Online \nHighscore? (y - yes/n - no)\n" + answer + "_",
                                new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40),
                                Color.White);
                            }
                        }
                        else if (player)
                        {
                            SpriteBatch.Draw(textfield, new Rectangle(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2,
                                textfield.Width, textfield.Height), Color.White);
                            if (counter == 15)
                            {
                                SpriteBatch.DrawString(font, "Please insert playername\n\n" +  pname,
                                new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40),
                                Color.White);
                            }
                            else
                            {
                                SpriteBatch.DrawString(font, "Please insert playername\n\n" +  pname + "_",
                                new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40),
                                Color.White);
                            }
                        }
                        else if (Wahl)
                        {
                            SpriteBatch.Draw(textfield, new Rectangle(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2,
                                textfield.Width, textfield.Height), Color.White);
                            SpriteBatch.DrawString(font, "Please create a profile \nor select one.\n" +
                                "Press any Key to return...",
                                new Vector2(game.Graphics.Device.Viewport.Width / 2 - textfield.Width / 2 + 30,
                                game.Graphics.Device.Viewport.Height / 2 - textfield.Height / 2 + 40), Color.White);
                        }
                        else
                        {
                            SpriteBatch.Draw(mouse, MousePosition, Color.White);
                        }
                        SpriteBatch.End();
                    }
                }
            }
        }

        /// <summary>
        /// Called when the component needs to load graphics resources.  Override to
        /// load any component specific graphics resources.
        /// </summary>
        /// <parameters>
        /// loadAllContent:
        ///    true if all graphics resources need to be loaded;false if only manual resources
        ///    need to be loaded.
        /// </parameters>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(game.Graphics.Device);
            background = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.BackgroundImage);
            textfield = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.TextFieldImage);
            mouse = game.Content.Load<Texture2D>("Content\\Textures\\" + BildXml.Mouse);
            CurrentList.LoadContent();
            font = game.Content.Load<SpriteFont>("Arial");
            font.Spacing = 0f;
            marker = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.Marker);
            textback = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.Textback);
            buttonback = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.buttonBackground);
            buttonbackbarCreate = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.barCreate);
            buttonbackbarDelete = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.barDelete);
            buttonbackbarOk = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + BildXml.barOk);
        }

        /// <summary>
        /// deleteProfil() setzt die veränderbaren Werte auf ihren Default-Wert
        /// </summary>
        public void deleteProfil()
        {
            //veränderbaren Werte
            CurrentProfil.ProfilName = "---";
            CurrentProfil.Active = false;
            CurrentProfil.OnlineHighscore = false;
            CurrentProfil.OnlineQuestion = false;
            CurrentProfil.Difficult = 1;
            CurrentProfil.Resolution = "1024x768";
            CurrentProfil.Shadow = true;
            CurrentProfil.Fog = true;
            CurrentProfil.Detail = 0.5f;
            CurrentProfil.Fullscreen = false;
            CurrentProfil.AntiAliasing = "0x";
            CurrentProfil.Music = 0.3f;
            CurrentProfil.Effect = 1.0f;
            CurrentProfil.Sound = true;
            CurrentProfil.Playername = "Playername";
            CurrentProfil.Model = "Starfighter";
            CurrentProfil.Health = 100;
            CurrentProfil.Speed = 410;
            CurrentProfil.Shield = 80;
            CurrentProfil.Fuel = 500;
            CurrentProfil.Translation = 0.5f;
            CurrentProfil.Acceleration = 10f;
            CurrentProfil.Brake = 10f;
            CurrentProfil.Banking = 0.01f;
            CurrentProfil.Drift = 0.01f;
            CurrentProfil.AutoLevel = 0.05f;
            CurrentProfil.Rolling = 0.8f;
            CurrentProfil.mouseIntensity = -0.001f;
            CurrentProfil.PlayerFirePrim = MouseButton.Left;
            CurrentProfil.PlayerFireSek = MouseButton.Right;
            CurrentProfil.PlayerLeftDrift = Keys.A;
            CurrentProfil.PlayerRightDrift = Keys.D;
            CurrentProfil.PlayerBrake = Keys.S;
            CurrentProfil.PlayerAcceleration = Keys.W;
            CurrentProfil.PlayerBoost = Keys.Space;
            CurrentProfil.PlayerPrimWeapon1 = Keys.D1;
            CurrentProfil.PlayerPrimWeapon2 = Keys.D2;
            CurrentProfil.PlayerSekWeapon1 = Keys.D3;
            CurrentProfil.PlayerSekWeapon2 = Keys.D4;

            question = false;

            //Speicherung (für Konsistenz)
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }

        public void makesDic(ProfilProperties eingabe)
        {
            //Wichtig: Casten in das jeweilige Format

            #region Clear
            GrafikList.Clear();
            SoundList.Clear();
            SteuerungList.Clear();
            SpielereinstellungenList.Clear();

            GrafikDat.Clear();
            SoundDat.Clear();
            SteuerungDat.Clear();
            SpielereinstellungenDat.Clear();
            NamenKeysSound.Clear();
            NamenKeysGrafik.Clear();
            NamenKeysSteuerung.Clear();
            NamenKeysSpielereinstellungen.Clear();

            #endregion

            //Zuweisen der Werte
            #region Grafik
            NamenKeysGrafik.Add("Resolution");
            NamenKeysGrafik.Add("Shadow");
            NamenKeysGrafik.Add("Fog");
            NamenKeysGrafik.Add("Detail");
            NamenKeysGrafik.Add("Fullscreen");
            NamenKeysGrafik.Add("Anti Aliasing");

            GrafikDat.Add("Resolution", "string");
            GrafikDat.Add("Shadow", "bool");
            GrafikDat.Add("Fog", "bool");
            GrafikDat.Add("Detail", "float");
            GrafikDat.Add("Fullscreen", "bool");
            GrafikDat.Add("Anti Aliasing", "string");

            GrafikList.Add("Resolution", CurrentProfil.Resolution);
            GrafikList.Add("Shadow", CurrentProfil.Shadow);
            GrafikList.Add("Fog", CurrentProfil.Fog);
            GrafikList.Add("Detail", CurrentProfil.Detail);
            GrafikList.Add("Fullscreen", CurrentProfil.Fullscreen);
            GrafikList.Add("Anti Aliasing", CurrentProfil.AntiAliasing);
            #endregion

            #region Sound
            NamenKeysSound.Add("Music");
            NamenKeysSound.Add("Effect");
            NamenKeysSound.Add("Sound");

            SoundDat.Add("Music", "float");
            SoundDat.Add("Effect", "float");
            SoundDat.Add("Sound", "bool");

            SoundList.Add("Music", CurrentProfil.Music);
            SoundList.Add("Effect", CurrentProfil.Effect);
            SoundList.Add("Sound", CurrentProfil.Sound);
            #endregion

            #region Steuerung

            NamenKeysSteuerung.Add("Translation");
            NamenKeysSteuerung.Add("Acceleration Level");
            NamenKeysSteuerung.Add("Brake");
            NamenKeysSteuerung.Add("Banking");
            NamenKeysSteuerung.Add("Drift");
            NamenKeysSteuerung.Add("Auto Level");
            NamenKeysSteuerung.Add("Rolling");
            NamenKeysSteuerung.Add("Mouse Intensity");
            NamenKeysSteuerung.Add("Primary Fire");
            NamenKeysSteuerung.Add("Secondary Fire");
            NamenKeysSteuerung.Add("Left Drift");
            NamenKeysSteuerung.Add("Right Drift");
            NamenKeysSteuerung.Add("Brakes");
            NamenKeysSteuerung.Add("Acceleration");
            NamenKeysSteuerung.Add("Boost");
            NamenKeysSteuerung.Add("Primary Weapon 1");
            NamenKeysSteuerung.Add("Primary Weapon 2");
            NamenKeysSteuerung.Add("Secondary Weapon 1");
            NamenKeysSteuerung.Add("Secondary Weapon 2");

            SteuerungDat.Add("Translation", "float");
            SteuerungDat.Add("Acceleration Level", "float");
            SteuerungDat.Add("Brake", "float");
            SteuerungDat.Add("Banking", "float");
            SteuerungDat.Add("Drift", "float");
            SteuerungDat.Add("Auto Level", "float");
            SteuerungDat.Add("Rolling", "float");
            SteuerungDat.Add("Mouse Intensity", "float");
            SteuerungDat.Add("Primary Fire", "MouseButton");
            SteuerungDat.Add("Secondary Fire", "MouseButton");
            SteuerungDat.Add("Left Drift", "Keys");
            SteuerungDat.Add("Right Drift", "Keys");
            SteuerungDat.Add("Brakes", "Keys");
            SteuerungDat.Add("Acceleration", "Keys");
            SteuerungDat.Add("Boost", "Keys");
            SteuerungDat.Add("Primary Weapon 1", "Keys");
            SteuerungDat.Add("Primary Weapon 2", "Keys");
            SteuerungDat.Add("Secondary Weapon 1", "Keys");
            SteuerungDat.Add("Secondary Weapon 2", "Keys");

            SteuerungList.Add("Translation", CurrentProfil.Translation);
            SteuerungList.Add("Acceleration Level", CurrentProfil.Acceleration);
            SteuerungList.Add("Brake", CurrentProfil.Brake);
            SteuerungList.Add("Banking", CurrentProfil.Banking);
            SteuerungList.Add("Drift", CurrentProfil.Drift);
            SteuerungList.Add("Auto Level", CurrentProfil.AutoLevel);
            SteuerungList.Add("Rolling", CurrentProfil.Rolling);
            SteuerungList.Add("Mouse Intensity", CurrentProfil.mouseIntensity);
            SteuerungList.Add("Primary Fire", CurrentProfil.PlayerFirePrim);
            SteuerungList.Add("Secondary Fire", CurrentProfil.PlayerFireSek);
            SteuerungList.Add("Left Drift", CurrentProfil.PlayerLeftDrift);
            SteuerungList.Add("Right Drift", CurrentProfil.PlayerRightDrift);
            SteuerungList.Add("Brakes", CurrentProfil.PlayerBrake);
            SteuerungList.Add("Acceleration", CurrentProfil.PlayerAcceleration);
            SteuerungList.Add("Boost", CurrentProfil.PlayerBoost);
            SteuerungList.Add("Primary Weapon 1", CurrentProfil.PlayerPrimWeapon1);
            SteuerungList.Add("Primary Weapon 2", CurrentProfil.PlayerPrimWeapon2);
            SteuerungList.Add("Secondary Weapon 1", CurrentProfil.PlayerSekWeapon1);
            SteuerungList.Add("Secondary Weapon 2", CurrentProfil.PlayerSekWeapon2);
            #endregion

            #region Spielereinstellungen
            NamenKeysSpielereinstellungen.Add("Difficult");
            NamenKeysSpielereinstellungen.Add("Playername");

            SpielereinstellungenDat.Add("Difficult", "int");
            SpielereinstellungenDat.Add("Playernam", "string");

            SpielereinstellungenList.Add("Difficult", CurrentProfil.Difficult);
            SpielereinstellungenList.Add("Playername", CurrentProfil.Playername);
            #endregion
        }

        #region Schreiben
        private void writePlayer()
        {
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }

        private void writeOnline()
        {
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }


        /// <summary>
        /// writeProfilName(string Name) verändert den Profilnamen
        /// </summary>
        public void writeProfilName(string Name)
        {
            CurrentProfil.ProfilName = Name;
            CurrentProfil.Active = true;  
        }

        /// <summary>
        /// writeProfilSound(Dictionary parameter) verändert/setzt und 
        /// speichert die Parameter
        /// </summary>
        public void writeProfilSound(Dictionary<string, object> parameter)
        {
            //Übergabe der Werte
            CurrentProfil.Music = (float)parameter["Music"];
            CurrentProfil.Effect = (float)parameter["Effect"];
            CurrentProfil.Sound = (bool)parameter["Sound"];

            //Speicherung für Konsistenz
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }

        /// <summary>
        /// writeProfilGrafik(Dictionary parameter) verändert/setzt und 
        /// speichert die Parameter
        /// </summary>
        public void writeProfilGrafik(Dictionary<string, object> parameter)
        {
            //Übergabe der Werte
            CurrentProfil.Resolution = (string)parameter["Resolution"];
            CurrentProfil.Shadow = (bool)parameter["Shadow"];
            CurrentProfil.Fog = (bool)parameter["Fog"];
            CurrentProfil.Detail = (float)parameter["Detail"];
            CurrentProfil.Fullscreen = (bool)parameter["Fullscreen"];
            CurrentProfil.AntiAliasing = (string)parameter["Anti Aliasing"];

            //Speicherung für Konsistenz
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }

        /// <summary>
        /// writeProfilSteuerung(Dictionary parameter) verändert/setzt und 
        /// speichert die Parameter
        /// </summary>
        public void writeProfilSteuerung(Dictionary<string, object> parameter)
        {
            //Übergabe der Werte
            CurrentProfil.Translation = (float)parameter["Translation"];
            CurrentProfil.Acceleration = (float)parameter["Acceleration Level"];
            CurrentProfil.Brake = (float)parameter["Brake"];
            CurrentProfil.Banking = (float)parameter["Banking"];
            CurrentProfil.Drift = (float)parameter["Drift"];
            CurrentProfil.AutoLevel = (float)parameter["Auto Level"];
            CurrentProfil.Rolling = (float)parameter["Rolling"];
            CurrentProfil.mouseIntensity = (float)parameter["Mouse Intensity"];
            CurrentProfil.PlayerFirePrim = (MouseButton)parameter["Primary Fire"];
            CurrentProfil.PlayerFireSek = (MouseButton)parameter["Secondary Fire"];
            CurrentProfil.PlayerLeftDrift = (Keys)parameter["Left Drift"];
            CurrentProfil.PlayerRightDrift = (Keys)parameter["Right Drift"];
            CurrentProfil.PlayerBrake = (Keys)parameter["Brakes"];
            CurrentProfil.PlayerAcceleration = (Keys)parameter["Acceleration"];
            CurrentProfil.PlayerBoost = (Keys)parameter["Boost"];
            CurrentProfil.PlayerPrimWeapon1 = (Keys)parameter["Primary Weapon 1"];
            CurrentProfil.PlayerPrimWeapon2 = (Keys)parameter["Primary Weapon 2"];
            CurrentProfil.PlayerSekWeapon1 = (Keys)parameter["Secondary Weapon 1"];
            CurrentProfil.PlayerSekWeapon2 = (Keys)parameter["Secondary Weapon 2"];

            //Speicherung für Konsistenz
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }

        /// <summary>
        /// writeProfilSpielereinstellungen(Dictionary parameter) verändert/setzt und 
        /// speichert die Parameter
        /// </summary>
        public void writeProfilSpielereinstellungen(Dictionary<string, object> parameter)
        {
            //Delivery the values
            CurrentProfil.Difficult = (int)parameter["Difficult"];
            CurrentProfil.Playername = (string)parameter["Playername"];

            //Securesave for consistency
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }

        /// <summary>
        /// writeProfilGleiter(string Name) set Player´s model
        /// </summary>
        /// <param name="Name">name of the model</param>
        public void writeProfilGleiter(string Name, int shield, int speed, int health, int fuel)
        {
            CurrentProfil.Model = Name;
            CurrentProfil.Shield = shield;
            CurrentProfil.Speed = speed;
            CurrentProfil.Health = health;
            CurrentProfil.Fuel = fuel;

            //Securesave for consistency
            CurrentProfil.Save("Profils\\Profil" + CurrentNumber + ".xml");
        }

        #endregion
    }
}


