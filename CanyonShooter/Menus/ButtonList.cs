using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Menus
{
    public abstract class ButtonList : LinkedList<Button>
    {
        public LinkedListNode<Button> currentNode; //The active Node
        public int currentNumber;
        protected bool isActive = true;
        protected int height;
        protected Vector2 startposition;
        

        protected Vector2 Startposition
        {
            get { return startposition; }
            set { startposition = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public bool Activity
        {
            get { return isActive; }
            set { isActive = value; }
        }

        protected ICanyonShooterGame game;


        public ButtonList(ICanyonShooterGame game)
            : base()
        {
            this.game = game;
        }

        public void Next()
        {
            if (isActive)
            {

                if (this.Last.Equals(currentNode))
                {
                    currentNode.Value.resetActive();
                    currentNode = this.First;
                    currentNode.Value.SetActive();
                    currentNumber = 0;
                }
                else
                {
                    currentNode.Value.resetActive();
                    currentNode = currentNode.Next;
                    currentNode.Value.SetActive(); 
                    currentNumber++;
                }
            }
        }//     Ende

        public void Previous()
        {
            if (isActive)
            {
                if (this.First.Equals(currentNode))
                {
                    currentNode.Value.resetActive();
                    currentNode = this.Last;
                    currentNode.Value.SetActive();
                    currentNumber = this.Count-1;
                }
                else
                {
                    currentNode.Value.resetActive();
                    currentNode = currentNode.Previous;
                    currentNode.Value.SetActive();
                    currentNumber--;
                }
            }
        }// Ende

        public virtual void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (this.Count != 0)
            {
                foreach (Button i in this)
                {
                    if (i.Visiblity == true)
                    {
                        i.Draw(gameTime, spritebatch);
                    }
                }
            }
        }//

        public virtual void Update(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (this.Count!=0)
            {
                foreach (Button i in this)
                {
                    i.Update(gameTime, spritebatch);
                }
            }
        }//

        public virtual void LoadContent()
        {
            if (this.Count != 0)
            {
                foreach (Button i in this)
                {
                    i.LoadContent(game);
                }
            }
        }//

        public abstract void init(string menuName, bool hori);

        public void adjustButtons(Vector2 startposition,string menuName, bool hori)
        {
            this.startposition = startposition;
            init(menuName, hori);
            LoadContent();
        }
    }
}
