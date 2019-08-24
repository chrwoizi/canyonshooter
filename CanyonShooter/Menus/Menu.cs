// Zuständigkeit: Sascha, Malte

#region Using Statements

using System;
using System.Threading;
using CanyonShooter.Engine;
using CanyonShooter.Profils;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CanyonShooter.GameClasses.Scores;
#endregion

namespace CanyonShooter.Menus
{
    /// <summary>
    /// Zum Menu gehören folgende Aufgaben:
    /// Anzeigen des Menüs durch Sprites (Eigenständig d.h. ohne Renderer, ggf. XNA Sprite Klasse wrappen)
    /// Eingabe über Tastatur und ggf. Maus verwerten (Input Komponente verwenden)
    /// Hierarchische Menüstruktur (Hauptmenü->Optionen->Grafik, Hauptmenü->Optionen->Sound, ...)
    /// Menüs aus Konfigurationsdateien laden, um Texte und Bilder anzupassen
    /// Einstellungen der Optionsmenüs laden und abspeichern. Eine globale Fonts-Klasse zum anzeigen von
    /// Texten wird den Projekt noch hinzugefügt.
    /// </summary>
    public class Menu : DrawableGameComponent, IMenu
    {
        #region Data Member        
        //Für Optionen
        private int difficult;
        private int activeDifficult;
        private String spielernamen;
        private static Vector2 mousepos = new Vector2(0,0);
        
        private MenuControl steuerung;

        private string currentMenu;

        //Textanzeige
        private SpriteFont font;
        private Texture2D textfield;

        private Vector2 mouseMovement;

        //Für Mauszeigerposition
        private Rectangle mouseInRec;

        //Für XML
        private MenuDescription menuXml;

        private string assetName = String.Empty; // Gibt den Speicherort der XML-Config an.         

        const string HAUPTMENU = "Hauptmenu";
        const string OPTIONENMENU = "Optionen";

        public HighscoreMenu HighscoreMenu;
        public Option OptionMenu;

        private bool action = false;

        private IScore score;

        //Buttons
        // public string menuName;
        private ButtonList currentList;
        private IMainList shifts;
        // private PromptCollection Prompts;
        public Prompt prompt;
        private bool scoring;
        private bool startscoring;

        //Für Graphiken
        private Texture2D background;
        private Texture2D mouse;
        private SpriteBatch SpriteBatch;

        private ICanyonShooterGame game;

        bool visible;
        bool isActive;        
        #endregion

        #region Properties

        public IMainList Shifts
        {
            get { return shifts; }
            set { shifts = value; }
        }

        public bool Activity
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public bool Visiblity
        {
            get { return visible; }
            set { visible = value; }
        }
        /*
        public PromptCollection PromptsCollection
        {
            get { return Prompts; }
        }
        */
        public string CurrentMenu
        {
            get { return currentMenu; }
            set { currentMenu = value; }
        }

        public int Difficult
        {
            get { return difficult; }
            set { difficult = value; }
        }

        public int ActiveDifficult
        {
            get { return activeDifficult; }
            set { activeDifficult = value; }
        }

