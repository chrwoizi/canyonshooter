#region using

using CanyonShooter.GameClasses.Scores;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace CanyonShooter.Menus
{
    class HighscoreList: ButtonList
        {
        private Vector2 space;
        private Vector2 Position;
        private Vector2 currentPosition;
        private int width = 0;
        
        public HighscoreDescription Xml;

        private HighscoreSupporter caller;


        public HighscoreList(ICanyonShooterGame game, string ListName,Vector2 startposition, HighscoreDescription Xml, bool hori, HighscoreSupporter caller)
            : base(game as ICanyonShooterGame)
        {

            this.startposition=startposition;
            this.Xml = Xml;
            this.caller = caller; 
            this.height = 0;
            init("", hori);
        }


        //ÜBERARBEITEN
        public override void init(string menuName, bool hori)
        {
            this.Clear();
            this.space = Xml.HighscorePart.Spacer;
            if (this.startposition.Equals(new Vector2()))
            {
                this.Position = new Vector2(game.Graphics.Device.Viewport.Width - 398 + (Xml.HighscorePart.Startposition.X - 398),
                      Xml.HighscorePart.Startposition.Y);
            }
            else
            {
                this.Position = this.startposition;
            }
            this.currentPosition = this.Position;
            foreach(Score i in game.Highscores.GetScores(this.caller.Difficulty))
            {
                if (this.Count == 0)
                {
                    this.AddFirst(new Button(game,i.Playername, this.currentPosition, Xml.HighscorePart.ImageHeight, Xml.HighscorePart.ImageWidth,
                      Xml.HighscorePart.ImageName, true, this));
                }
                else
                {
                    this.AddLast(new Button(game,i.Playername, this.currentPosition, Xml.HighscorePart.ImageHeight, Xml.HighscorePart.ImageWidth,
                      Xml.HighscorePart.ImageName, true, this));
                }
                calculateDisplacement(hori, (int)Xml.HighscorePart.ImageHeight, (int) Xml.HighscorePart.ImageWidth);
            }
            if (this.Count!=0)
            {
                currentNode = this.First;
                currentNode.Value.SetActive();
            } 
        }

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


        public override void Update(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (this.Count != 0 && game.Highscores.GetScores(this.caller.Difficulty).Count!=0)
            {
                foreach (Button i in this)
                {
                    i.Update(gameTime, spritebatch);
                }
            }
            else if (this.Count != 0)
            {
                this.Clear();
            }
        }//

    }
}
