#region Using Statements
using System;
using System.Collections.Generic;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace CanyonShooter.Menus
{
    public class Listbox: HighscoreSupporter
    {

        #region Data Members

        private string Titel;
        //Grafik
        private SpriteFont font;
        private Texture2D header;
        private Texture2D leftside;
        private Texture2D rightside;
        private Texture2D background;
        private Texture2D statusrow;
        private Texture2D buttonsback;
        //Größe
        private Vector2 Position;
        private int BoxWidth = 400;
        private int BoxHeight = 511;
        private int BoxContentHeight;
        private int BoxContentWidth;
        private Vector2 BoxContentPosition;
        
        private bool scrollable;
        private bool visible;
        private bool isActive;
        
        //For the OptionList
        private Dictionary<string, object> data;

        public ButtonList List;
        public ButtonList Buttons;
        private Scrollbar scrollbar;

        private ICanyonShooterGame game;

        private int checkNumber;
        private int style;

        private static Vector2 emergencyInfo = new Vector2(10,10);

        private Dictionary<string, HSlideBar> slideBars;

        #endregion

        #region Properties

        internal Dictionary<string, HSlideBar> SlideBars
        {
            get { return slideBars; }
        }

        public bool Visiblity
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        public bool Activity
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public Dictionary<string, object> Data
        {
            get { return data; }
        }

        public Scrollbar Scrollbar
        {
            get { return scrollbar; }
            set { scrollbar = value; }
        }
        #endregion

        //Konstruktor
        public Listbox(ICanyonShooterGame game, Vector2 pos, bool scroll, MenuDescription xml, int difficulty) 
            : base(difficulty)
        {
            this.isActive = true;
            this.visible = true;
            this.game = game;
            this.style = 0;
            
            this.Position = pos;
            this.scrollable = scroll;

            this.BoxContentPosition = new Vector2(Position.X + 7, Position.Y + 30);
            this.BoxContentHeight = this.BoxHeight - 30 - 31;
            this.Titel = "Bestenliste: ";

            switch (difficulty)
            {
                case 0:
                    this.Titel += "Rooky";
                    break;
                case 1:
                    this.Titel += "Standard";
                    break;
                case 2:
                    this.Titel += "Elite";
                    break;
                default:
                    break;                
            }

            //Building the Member for the Buttons and Scores
            this.List = new HighscoreList(game,"ScoreList",BoxContentPosition,game.Content.Load<HighscoreDescription>("Content\\Menu\\HighscoreMenu"),false,this);
            this.Buttons = new NormalList(game, ButtonPosition(xml), "Score", xml, true);
            this.Scrolling();
            this.intializeScrollbar();
        }

        public Listbox(ICanyonShooterGame game, Vector2 pos, bool scroll, MenuDescription xml, int district, Dictionary<string, object> Data,Dictionary<string, string> Format, List<string> Names)
            : base(district)
        {
            this.isActive = true;
            this.visible = true;
            this.game = game;
            this.style = 1;

            this.Position = pos;
            this.scrollable = scroll;

            this.data = Data;

            this.BoxContentPosition = new Vector2(Position.X + 7, Position.Y + 30);
            this.BoxContentHeight = this.BoxHeight - 30 - 31;
            this.Titel = "Optionen: ";

            switch (difficulty)
            {
                case 0:
                    this.Titel += "Spielereinstellungen";
                    break;
                case 1:
                    this.Titel += "Grafik";
                    break;
                case 2:
                    this.Titel += "Sound";
                    break;
                case 3:
                    this.Titel += "Steuerung";
                    break;
                default:
                    break;
            }

            //Building the Member for the Buttons and Scores
            this.List = new OptionList(game, game.Content.Load<OptionDescription>("Content\\Menu\\OptionMenu"), district, BoxContentPosition, Data, Format, Names, false,this);
            this.Buttons = new NormalList(game,ButtonPosition(xml), "OptionList", xml, true);
            this.Scrolling();
            this.intializeScrollbar();

            slideBars = new Dictionary<string, HSlideBar>();
            int counter = Names.Count;
            foreach (Button g in this.List)
            {
                HSlideBar hsb = getSlideBar(g, Names[Names.Count - counter]);
                if (hsb != null)
                {
                    slideBars.Add(g.getButtonName(), hsb);
                }

                counter--;
            }

        }

        private Vector2 ButtonPosition(MenuDescription xml)
        {
            float high = 0;
            foreach (MenupartDescription i in xml.MenuParts)
            {
                if (i.Name == "Score" && style == 0)
                {
                    foreach (ButtonDescription g in i.Buttons)
                    {
                        if (high < g.height)
                        {
                            high = g.height;
                        }
                    }
                }
                else
                {
                    if (i.Name == "OptionList" && style == 1)
                    {
                        foreach (ButtonDescription g in i.Buttons)
                        {
                            if (high < g.height)
                            {
                                high = g.height;
                            }
                        }
                    }
                }
            }
            return this.Position + new Vector2(0, BoxHeight);
        }

        private void intializeScrollbar()
        {
            if (this.scrollable)
            {
                int counter = 0;
                int ButtonWidth = 0;
                int ButtonHeight = 0;
                foreach (Button i in List)
                {
                    if (ButtonWidth < i.Width)
                    {
                        ButtonWidth = i.Width;
                    }
                    if (ButtonHeight < i.Height)
                    {
                        ButtonHeight = i.Height;
                    }
                    counter++;
                }
                this.scrollbar = new Scrollbar(this.game, new Vector2(this.BoxContentPosition.X + this.BoxContentWidth, this.BoxContentPosition.Y), this.BoxContentHeight, counter * ButtonHeight, this.BoxContentHeight, ButtonHeight,List.currentNumber,3);
            }
        }

        private void Scrolling() {
            if (scrollable) 
            {
                int size=0;
                foreach (Button i in this.List)
                {
                    size = size + i.Height;
                }
                if (size <= this.BoxContentHeight)
                {
                    scrollable = false;
                }
            }

            if (scrollable)
            {
                ScrollBarDescription xml = game.Content.Load<ScrollBarDescription>("Content\\Menu\\ScrollBarXML");
                this.BoxContentWidth = this.BoxWidth - 7 - 7 - xml.UnitWidth - 1;
            }
            else
            {
                this.BoxContentWidth = this.BoxWidth - 7 - 7;
            }

            foreach (Button i in this.List)
            {
                i.Width = this.BoxContentWidth;
                i.Position = new Rectangle(i.Position.X,i.Position.Y,this.BoxContentWidth,i.Position.Height);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch loader)
        {
            if (visible)
            {
                loader.Draw(background, calculateRectangle(new Vector2(Position.X + 7, Position.Y + 30), 450, 386), Color.White);
                this.DrawScoreList(gameTime, loader);
                loader.Draw(header, calculateRectangle(Position, 30, 400), Color.White);
                loader.DrawString(font, this.Titel, new Vector2(Position.X+10,Position.Y+3), Color.Orange);
                loader.Draw(leftside, calculateRectangle(new Vector2(Position.X, Position.Y + 30), 450, 7), Color.White);
                loader.Draw(rightside, calculateRectangle(new Vector2(Position.X + 7 + 386, Position.Y + 30), 450, 7), Color.White);
                loader.Draw(statusrow, calculateRectangle(new Vector2(Position.X, Position.Y + 30 + 450), 31, 400), Color.White);
                loader.Draw(buttonsback, calculateRectangle(new Vector2(Position.X, Position.Y + 30 + 450+31), 30, 400), Color.White);
                if (scrollable)
                {
                    scrollbar.Draw(gameTime, loader);
                }
                Buttons.Draw(gameTime, loader);
            }
        }

        private void DrawScoreList(GameTime gameTime, SpriteBatch loader)
        {
            if (List.Count != 0)
            {
                List.Draw(gameTime, loader);
                if (this.style == 0)
                {
                    int counter = 0;
                    foreach (Button i in List)
                    {
                        if (i.Visiblity)
                        {
                            if (game.Highscores.GetScores(Difficulty).Count != 0)
                            {
                                loader.DrawString(font, "" + (counter + 1) + ".", calcMidLine(i.Position, 0, "" + (counter + 1) + "."), Color.Beige);
                                loader.DrawString(font, i.getButtonName(), calcMidLine(i.Position, 1, i.getButtonName()), Color.White);
                                loader.DrawString(font, game.Highscores.GetScores(Difficulty)[counter].Highscore.ToString(), calcMidLine(i.Position, 2, game.Highscores.GetScores(Difficulty)[counter].Highscore.ToString()), Color.Red);
                                counter++;
                            }
                        }
                    }
                }
                else if (this.style == 1)
                {
                    int counter = 0;
                    foreach (Button i in List)
                    {
                        if (i.Visiblity)
                        {
                            i.Draw(gameTime, loader);
                            loader.DrawString(font, i.getButtonName(), calcMidLine(i.Position, 0, i.getButtonName()), Color.White);
                            if (SlideBars.ContainsKey(i.getButtonName()))
                            {
                                SlideBars[i.getButtonName()].Draw(gameTime, loader);
                            }
                            else
                            {
                                loader.DrawString(font, "" + data[i.getButtonName()], calcMidLine(i.Position, 2, "" + data[i.getButtonName()]), Color.White);
                            }
                            counter++;
                        }
                    }
                }
            }
            else
            {
                loader.DrawString(font, "No Data for this Area!", (BoxContentPosition + emergencyInfo), Color.OrangeRed);
            }
        }

        public void Update(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (this.Activity)
            {  
                this.init();
                List.Update(gameTime, spritebatch);
                Buttons.Update(gameTime, spritebatch);
                if (scrollable)
                {
                    int counter = 0;
                    updateAktivity();
                    foreach (Button g in this.List)
                    {
                        if (g.Equals(List.currentNode.Value))
                        {
                            scrollbar.Update(gameTime, counter);
                        }
                        counter++;
                    }
                }
            }
        }

        public void LoadContent(ICanyonShooterGame game)
        {
            font = game.Content.Load<SpriteFont>("Arial");
            header = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\listboxheader");
            leftside = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\listboxleft");
            rightside = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\listboxright");
            background = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\listboxback");
            statusrow = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\listboxstatus");            
            buttonsback = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\buttonback");
            if (scrollable)
            {
               scrollbar.LoadContent();
            }
            List.LoadContent();
            Buttons.LoadContent();
            foreach (Button i in this.List)
            {
                if (SlideBars != null)
                {
                    if (SlideBars.ContainsKey(i.getButtonName()))
                    {
                        SlideBars[i.getButtonName()].LoadContent();
                    }
                }
            }
        }

        public void updateAktivity()
        {
            
            int number = scrollbar.checkPosition();
            if (number != checkNumber)
            {
                if (number < List.Count)
                {
                    List.currentNode.Value.resetActive();
                    int save = List.currentNumber;
                    List.currentNode = List.First;
                    List.currentNumber = 0;
                    List.currentNode.Value.SetActive();
                    for (int i = 0; i <= number; i++)
                    {
                        List.Next();
                    }
                }
                this.checkNumber = number;
            }
        }

        private Rectangle calculateRectangle(Vector2 pos, int height, int width)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }

        //Überlappende Buttons müssen abgeschnitten werden!!!!
        public void init()
        {

            foreach (Button g in List)
            {
                if (g.GetActive() && g.Position.Top >= BoxContentPosition.Y && g.Position.Bottom <= BoxContentPosition.Y + BoxContentHeight)
                {
                    foreach (Button h in List)
                    {
                        if (this.BoxContentWidth != h.Position.Width)
                        {
                            h.Position = new Rectangle(h.Position.X,h.Position.Y, h.Width, h.Height);
                        }
                        if (h.Position.Bottom <= BoxContentPosition.Y || h.Position.Top >= (BoxContentPosition.Y + BoxContentHeight))
                        {
                            h.Visiblity = false;
                        }
                        else
                        {
                            h.Visiblity = true;
                        }
                    }
                }
                else 
                {
                   if (g.GetActive() && g.Position.Top < BoxContentPosition.Y)
                    {
                      float dif1 = (BoxContentPosition.Y - g.Position.Top);
                      foreach (Button h in List)
                      {
                          h.Position = new Rectangle(h.Position.X,(int) (h.Position.Y + dif1),h.Width,h.Height);
                          if (h.Position.Bottom <= BoxContentPosition.Y || h.Position.Top >= (BoxContentPosition.Y + BoxContentHeight))
                          {
                              h.Visiblity = false;
                          }
                          else
                          {
                              h.Visiblity = true;
                          }
                      }
                    }
                    else
                    {
                        if (g.GetActive() && g.Position.Bottom > BoxContentPosition.Y + BoxContentHeight)
                        {
                            float dif2 = (g.Position.Bottom - (BoxContentPosition.Y + BoxContentHeight));
                            foreach (Button h in List)
                            {    
                                //h.Visiblity = true;
                                h.Position = new Rectangle(h.Position.X,(int)(h.Position.Y - dif2),h.Width,h.Height);
                                if (h.Position.Bottom <= BoxContentPosition.Y || h.Position.Top > (BoxContentPosition.Y + BoxContentHeight))
                                {
                                    h.Visiblity = false;
                                }
                                else
                                {
                                    h.Visiblity = true;
                                }
                            }
                        }
                    }
                }

            }

        }//

        private Vector2 calcMidLine(Rectangle Position, int first, string output)
        {
            float line = (Position.Height / 2);
            Vector2 fontsize = font.MeasureString(output);
            if (first == 0)
            {
                return new Vector2(Position.X + 7, Position.Y + line - (fontsize.Y / 2));
            }
            else
            {
                if (first == 1)
                {
                    return new Vector2(Position.X + 30, Position.Y + line - (fontsize.Y / 2));
                }
                else if (first == 2)
                {
                    float score = Position.Width * 85 / 100;
                    return new Vector2(Position.X + score- (fontsize.X/2), Position.Y + line - (fontsize.Y / 2));
                }
            }
            return new Vector2(Position.X, Position.Y);
        }

        /// <summary>
        /// Return the needed Slidebar
        /// </summary>
        /// <param name="but">used Button</param>
        /// <param name="Name">Buttonname</param>
        /// <returns>Slidebar</returns>
        private HSlideBar getSlideBar(Button but, string Name)
        {
            Vector2 space = new Vector2(but.Width * 50 / 100, 0);
            switch (Name)
            {
                case "Translation":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 1.0f, (float)Data[Name], 0.1f, 2, 10);
                    }
                case "Acceleration Level":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 20.0f, (float)Data[Name], 2.0f, 5, 1);
                    }
                case "Brake":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 20.0f, (float)Data[Name], 2.0f, 5, 1);
                    }
                case "Banking":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 2.0f, (float)Data[Name] * 100, 0.2f, 5, 10);
                    }
                case "Drift":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 2.0f, (float)Data[Name] * 100, 0.2f, 5, 10);
                    }
                case "Auto Level":
                    {
                        float test = (float)Data[Name];
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 1.0f, (float)Data[Name] * 10, 0.1f, 5, 10);
                    }
                case "Rolling":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 2.0f, (float)Data[Name], 0.2f, 5, 10);
                    }
                case "Mouse Intensity":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 20,
                            0.0f, 10.0f, (float)Data[Name] * 1000 + 5, 1.0f, 5, 1);
                    }
                default:
                    return null;
            }
        }
    }
}
