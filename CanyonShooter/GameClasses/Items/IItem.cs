//
//
//  @ Project : CanyonShooter
//  @ File Name : IItem.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using CanyonShooter.GameClasses.World;

namespace CanyonShooter.GameClasses.Items
{
    /// <summary></summary>
    public enum ItemType
    {
        ITEM_TEST,
        SHIELD,
        HEALTH,
        AMMO_LASERCELLS,
        AMMO_ROCKETS,
        AMMO_BULLETS,
        AMMO_PLASMABALLS,
    };

    /// <summary></summary>
    public interface IItem : IGameObject
    {

        /// <summary>
        /// used for scripting/gameplay purposes to identify the item
        /// </summary>
        int Identifier { get; }
    }
}
