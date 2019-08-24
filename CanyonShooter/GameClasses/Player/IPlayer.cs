//
//
//  @ Project : CanyonShooter
//  @ File Name : IPlayer.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.GameClasses.Items;
using CanyonShooter.GameClasses.Scores;
using CanyonShooter.GameClasses.World;
using CanyonShooter.GameClasses.Huds;

namespace CanyonShooter.GameClasses
{
    /// <summary></summary>
    public interface IPlayer : IGameObject
    {
        /// <summary>
        /// health points.
        /// the player will be dead if health==0.
        /// </summary>
        int Health { get; set; }

        /// <summary>
        /// Set the maximal booster heat.
        /// </summary>
        int MaxBoosterHeat { get; set; }

        // relative current speed. 0: min speed, 1: max speed
        float RelativeSpeed { get; }

        /// <summary>
        /// Shield points.
        /// </summary>
        int Shield { get; set; }

        /// <summary>
        /// Is the player alive?
        /// </summary>
        int Lifes { get; set; }

        /// <summary>
        /// Current player speed.
        /// </summary>
        float Speed { get; set;}

        /// <summary>
        /// Players current fuel for boost
        /// </summary>
        float Fuel { get; set; }

        /// <summary>
        /// Maximal fuel of the starship
        /// </summary>
        float MaxFuel { get; set; }

        /// <summary>
        /// Temperature of the booster
        /// </summary>
        int BoosterHeat { get; set; }

        /// <summary>
        /// The Level remaining time in seconds.
        /// </summary>
        int RemainingTime { get; set; }

        /// <summary>
        /// Distance since start.
        /// </summary>
        double Distance { get; set; }

        /// <summary>
        /// Set the Player...
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// inflicts damage.
        /// </summary>
        /// <param name="value">The amount of damage.</param>
        void ReceiveDamage(int value);

        /// <summary>
        /// the player adds the item to his inventory. the item will not be removed from the world.
        /// </summary>
        /// <param name="item">The item to give to this.</param>
        void GiveItem(IItem item);

        /// <summary>
        /// checks if the player has the item in his inventory
        /// </summary>
        /// <param name="item">Which Item to look for.</param>
        /// <returns>True if the item is in the inventory.</returns>
        bool HasItem(IItem item);

        /// <summary>
        /// removes an item from the inventory (for scripting/story purposes)
        /// </summary>
        /// <param name="item">Which Item to remove. Doesn't need to be in the player's inventory.</param>
        void RemoveItem(IItem item);

        /// <summary>
        /// Property for position in the canyon;
        /// </summary>
        int CanyonPosition { get; }

        /// <summary>
        /// The camera following the player
        /// </summary>
        ICamera Camera { get; }
    }
}