        public MenuDescription MenuXml
        {
            get { return menuXml; }
            set { menuXml = value; }
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

        public String Spielernamen
        {
            get { return spielernamen; }
            set { spielernamen = value; }
        }

        public Vector2 Mouse
        {
            get { return mouseMovement; }
        }
       

        #endregion

        private Texture2D red;

        //Constructor
        public Menu(ICanyonShooterGame game, string name)
            : base(game as Game)
        {
            assetName = "Config\\" + name + ".xml";
            this.game = game;
            this.currentMenu = name;
            menuXml = game.Content.Load<MenuDescription>("Content\\Menu\\" + name);
            this.visible = true;
            this.isActive = true;
            
            steuerung = new MenuControl(this, game);

            //Prompts = PromptCollection.getInstance();

            //Erstellung des Hauptmenüs
            MainList Menu = new MainList(game, name, menuXml, true);
            CurrentList = Menu;
            shifts = Menu;

            //this.OptionMenu = new Option(game,this.menuXml);

            this.DrawOrder = (int)DrawOrderType.Menu;
        }
        /// <summary>
        /// Constructor for Including Score
        ///
        /// </summary>
        public Menu(ICanyonShooterGame game, string name, bool score)
            : base(game as Game)
        {
            assetName = "Config\\" + name + ".xml";
            this.game = game;
            this.currentMenu = name;
            menuXml = game.Content.Load<MenuDescription>("Content\\Menu\\" + name);
            this.visible = true;
            this.isActive = true;

            steuerung = new MenuControl(this, game);

            //Erstellung des Hauptmenüs
            MainList Menu = new MainList(game, name, menuXml, true);
            CurrentList = Menu;
            shifts = Menu;
            this.scoring = score;            
            if (score)
            {
                this.startscoring = true;
                this.score = game.GameStates.Score;
            }
            
            this.DrawOrder = (int)DrawOrderType.Menu;
        }
        #region SaveScore
        private void SaveScore()
        {
            prompt = new Prompt(game, "SaveScore", game.Graphics.Device.Viewport.Width,
                                                    game.Graphics.Device.Viewport.Height, new string[] { 
                                                    "Do you want to save your Score?","Your score: " + score.Highscore,
                                                    "Submit Score online: ",
                                                    "Playername: "},
                                                    false,game.GameStates.Profil.CurrentProfil.Playername,true,true);
            prompt.accept += new PromptEventHandler(CommitScore);
            prompt.decline += new PromptEventHandler(Cancel);
            prompt.checking +=new PromptEventHandler(changeOnlineStatus);
            prompt.intiateButtons();
            if (game.GameStates.Profil.CurrentProfil.OnlineHighscore)
            {
                changeOnlineStatus();
            }                         
            this.Activity = false;
        }

        private void CommitScore() 
        {
            prompt.SetPlayerName(game.GameStates.Profil.SpielereinstellungenList, "Playername");
            game.GameStates.Profil.writeProfilSpielereinstellungen(game.GameStates.Profil.SpielereinstellungenList);
            this.score.Playername = game.GameStates.Profil.CurrentProfil.Playername;
            game.Highscores.AddScore((Score)this.score, this.game,this.game.GameStates.Profil.CurrentProfil.Difficult, true);
            if (prompt.Promptchecked)
            {
                Thread thread = new Thread(OnlineScore.PostScore);
                thread.Start(game);
            }
            prompt.Activity = false;
            prompt.Visible = false;
            scoring = false;
            prompt.unloadPrompt();
            this.Activity = true;
        }

        private void Cancel()
        {
            prompt.Activity = false;
            prompt.Visible = false;
            scoring = false;
            prompt.unloadPrompt();
            this.Activity = true;
        }

        public void changeOnlineStatus()
        {
            if (prompt.Promptchecked)
            {
                prompt.CheckBox.changeImage("promptCheck1");
                prompt.Promptchecked = false;
            }
            else
            {
                if (!prompt.Promptchecked)
                {
                    prompt.CheckBox.changeImage("promptCheck2");
                    prompt.Promptchecked = true;
                }
            }
        }

        #endregion

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
            
            #region Hauptmenü
            if (scoring)
            {
                if (this.startscoring && this.score!=null)
                {
                    SaveScore();
                    prompt.LoadContent(game);
                    this.startscoring = false;
                }         
            }
            if (Visiblity && isActive)
            {
                if (Action)
                {
                    #region Mouse
                    //Auswahl tätigen
                    if (steuerung.isMouseInShift() &&
                        new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse) 
                    {
                    }
                    if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
                    {
                        if (steuerung.isMouseInShift())
                        {
                             steuerung.changeButton();
                        }
                        else if (steuerung.isMouseInBox() && CurrentButton.getPosition() == MouseInRec)
                        {
                            steuerung.selection();
                            LoadContent();
                        }
                    }
                    #endregion

                    #region Keyboard
                    if (game.Input.HasKeyJustBeenPressed("Menu.Left"))
                    {
                        steuerung.changeButton(Keys.Left);
                    }

                    else if (game.Input.HasKeyJustBeenPressed("Menu.F2"))
                    {
                        game.HasSpaceMouse = !game.HasSpaceMouse;
                    }

                    else if (game.Input.HasKeyJustBeenPressed("Menu.Right"))
                    {
                        steuerung.changeButton(Keys.Right);
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                    {
                        Shifts.LeftShift.resetActive();
                        Shifts.RightShift.resetActive();
                        steuerung.selection();
                        LoadContent();
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
                    {
                        Shifts.LeftShift.resetActive();
                        Shifts.RightShift.resetActive();
                        steuerung.Back(CurrentMenu);
                        LoadContent();
                    }
                    #endregion
                }
            }
            #endregion

            else if (HighscoreMenu != null && HighscoreMenu.Visiblity)
            {
                HighscoreMenu.Update(gameTime, SpriteBatch);
            }
            else if (OptionMenu != null && OptionMenu.Visiblity)
            {
               OptionMenu.Update(gameTime, SpriteBatch);
            }

            if (prompt != null)
            {
                prompt.Update(gameTime);
            }

            if (mousepos != MousePosition)
            {
                mouseMovement = MousePosition;
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

                if (scoring && prompt != null)
                {
                    prompt.Draw(gameTime,SpriteBatch);
                }
                #region Hauptmenü
                if (Visiblity)
                {
                    //if (Action)
                    {

                        foreach (MenupartDescription i in menuXml.MenuParts)
                        {
                            if (i.Name.Equals(CurrentMenu))
                            {
                                SpriteBatch.Draw(background, new Rectangle(0, 0,
                                    game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height),
                                    Color.White);
                            }
                        }
                        CurrentList.Draw(gameTime, SpriteBatch);
                    }
                    SpriteBatch.DrawString(font, "Press F2 for using 3D mouse",
                            new Vector2(10, game.Graphics.Device.Viewport.Height - 70), Color.White);
                    if (game.HasSpaceMouse)
                    {
                        SpriteBatch.Draw(red, new Rectangle(10, game.Graphics.Device.Viewport.Height - 35, 
                            16, 16), Color.White);
                    }
                }
                #endregion

                else if (HighscoreMenu != null && HighscoreMenu.Visiblity)
                {
                    HighscoreMenu.Draw(gameTime, SpriteBatch);
                }
                else if (OptionMenu != null && OptionMenu.Visiblity)
                {
                    OptionMenu.Draw(gameTime, SpriteBatch);
                }
                if (prompt != null)
                {
                    prompt.Draw(gameTime, SpriteBatch);
                }
                /*if (Prompts.Count != 0)
                {
                    Prompts.Draw(gameTime, SpriteBatch);
                }*/
                SpriteBatch.Draw(mouse, MousePosition, Color.White);
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
            if (scoring && prompt!=null)
            {
                prompt.LoadContent(game);
            }
            SpriteBatch = new SpriteBatch(game.Graphics.Device);
            foreach (MenupartDescription i in menuXml.MenuParts)
            {
                if (i.Name.Equals(CurrentMenu))
                {
                    background = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + i.BackgroundImage);
                    mouse = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + i.Mouse);               
                }
            }

            textfield = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + "Textfield");
            CurrentList.LoadContent();
            font = game.Content.Load<SpriteFont>("Arial");
            red = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + "3dmousered");
            if (prompt != null)
            {
                prompt.LoadContent(game);
            }
            if (HighscoreMenu != null && HighscoreMenu.Visiblity)
            {
                HighscoreMenu.LoadContent();
            }
            if (OptionMenu != null && OptionMenu.Visiblity)
            {
                OptionMenu.LoadContent();
            }

        }



        #region IMenu Member

        public string Name
        {
            get { return currentMenu; }
        }

        public bool Action
        {
            get { return action; }
            set { action = value; }
        }

        public void load()
        {
            LoadContent();
        }

        #endregion

        //Für Optionen
        public enum DifficultEnum
        {
            leicht = 0,
            mittel = 1,
            schwer = 2
        }

        public void addPrompt(Prompt prompt) {
            this.prompt = prompt;
        }

        public void removePrompt()
        {
            this.prompt = null;
        }

    }
}


