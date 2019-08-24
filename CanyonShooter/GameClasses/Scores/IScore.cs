//
//
//  @ Project : CanyonShooter
//  @ File Name : IScore.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//

namespace CanyonShooter.GameClasses.Scores
{
    /// <summary></summary>
    /// 
    public interface IScore
    {
        /// <summary></summary>
        string Playername { set; }
               
        int Highscore{ get; }
        
        int Multiplikator { get; }

        int KilledEnemy { get; set; }

        /// <summary></summary>
        void Reset();

        /// <summary></summary>
        /// <param name="value">Scores as a number.</param>
        void AddPoints(int value);

        /// <summary></summary>
        /// <returns>Scores as a number.</returns>
        int GetPoints();
    }
}
