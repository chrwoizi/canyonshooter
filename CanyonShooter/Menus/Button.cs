#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#endregion

namespace CanyonShooter.Menus
{
    //Eventhändler
    public delegate void ActionEventHandler();
    //------------
    public class Button : DrawableGameComponent
    {
        #region Data Members
        private ICanyonShooterGame game;
        public object caller;
        private Texture2D Buttonground;
        private Texture2D Buttonhighlited;
        private Texture2D Buttonbar;
        //private SpriteBatch SpriteBatch;
        private Menu link;
        private Rectangle position;
        private string ButtonName;
        private string ImageName;
        private string BarName;
        private Rectangle BarPosition;
        private bool isActive;
        private bool highlightbutton;
        private bool visible = true;

        private bool useBar = false;

        private int width;
        private int height;

        // ---------- Ereignis ---------- 
        public event ActionEventHandler Buttonpressed;

        #region Properties

        public string Name
        {
            get { return ButtonName; }
            set { ButtonName = value; }
        }

        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public bool Visiblity
        {
            get { return visible; }
            set { visible = value; }
        }

        #endregion


        #endregion

        public void setPosition(Vector2 pos, int width, int height)
        {
            position = calculateRectangle(pos, height, width);
        }

        public Button(ICanyonShooterGame game, string name, Vector2 Pos, float height, float width, string ImageName, string BarName, Rectangle BarPosition, bool highlighted, object caller)
            : base(game as Game)
        {
            // TODO: Construct any child components here
            this.game = game;
            this.caller = caller;
            this.Position = calculateRectangle(Pos, (int)height, (int)width);
            this.ImageName = ImageName;
            this.ButtonName = name;
            this.BarName = BarName;
            this.BarPosition = BarPosition;
            this.isActive = false;
            this.highlightbutton = highlighted;

            this.useBar = true;

            this.width = (int)width;
            this.height = (int)height;
        }

        public Button(ICanyonShooterGame game, string name, Vector2 Pos, float height, float width, string ImageName, bool highlighted, object caller)
            : base(game as Game)
        {
            // TODO: Construct any child components here
            this.game = game;
            this.caller = caller;
            this.Position = calculateRectangle(Pos, (int)height, (int)width);
            this.ImageName = ImageName;
            this.ButtonName = name;
            this.isActive = false;
            this.highlightbutton = highlighted;

            this.width = (int)width;
            this.height = (int)height;
        }

        public Button(ICanyonShooterGame game, string name, Rectangle pos, string ImageName, bool highlighted, object caller)
            : base(game as Game)
        {
            // TODO: Construct any child components here
            this.game = game;
            this.caller = caller;
            this.Position = pos;
            this.ImageName = ImageName;
            this.ButtonName = name;
            this.isActive = false;
            this.highlightbutton = highlighted;

            this.width = (int)pos.Width;
            this.height = (int)pos.Height;
        }

        //private ICanyonShooterGame game;
        #region Get/Set-Methoden
        public string getButtonName()
        {
            return ButtonName;
        }

        public void SetMenu(Menu linkmenu)
        {
            link = linkmenu;
        }
        public Menu GetMenu()
        {
            return link;
        }

        public void SetActive()
        {
            this.isActive = true;
        }
        public void resetActive()
        {
            this.isActive = false;
        }
        public bool GetActive()
        {
            return isActive;
        }

        public Rectangle getPosition()
        {
            return position;
        }

        public void setPosition(Rectangle pos)
        {
            position = pos;
        }
        #endregion

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void Draw(GameTime gameTime, SpriteBatch loader)
        {
            if (loader != null)
            {
                if (this.isActive && this.highlightbutton)
                {
                    if (!useBar)
                    {
                        loader.Draw(Buttonhighlited, position, Color.White);
                    }
                    else
                    {
                        loader.Draw(Buttonhighlited, position, Color.White);
                            
                        loader.Draw(Buttonbar, new Rectangle(Game.GraphicsDevice.Viewport.Width - 383,
                                BarPosition.Y, BarPosition.Width, BarPosition.Height), Color.White);
                     
                    }
                }
                else
                {
                    loader.Draw(Buttonground, position, Color.White);
                }
            }
        }

        public void LoadContent(ICanyonShooterGame game)
        {
            // TODO: Load any ResourceManagementMode.Automatic content

            // SpriteBatch = new SpriteBatch(game.Graphics.Device);
            Buttonground = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + ImageName);
            if (this.highlightbutton)
            {
                Buttonhighlited = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + ImageName + "HI");

                if (useBar)
                {
                        Buttonbar = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + BarName);
                }  
            }
        }

        public void Update(GameTime gameTime, SpriteBatch loader)
        {
        }

        private Rectangle calculateRectangle(Vector2 pos, int height, int width)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }

        public void ButtonPressed()
        {
            if (!this.Buttonpressed.Target.Equals(null))
            {
                if (this.visible && this.isActive)
                {
                    this.Buttonpressed();
                }
            }
        }

        public void changeImage(string name)
        {
            this.ImageName = name;
            this.LoadContent(game);
        }
             
    }
}
