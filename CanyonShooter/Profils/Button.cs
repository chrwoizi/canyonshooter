#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace CanyonShooter.Profils
{
    public class Button : DrawableGameComponent
    {
        #region Data Member

        private Texture2D Buttonground;
        private Texture2D Buttonhighlited;
       // private Texture2D Buttonbar;
        private Rectangle position;

        private string ButtonName;
        private string ImageName;
        private bool isActive;
        private ICanyonShooterGame game;
        private int width = 0;
        private bool res = false;
        #endregion

        #region Properties

        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        #endregion

        public string getButtonName()
        {
            return ButtonName;
        }

        public void SetActive()
        {
            this.IsActive = true; ;
        }

        public void resetActive()
        {
            this.IsActive = false;
        }
        

        public Button(ICanyonShooterGame game, string name, Rectangle Pos, string ImageName)
            : base(game as Game)
        {
            // TODO: Construct any child components here
            this.game = game;
            this.Position = Pos;
            this.ImageName = ImageName;
            this.ButtonName = name;
            this.IsActive = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Draw(GameTime gameTime, SpriteBatch loader)
        {
            if (loader != null)
            {
                if (!res)
                {
                    if (width != game.Graphics.Device.Viewport.Width)
                    {
                        width = game.Graphics.Device.Viewport.Width;

                    }
                    else
                    {
                        this.Position = new Rectangle(game.Graphics.Device.Viewport.Width - 362 + 18 +
                        (Position.X - 460), Position.Y, Position.Width, Position.Height);
                        res = true;
                    }
                }
                if (this.IsActive)
                {
                    loader.Draw(Buttonhighlited, Position, Color.White);
                }
                else
                {
                    loader.Draw(Buttonground, Position, Color.White);
                }
            }
        }

        public void LoadContent(ICanyonShooterGame game)
        {
            // TODO: Load any ResourceManagementMode.Automatic content

            // SpriteBatch = new SpriteBatch(game.Graphics.Device);
            Buttonground = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + ImageName);
            Buttonhighlited = game.Content.Load<Texture2D>("Content\\Textures\\Profils\\" + ImageName + "HI");

            this.width = game.Graphics.Device.Viewport.Width;
            /*
            try
            {
                Buttonbar = game.Content.Load<Texture2D>("Content\\Textures\\" + ImageName + "Bar");
            }
            catch
            {
                //umgehen wenn keine auswahl anzeige vorhanden
            }
             */
            
        }

        public void Update(GameTime gameTime, SpriteBatch loader)
        {
            this.Draw(gameTime, loader);
        }


    }
}
