// Zuständigkeit: Sascha, Malte

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CanyonShooter.GameClasses.Scores
{
    /// <summary>
    /// Speichert die Punkte im aktuellen Spiel und wendet darauf die aktuellen Multiplikatoren an.
    /// Jeder Player besitzt ein Scores-Objekt.
    /// </summary>

    [Serializable()] 
    public class Score : IScore, ISerializable
    {
        #region Data Member

        private int highscore;

        private string playername;


        //Multiplikator soll nicht serializiert werden, da unwichtig für die Liste
        [NonSerialized()]
        private int multiplikator;

        private int killedEnemy = 0;
        
        #endregion

        #region Properties

        public int Highscore
        {
            get { return highscore; }
            set { highscore = value; }
        }
        public int Multiplikator
        {
            get { return multiplikator; }
            set { multiplikator = value; }
        }

        public int KilledEnemy
        {
            get { return killedEnemy; }
            set { killedEnemy = value; }
        }

        public string Playername
        {
            get { return playername; }
            set { playername = value; }
        }

        #endregion

        public Score()
        {
            Multiplikator = 1;
        }

        #region IScore Members

        public void Reset()
        {
            //TODO: Abspeichern in einer Datei zum Einladen
            Multiplikator = 1;
            KilledEnemy = 0;
            Highscore = 0;
        }

        public void AddPoints(int value)
        {
            switch (KilledEnemy)
            {
                case 3:
                    {
                        Multiplikator = 2;
                        break;
                    }
                case 6:
                    {
                        Multiplikator = 3;
                        break;
                    }
                case 10:
                    {
                        Multiplikator = 4;
                        break;
                    }
                case 15:
                    {
                        Multiplikator = 5;
                        break;
                    }
                case 21:
                    {
                        Multiplikator = 6;
                        break;
                    }
                case 28:
                    {
                        Multiplikator = 7;
                        break;
                    }
                case 36:
                    {
                        Multiplikator = 8;
                        break;
                    }
                case 45:
                    {
                        Multiplikator = 9;
                        break;
                    }
                case 55:
                    {
                        Multiplikator = 10;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            Highscore += Multiplikator * value;
        }

        public int GetPoints()
        {
            return Highscore;
        }

        #endregion

        #region Serialization
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter= true)]   
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Playername", Playername);
            info.AddValue("Highscore", Highscore);
        }

        protected Score(SerializationInfo info, StreamingContext context)
        {
            Playername = info.GetString("Playername");
            Highscore = info.GetInt32("Highscore");
        }
        #endregion

    }
}