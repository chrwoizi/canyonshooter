// Zuständigkeit: Sascha, Malte

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

/// ATTENTION CANT USE ISCORE BECAUSE OF SERIALIZABLE!!!
namespace CanyonShooter.GameClasses.Scores
{
    /// <summary>
    /// Verwaltet die Liste der Scores.
    /// Features: Speichern, Laden, Sortieren [, Upload ins Internet]
    /// Es muss eine Funktion geben, die ein Scores-Objekt in die Liste aufnimmt.
    /// </summary>
    public class Highscores : IHighscores
    {
        #region Data Members
        public List<List<Score>> highscores;
        private const int MAX_COUNT = 20;
        private ICanyonShooterGame game;
        #endregion

        #region IHighscores Members

        public Highscores(ICanyonShooterGame game)
        {
            this.game = game;
            highscores = new List<List<Score>>();
            init();
        }

        public void Reset(int diff)
        {
            highscores[diff].Clear();
            this.SaveToFile();
        }

        public void RessetAll()
        {
            for (int i = 0; i < 3; i++)
            {
                highscores[i].Clear();
            }
            this.SaveToFile();
        }

        private void init()
        {
            for (int diff = 0; diff < 3; diff++)
            {
                highscores.Add(new List<Score>());
            }
            if (this.LoadFromFile() == false)
            {
                generateHighscores();
            }
        }

        
        public bool AddScore(Score score, ICanyonShooterGame game, int difficulty, bool newnames)
        {
            //this.LoadFromFile();
            if (highscores[difficulty].Count < MAX_COUNT)
            {
                if (newnames)
                {
                    score.Playername = game.GameStates.Profil.CurrentProfil.Playername;
                }
                highscores[difficulty].Add(score);
                SortList(difficulty);
            }
            else {
                if (score.Highscore > highscores[difficulty][highscores[difficulty].Count - 1].Highscore)
                {
                    if (newnames)
                    {
                        score.Playername = game.GameStates.Profil.CurrentProfil.Playername; ////ABÄNDERN
                    }
                    highscores[difficulty].Add(score);
                    SortList(difficulty);
                    highscores[difficulty].RemoveAt(highscores.Count - 1);
                }
                else
                {
                    return false;
                }
            }
            return this.SaveToFile();
        }

        public ReadOnlyCollection<ReadOnlyCollection<Score>> GetAllScores()
        {
            List<ReadOnlyCollection<Score>> result = new List<ReadOnlyCollection<Score>>();
            foreach (List<Score> i in highscores) 
            {
                result.Add(i.AsReadOnly());
            }
            return result.AsReadOnly();
        }
        public ReadOnlyCollection<Score> GetScores(int diffi)
        {
            return highscores[diffi].AsReadOnly();
        }

        public bool LoadFromFile()
        {
            try
            {
                // Serializer-Object erstellen, der Objecte von Type MyUserInfo behandeln hat.
                XmlSerializer serializer = new XmlSerializer(typeof(List<List<Score>>));
                // StreamReader kann Dateien auslesen.
                StreamReader reader = File.OpenText("Content\\Highscore\\Highscore.xml");
                // Klasse wird deserialisiert
                highscores = (List<List<Score>>)serializer.Deserialize(reader);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveToFile()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<List<Score>>));
                Directory.CreateDirectory("Content\\Highscore");
                Stream writer = File.Open("Content\\Highscore\\Highscore.xml", FileMode.Create);
                serializer.Serialize(writer, highscores);
                writer.Close();
                return true;
             }
             catch
             {
                return false;
             }
        }

        #endregion

        private class ScoreComparer : IComparer<Score>
        {
            #region IComparer<Score> Members

            public int Compare(Score x, Score y)
            {
                return (int) y.Highscore - x.Highscore;
            }

            #endregion
        }

        private void SortList(int diffi)
        {
            highscores[diffi].Sort(new ScoreComparer());
        }

        // Generiert einen Satz von Highscores damit Liste nicht leer ist
        private void generateHighscores()
        {
            string[] names = { "SMartin", "SSimon", "SPaul", "SGabi", "SPhhillip", "SDavid", "SSabrina", "SAnna", "SAnton", "SHarald", "SGiesela", "SJulia" };
            Random random = new Random();
            for (int diff = 0; diff < 3; diff++)
            {
                for (int i = 0; i < 10; i++)
                {
                    int points = random.Next(10, 1500);
                    Score score = new Score();
                    score.Highscore = points;
                    score.Playername = names[points % names.Length];
                    AddScore(score, game, diff, false);
                }
            }
            SaveToFile();
        }
    }
}
