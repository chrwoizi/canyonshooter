//
//
//  @ Project : CanyonShooter
//  @ File Name : IEnemy.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


namespace CanyonShooter.GameClasses.World.Enemies
{
    /// <summary></summary>
    public interface IEnemy : IGameObject
    {
        /// <summary>
        /// used for scripting/gameplay purposes to identify the enemy
        /// </summary>
        int Identifier { get; }

        /// <summary>
        /// health points.
        /// the enemy will be dead if health==0.
        /// </summary>
        int Health { get; }

        /// <summary>
        /// inflicts damage
        /// </summary>
        /// <param name="value">The amount of damage.</param>
        void ReceiveDamage(int value);
    }
}
