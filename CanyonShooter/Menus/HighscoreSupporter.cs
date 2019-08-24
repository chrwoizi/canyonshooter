using System;
using System.Collections.Generic;
using System.Text;

namespace CanyonShooter.Menus
{
    /*
     * Diese Klasse stellt einen Grundtyp für alle Caller der HighscoreList dar
     */

    public class HighscoreSupporter
    {
        protected int difficulty;

        public int Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }


        public HighscoreSupporter(int difficulty)
        {
            this.difficulty = difficulty;
        }

    }
}
