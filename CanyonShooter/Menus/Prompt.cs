#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CanyonShooter.Engine.Helper;

#endregion

namespace CanyonShooter.Menus
{
    //Eventhändler
    public delegate void PromptEventHandler();
    //------------
    public class Prompt : DrawableGameComponent
    {

        #region static Data Members
        const int height = 200;
        const int width = 400;
        private static Vector2 button1 = new Vector2(14, 159);
        private static Vector2 button2 = new Vector2(232, 159);
        private static Vector2 buttoncheck = new Vector2(220, 67);
        const int Butwidth = 154;
        const int Butheight = 33;
        const int Checkwidth = 15;
        const int CheckHeight = 15;
        private static Vector2 textpos = new Vector2(12, 21);
        private static Vector2 textoffset = new Vector2(0, 20);
        #endregion

        #region Data Members
        private ICanyonShooterGame game;
        private string name;

        public Rectangle Postion;
        private Button Accept;
        private Button Decline;
        private Button checkBox;
        private string[] text;

        private DateTime startTime;
        private DateTime currentTime;

        private bool reqInput;
        private bool timefixed;
        private int gotTime;

        private Texture2D promptback;
        private SpriteFont font;
        private Texture2D dither;

        private bool visible;
        private bool isActive;

        private bool checkprompt;
        private bool promptchecked = false; //checkbox checked or not

        private bool ButtonsRDY = false;

        //Test
        private bool write = false;
        private string writeText;
        private int counter;
        private int maxcount;
        private KeyboardState key;
        private bool keyload = false;
        private Keys[] keysarray;
        private Keys currentKey;
        private bool keystart = false;

        // ---------- Ereignisse ---------- 
        public event PromptEventHandler accept;
        public event PromptEventHandler decline; // Eventuell eine Methode die den Prompt beendet und keine Änderungen initiert.
        public event PromptEventHandler checking;
        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Activity
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public Rectangle Position
        {
            get { return this.Postion; }
            set { Postion = value; }
        }

        public bool Visiblity
        {
            get { return visible; }
            set { visible = value; }
        }

        public string[] Text
        {
            get { return text; }
            set { text = value; }
        }

        public Keys CurrentKey
        {
            get { return currentKey; }
            set { currentKey = value; }
        }


        public bool Promptchecked
        {
            get { return promptchecked; }
            set { promptchecked = value; }
        }

        public Button CheckBox
        {
            get { return checkBox; }
            set { checkBox = value; }
        }
        #endregion
        #region Constructor

