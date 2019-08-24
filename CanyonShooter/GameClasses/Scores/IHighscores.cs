//
//
//  @ Project : CanyonShooter
//  @ File Name : IHighscores.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using System.Collections.ObjectModel;

namespace CanyonShooter.GameClasses.Scores
{
    /// <summary></summary>
    public interface IHighscores
    {
        /// <summary></summary>
        void Reset(int diff);

        void RessetAll();

        /// <summary></summary>
        /// <param name="score">Which scores to add to the list.</param>
        bool AddScore(Score score, ICanyonShooterGame game, int difficulty, bool newnames);


        ReadOnlyCollection<ReadOnlyCollection<Score>> GetAllScores();

        /// <summary></summary>
        /// <returns>A sorted collection of Scores. The first Score object is the best.</returns>
        ReadOnlyCollection<Score> GetScores(int diffi);
        
        /// <summary></summary>
        bool LoadFromFile();
 
        /// <summary></summary>
        bool SaveToFile();
    }
}
