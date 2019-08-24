using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CanyonShooter.Menus
{
    public class PromptCollection : List<Prompt>
    {
        #region Singleton
       // Eine (versteckte) Klassenvariable vom Typ der eigene Klasse
       private static PromptCollection Instance;

         // Konstuktor
         // Dieser Konstruktor kann von auﬂen nicht erreicht werden.
         private PromptCollection() {} 

        // Instanziierung
        public static PromptCollection getInstance()
         {
           if (Instance == null)
           {
            Instance = new PromptCollection();
           }
           return Instance;
         }
        #endregion

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (this.Count!=0)
            {
                foreach (Prompt i in this)
                {
                    if (i.Visiblity == true)
                    {
                        i.Draw(gameTime, spritebatch);
                    }
                }
            }
        }//

        public void Update(GameTime gameTime, SpriteBatch spritebatch)
        {
            if (this.Count != 0)
            { //Probleme wenn Prompt removet wird
                foreach (Prompt i in this)
                {
                    i.Update(gameTime);
                }
            }                   
                
        }//

        public void LoadContent(ICanyonShooterGame game)
        {
            if (this.Count != 0)
            {
                foreach (Prompt i in this)
                {
                    i.LoadContent(game);
                }
            }
        }//

    }
}
