using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Menus
{
    class NormalList : ButtonList
    {
        private Vector2 space;
        private Vector2 Position;
        private Vector2 currentPosition;
        private int width = 0;
        private int height = 0;
        private string menuName;

        public MenuDescription menuXml;

      
        public NormalList(ICanyonShooterGame game,Vector2 pos, string menuName, MenuDescription menuXml, bool hori)
            : base(game as ICanyonShooterGame)
        {
            this.menuXml = menuXml;
            this.menuName = menuName;
            this.Startposition = pos;
            init(menuName, hori);         
        }
        
        public NormalList(ICanyonShooterGame game, string menuName, MenuDescription menuXml, bool hori)
            : base(game as ICanyonShooterGame)
        {
            this.menuXml = menuXml;
            this.menuName = menuName;
            init(menuName, hori);
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
                              g.ButtonImage, true, this));
                        }
                        else
                        {
                            this.AddLast(new Button(game, g.ButtonName, this.currentPosition, g.height, g.width,
                           g.ButtonImage, true, this));
                        }
                        calculateDisplacement(hori, (int)g.height, (int)g.width);
                    }
                }
            }
            if (this.Count != 0)
            {
                //currentNode = this.First;
                currentNode = this.Last;
                currentNode.Value.SetActive();
            }         
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
                if (!this.First.Equals(null))
                {
                    foreach (Button i in this)
                    {
                            i.Draw(gameTime, spritebatch);
                    }
                }
            }
        }//


        public override void LoadContent()
        {
            if (!this.First.Equals(null))
            {
                foreach (Button i in this)
                {
                    i.LoadContent(game);                 
                }
            }
        }//
   }
}
