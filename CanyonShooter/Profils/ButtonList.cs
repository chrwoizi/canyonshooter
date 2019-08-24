using System.Collections.Generic;
using DescriptionLibs.Profil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Profils
{
    public class ButtonList : LinkedList<Button>
    {
        public LinkedListNode<Button> currentNode; //The active Node
        public int currentNumber;

        private ICanyonShooterGame game;


        public ButtonList(ICanyonShooterGame game, ProfilDescription profilXml)
            //: base()
        {
            this.game = game;
            init(this,game,profilXml);
        }

        public void Next()
        {

            if (this.Last.Equals(currentNode))
            {
                currentNode.Value.resetActive();
                currentNode = this.First;
                currentNode.Value.SetActive();
            }
            else
            {
                currentNode.Value.resetActive();
                currentNode = currentNode.Next;
                currentNode.Value.SetActive();
            }

         }//     Ende

        public void Previous()
        {
            if (this.First.Equals(currentNode))
            {
                currentNode.Value.resetActive();
                currentNode = this.Last;
                currentNode.Value.SetActive();
            }
            else
            {
                currentNode.Value.resetActive();
                currentNode = currentNode.Previous;
                currentNode.Value.SetActive();
            }
        }// Ende

    /*to implement:
     * Update-methode for the List
     * Draw and Load of the List Elements
     */

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (!this.First.Equals(null))
            {
                foreach (Button i in this)
                {
                    i.Draw(gameTime, spritebatch);
                }
            }
        }//

        public void Update(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (!this.First.Equals(null))
            {
                foreach (Button i in this)
                {
                    i.Update(gameTime, spritebatch);
                }
            }            
        }//

        public void LoadContent()
        {
            if (!this.First.Equals(null))
            {
                foreach (Button i in this)
                {
                    i.LoadContent(game);
                }
            } 
        }//

        public ButtonList init(ButtonList result, ICanyonShooterGame game, ProfilDescription profilXml)
        {
            Clear();
            foreach (ButtonDescription i in profilXml.Buttons)
            {
                if (result.Count == 0)
                {
                    result.AddFirst(new Button(game, i.ButtonName, i.ButtonRectangle,
                      i.ButtonImage));
                }
                else
                {
                    result.AddLast(new Button(game, i.ButtonName, i.ButtonRectangle,
                       i.ButtonImage));
                }
            }
            if (!result.First.Equals(null))
            {
                currentNode = result.First;
                currentNode.Value.SetActive();
            }
            return result;
        }//


    }
}
