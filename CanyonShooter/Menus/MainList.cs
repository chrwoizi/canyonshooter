using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Menus
{
    class MainList : ButtonList,IMainList
    {
        private Vector2 space;
        private Vector2 Position;
        private Vector2 currentPosition;
        private int width = 0;
        private int height = 0;
        private Texture2D Background;
        private string menuName;

        private static Vector2 button1 = new Vector2(0,0); // relativ zu Startposition
        private static Vector2 button2 = new Vector2(100, 0);// relativ zu Startposition
        private static Vector2 button3 = new Vector2(200, 0);// relativ zu Startposition

        private Button leftshift;
        private Button rightshift;

        //PROMPT für Highscore Menu
        private Prompt Resetall;

        //Für die Buttons
        public Button LeftShift
        {
            get { return this.leftshift; }
        }

        public Button RightShift
        {
            get { return this.rightshift; }
        }



        public MenuDescription menuXml;

      
        public MainList(ICanyonShooterGame game, string menuName, MenuDescription menuXml, bool hori)
            : base(game as ICanyonShooterGame)
        {
            this.menuXml = menuXml;
            init(menuName, hori);
            this.menuName = menuName;

            foreach (MenupartDescription i in menuXml.MenuParts) 
            {
                if (i.Name==menuName) {
                    Vector2 ButtonMessure = new Vector2();
                    for (int g = 0; g < i.ButtonCount; g++)
                    {
                        ButtonMessure.X += i.Buttons[g].width;
                        ButtonMessure.Y += i.Buttons[g].height;
                    }
                    this.leftshift = new Button(game, "LeftShift", new Rectangle((int)((game.Graphics.Device.Viewport.Width 
                        - 398 + (i.StartPosition.X - 398))-23-i.ShiftOffset.X), (int)i.StartPosition.Y, 23, 72), 
                        "shift_left", true, this);
                    this.rightshift = new Button(game, "RightShift", new Rectangle((int)(
                        (game.Graphics.Device.Viewport.Width - 398 + (i.StartPosition.X - 398 - 3)) 
                        + i.ShiftOffset.X + ButtonMessure.X + ((i.ButtonCount - 1) * i.Spacer.X)),
                        (int)i.StartPosition.Y, 23, 72), "shift_right", true, this);
                }
            }
        }




        public override void init(string menuName, bool hori)
        {
            this.Clear();
            foreach (MenupartDescription i in this.menuXml.MenuParts)
            {
                if (i.Name.Equals(menuName))
                {
                    this.space = i.Spacer;
                    if (this.Startposition.Equals(new Vector2()))
                    {
                        this.Position = new Vector2(game.Graphics.Device.Viewport.Width - 398 + (i.StartPosition.X - 398),
                              i.StartPosition.Y);
                    }
                    else
                    {
                        this.Position = this.Startposition;
                    }
                    this.currentPosition = this.Position;
                    foreach (ButtonDescription g in i.Buttons)
                    {
                        if (this.Count == 0)
                        {
                            this.AddFirst(new Button(game, g.ButtonName, this.currentPosition, g.height, g.width,
                              g.ButtonImage, g.BarName, g.BarPosition, true, this));
                        }
                        else
                        {
                            this.AddLast(new Button(game, g.ButtonName, this.currentPosition, g.height, g.width,
                           g.ButtonImage, g.BarName, g.BarPosition, true, this));
                        }
                        calculateDisplacement(hori, (int)g.height, (int)g.width);
                    }
                }
            }
            if (!this.First.Equals(null)) //FALSCH COUNT==0 Abfragen EQUALS WIRFT FEHLER ganz komisch
            {
                currentNode = this.First;
                currentNode.Value.SetActive();
            }
            this.addExecution();
        }


        // KANN HERAUSGENOMMEN werden :D
        private void calculateDisplacement(bool horizont, int butheight, int butwidth)
        {
            // Ermittlung der Verschiebung !!!TODO: Verschiebung nach letztem bzw vor erstem heraus bekommen !!!
            if (horizont)
            {
                this.currentPosition.X = this.currentPosition.X + this.space.X + butwidth;
                if (butheight > this.height)
                {
                    this.height = (int)butheight;
                }

            }
            else
            {
                this.currentPosition.Y = this.currentPosition.Y + this.space.Y + butheight;
                if (butwidth > this.width)
                {
                    this.width = (int)butwidth;
                }
            }
        }//ENDE

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
                if (!spritebatch.Equals(null))
                {
                    foreach (MenupartDescription g in this.menuXml.MenuParts)
                    {
                        if (g.Name.Equals(menuName))
                        {
                            spritebatch.Draw(Background, new Rectangle(game.Graphics.Device.Viewport.Width - 398,
                                g.MenuPosition.Y, g.MenuPosition.Width, g.MenuPosition.Height), Color.White);
                        }
                    }
                    if (!this.First.Equals(null))
                    {
                        foreach (Button i in this)
                        {
                            if (i.GetActive())
                            {

                                if (currentNode.Value.Equals(i))
                                {
                                    if (currentNode.Equals(this.First))
                                    {
                                        this.Last.Value.setPosition(this.Position+button1, this.Last.Value.Width, this.Last.Value.Height);
                                        this.Last.Value.Draw(gameTime, spritebatch);
                                    }
                                    else
                                    {
                                        currentNode.Previous.Value.setPosition(this.Position+button1, currentNode.Previous.Value.Width, currentNode.Previous.Value.Height);
                                        currentNode.Previous.Value.Draw(gameTime, spritebatch);
                                    }
                                    i.setPosition(this.Position+button2, i.Width, i.Height);
                                    i.Draw(gameTime, spritebatch);
                                    if (currentNode.Equals(this.Last))
                                    {
                                        this.First.Value.setPosition(this.Position+button3, this.First.Value.Width, this.First.Value.Height);
                                        this.First.Value.Draw(gameTime, spritebatch);
                                    }
                                    else
                                    {
                                        currentNode.Next.Value.setPosition(this.Position+button3, currentNode.Next.Value.Width, currentNode.Next.Value.Height);
                                        currentNode.Next.Value.Draw(gameTime, spritebatch);
                                    }
                                }
                            }
                        }
                    }
                    leftshift.Draw(gameTime, spritebatch);
                    rightshift.Draw(gameTime, spritebatch);
                }
        }//


        public override void LoadContent()
        {
            foreach (MenupartDescription i in this.menuXml.MenuParts)
            {
                if(i.Name==this.menuName)
                {
                    this.Background = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\"+i.MenuBackground);
                }
            }
            if (!this.First.Equals(null))
            {
                foreach (Button i in this)
                {
                    i.LoadContent(game);
                }
            }

            rightshift.LoadContent(game);
            leftshift.LoadContent(game);
        }//

        // Einem Button seine Aktivität zu weisen
        public void addExecution()
        {
          foreach(Button i in this)
          {
              switch (i.Name)
              {
                  case "ResetAll":
                      {
                          i.Buttonpressed+= new ActionEventHandler(ResetAll);
                          break;
                      }
                  default:
                      break;
              }
          }
      }

      #region EventHandlers
        public void ResetAll()
        {
            Resetall = new Prompt(game, "ResetAll", game.Graphics.Device.Viewport.Width,
                                game.Graphics.Device.Viewport.Height, 
                                new string[] { "Would you delete any scores?" }, false);
            Resetall.accept += new PromptEventHandler(HSReset);
            Resetall.decline +=new PromptEventHandler(nothing);
            Resetall.intiateButtons();
            this.Activity = false;
        }

        // Reset aller Spielstände - Delegate
        public void HSReset()
        {
            game.Highscores.RessetAll();
            Resetall.unloadPrompt();
            this.Activity = true;
            game.GameStates.Menu.Action = true;
        }


        //wenn ein delegate nicht zu tun hat -Delegate
        public void nothing() 
        {
            Resetall.unloadPrompt();
            this.Activity = true;
            game.GameStates.Menu.Action = true;
        }

      #endregion
  }
}