        /*
         * Standard Konstruktor
         */
        public Prompt(ICanyonShooterGame game, string name, int resX, int resY, string[] text, bool input)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            this.isActive = true;
            this.Postion.Width = width;
            this.Postion.Height = height;
            this.Postion.X = (int)resX / 2 - (width / 2);
            this.Postion.Y = (int)resY / 2 - (height / 2);
            initiate(text, input);
        }
        /*
         * Konsturktor für eigene Positionen
         */
        public Prompt(ICanyonShooterGame game, string name, Vector2 startpos, string[] text, bool input)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            this.isActive = true;
            this.Postion.Width = width;
            this.Postion.Height = height;
            this.Postion.X = (int)startpos.X;
            this.Postion.Y = (int)startpos.Y;
            initiate(text, input);
        }

        /*
         * Constructor for timebased prompt
         */
        public Prompt(ICanyonShooterGame game, string name, int resX, int resY, bool time, string[] text, bool input, int gottime)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            this.isActive = true;
            this.Postion.Width = width;
            this.Postion.Height = height;
            this.Postion.X = (int)resX / 2 - (width / 2);
            this.Postion.Y = (int)resY / 2 - (height / 2);
            this.startTime = DateTime.Now;
            this.timefixed = true;
            this.gotTime = gottime;
            initiate(text, input);
        }


        /// <summary>
        /// Constructor for Prompt with textinput
        /// </summary>
        /// <param name="game"></param>
        /// <param name="name"></param>
        /// <param name="resX"></param>
        /// <param name="resY"></param>
        /// <param name="text"></param>
        /// <param name="input"></param>
        public Prompt(ICanyonShooterGame game, string name, int resX, int resY, string[] text,
            bool input, string writeText, bool write)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            this.isActive = true;
            this.Postion.Width = width;
            this.Postion.Height = height;
            this.Postion.X = (int)resX / 2 - (width / 2);
            this.Postion.Y = (int)resY / 2 - (height / 2);
            this.write = write;
            this.writeText = writeText;
            initiate(text, input);
            this.counter = writeText.Length;
            this.maxcount = 15;
        }

        /// <summary>
        /// Constructor for Prompt with textinput and checkbox
        /// </summary>
        /// <param name="game"></param>
        /// <param name="name"></param>
        /// <param name="resX"></param>
        /// <param name="resY"></param>
        /// <param name="text"></param>
        /// <param name="input"></param>
        public Prompt(ICanyonShooterGame game, string name, int resX, int resY, string[] text,
            bool input, string writeText, bool write, bool check)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            this.isActive = true;
            this.Postion.Width = width;
            this.Postion.Height = height;
            this.Postion.X = (int)resX / 2 - (width / 2);
            this.Postion.Y = (int)resY / 2 - (height / 2);
            this.write = write;
            this.writeText = writeText;            
            this.counter = writeText.Length;
            this.maxcount = 15;
            this.checkprompt = check;
            initiate(text, input);
        }

        /// <summary>
        /// Constructor for Prompt with Keyinput
        /// </summary>
        /// <param name="game"></param>
        /// <param name="name"></param>
        /// <param name="resX"></param>
        /// <param name="resY"></param>
        /// <param name="text"></param>
        /// <param name="input"></param>
        /// <param name="keyload"></param>
        public Prompt(ICanyonShooterGame game, string name, int resX, int resY, string[] text,
            bool input, bool keyload)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            this.isActive = true;
            this.Postion.Width = width;
            this.Postion.Height = height;
            this.Postion.X = (int)resX / 2 - (width / 2);
            this.Postion.Y = (int)resY / 2 - (height / 2);
            this.keyload = keyload;
            initiate(text, input);
        }
        #endregion
        public void initiate(string[] text, bool input)
        {
            this.text = text;
            this.reqInput = input;
            Accept = new Button(game, "Annehmen", calcVec(Postion, button2), Butheight, Butwidth, "promptacc", true, this);
            Decline = new Button(game, "Ablehnen", calcVec(Postion, button1), Butheight, Butwidth, "promptdec", true, this);
            if (checkprompt)
            {
                checkBox = new Button(game, "Check", calcVec(Position, buttoncheck), CheckHeight, Checkwidth, "promptCheck1", true, this);
            }
            if (accept != null && decline != null)
            {
                intiateButtons();
            }
        }
        
        public void intiateButtons()
        {
            if (!accept.Equals(null) && !decline.Equals(null))
            {
                Accept.Buttonpressed += new ActionEventHandler(accept);
                Decline.Buttonpressed += new ActionEventHandler(decline);
                if (checkprompt)
                {
                    checkBox.Buttonpressed += new ActionEventHandler(checking);
                }
                ButtonsRDY = true;
                if (ButtonsRDY)
                {
                    this.Visiblity = true;
                    game.GameStates.Menu.addPrompt(this);
                    LoadContent(game);
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            if (this.Visiblity)
            {
                if (ButtonsRDY)
                {
                    SpriteBatch.Draw(dither, new Rectangle(0, 0, game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height), Color.Black);
                    Vector2 currentpos = calcVec(Postion, textpos);
                    //SpriteBatch.Begin();
                    SpriteBatch.Draw(promptback, Postion, Color.White);
                    for (int i = 0; i < text.Length; i++)
                    {
                        SpriteBatch.DrawString(font, text[i], currentpos, Color.White);
                        currentpos += textoffset;
                    }
                    Accept.Draw(gameTime, SpriteBatch);
                    Decline.Draw(gameTime, SpriteBatch);
                    if (checkprompt)
                    {
                        checkBox.Draw(gameTime, SpriteBatch);
                    }
                    if (write)
                    {
                        currentpos += textoffset;
                        if (counter == maxcount)
                        {
                            SpriteBatch.DrawString(font, this.writeText, currentpos, Color.White);
                        }
                        else
                        {
                            SpriteBatch.DrawString(font, this.writeText + "_", currentpos, Color.White);
                        }
                    }

                    if (keyload)
                    {
                        if (!keystart)
                        {
                            key = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                            this.keysarray = key.GetPressedKeys();
                            if (this.keysarray.Length > 0)
                            {
                                currentKey = this.keysarray[0];
                                if (currentKey == Keys.Enter || currentKey == Keys.Escape)
                                {
                                    currentKey = Keys.None;
                                }
                            }   
                        }
                        
                        currentpos += textoffset;
                        if (currentKey.Equals(Keys.None))
                        {
                            SpriteBatch.DrawString(font, "_", currentpos, Color.White);
                        }
                        else
                        {
                            keystart = true;
                            SpriteBatch.DrawString(font, currentKey.ToString(), currentpos, Color.White);
                        }

                    }

                    //SpriteBatch.End();
                }
                else
                {
                    intiateButtons();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Visiblity && this.isActive)
            {
              if (timefixed)
                {
                    this.currentTime = DateTime.Now;
                    TimeSpan elapsedTime = TimeSpan.FromSeconds(gotTime) -(currentTime - startTime);
                    this.text[text.Length-1] = "Time left to accept:     " + elapsedTime.Minutes+":"+elapsedTime.Seconds;
                    if (elapsedTime <= TimeSpan.Zero)
                    {
                        this.decline();
                    }
                }
                if ( Helper.Lock("Pause für dumme Programmierer",TimeSpan.FromMilliseconds(1000)))
                {
                    this.controlling();
                }
               
                if (write)
                {
                    if ((counter < maxcount) || (game.Input.HasKeyJustBeenPressed("Menu.Back")))
                    {
                        game.Input.HandleKeyboardInput(ref writeText);
                        counter = writeText.Length;
                    }
                }
            }

        }

        public void LoadContent(ICanyonShooterGame game)
        {
            if (ButtonsRDY)
            {
                dither = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\dither");
                promptback = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\prompt");
                Accept.LoadContent(game);
                Decline.LoadContent(game);
                if (checkprompt)
                {
                    checkBox.LoadContent(game);
                }
                font = game.Content.Load<SpriteFont>("Arial");
            }
            else
            {
                intiateButtons();
            }
        }

        private Vector2 calcVec(Rectangle a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public void unloadPrompt()
        {
             
            this.write = false;
            this.Visiblity = false;
            game.GameStates.Menu.removePrompt();
            game.GameStates.Menu.Action = true;
        }
        //Check if there is a copy of Key before setting the new Key!
        public void SetNewKey(Dictionary<string, object> Data, string Name)
        {
            
                foreach (string i in Data.Keys)
                {
                    if (this.currentKey.Equals(Data[i]) && i != this.name)
                    {
                        Data[i] = Keys.None;
                        break;
                    }
                }
            Data[Name] = currentKey;
        }

        public void SetPlayerName(Dictionary<string, object> Data, string Name)
        {
            Data[Name] = this.writeText;
        }

        #region Steuerung
        private void controlling()
        {
            if (this.Visible && this.isActive)
            {
                #region Mouse
                //Auswahl tätigen
                if (isMouseInBox(Accept))
                {
                    Accept.SetActive();
                    Decline.resetActive();
                    if (checkprompt)
                    {
                        checkBox.resetActive();
                    }
                    if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
                    {
                        Accept.ButtonPressed();
                    }
                }
                else
                {
                    if (isMouseInBox(Decline))
                    {
                        Accept.resetActive();
                        Decline.SetActive();
                        if (checkprompt)
                        {
                            checkBox.resetActive();
                        }
                        if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
                        {
                            Decline.ButtonPressed();
                        }
                    }
                    else
                    {
                        if (checkprompt)
                        {
                            if (isMouseInBox(checkBox))
                            {
                                Accept.resetActive();
                                Decline.resetActive();
                                checkBox.SetActive();
                                if (game.Input.HasKeyJustBeenPressed("Menu.Select") && checkBox.GetActive())
                                {                                   
                                        checkBox.ButtonPressed();
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Keyboard
                if (game.Input.HasKeyJustBeenPressed("Menu.Left"))
                {
                    if (Accept.GetActive())
                    {
                        Accept.resetActive();
                        Decline.SetActive();
                    }
                    else
                    {
                        if (Decline.GetActive())
                        {
                            Decline.resetActive();
                            if (checkprompt)
                            {
                                checkBox.SetActive();
                            }
                            else
                            {
                                Accept.SetActive();
                            }
                        }
                        else
                        {
                            if (checkprompt)
                            {
                                if (this.checkBox.GetActive())
                                {
                                    checkBox.resetActive();
                                    Accept.SetActive();
                                }
                            }
                            else if (!Accept.GetActive() && !Decline.GetActive())
                            {
                                Accept.SetActive();
                            }
                        }
                    }
                }
                else if (game.Input.HasKeyJustBeenPressed("Menu.Right"))
                {
                    if (Accept.GetActive())
                    {
                        Accept.resetActive();
                        if (checkprompt)
                        {
                            checkBox.SetActive();
                        }
                        else
                        {
                            Decline.SetActive();
                        }
                    }
                    else
                    {
                        if (Decline.GetActive())
                        {
                            Decline.resetActive();
                            Accept.SetActive();
                        }
                        else
                        {
                            if (checkprompt)
                            {
                                if (this.checkBox.GetActive())
                                {
                                    checkBox.resetActive();
                                    Decline.SetActive();
                                }
                            }
                            else if (!Accept.GetActive() && !Decline.GetActive())
                            {
                                Accept.SetActive();
                            }
                        }
                    }

                }
                else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                {
                    if (Decline.GetActive())
                    {
                        Decline.ButtonPressed();
                    }
                    else if (Accept.GetActive())
                    {
                        Accept.ButtonPressed();       
                    }         
                    else if (checkprompt)
                    {
                        
                        if (checkBox.GetActive())
                        {
                            checkBox.ButtonPressed();                            
                        }
                    }
                }
                else if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
                {
                    Decline.SetActive();
                    Decline.ButtonPressed();
                }
                #endregion
            }

        }

        public bool isMouseInBox(Button Check)
        {
            if (this.Visiblity)
            {
                if ((game.GameStates.Menu.MousePosition.X >= Check.getPosition().X) &&
                    (game.GameStates.Menu.MousePosition.X <= Check.getPosition().Right) &&
                    (game.GameStates.Menu.MousePosition.Y >= Check.getPosition().Y) &&
                    (game.GameStates.Menu.MousePosition.Y <= Check.getPosition().Bottom))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}
