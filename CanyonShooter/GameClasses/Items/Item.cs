// Zuständigkeit: Martin

#region Using Statements

using System;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;
using CanyonShooter.GameClasses.World.Enemies;
using CanyonShooter.GameClasses.Weapons;
using CanyonShooter.Engine.Audio;
using DescriptionLibs.Item;
using System.Collections.Generic;

#endregion

namespace CanyonShooter.GameClasses.Items
{
    /// <summary>
    /// Waffen, Upgrades und co. Liegen/Fliegen im Canyon herum. Werden von World verwaltet.
    /// </summary>
    public class Item : GameObject, IItem
    {
        int propIdentifier;

        private ItemDescription desc;
        
        private ICanyonShooterGame game;
        float TimeLiving;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="itemDescription">The item description.</param>
        public Item(ICanyonShooterGame game, string itemDescription)
            : base(game, itemDescription)
        {
            
            this.game = game;
            // Load Weapon Description
            desc = game.Content.Load<ItemDescription>("Content\\Items\\" + itemDescription);

            switch(desc.NameValue)
            {
                case "HEALTH": SetModel("item_health"); break;
                case "SHIELD": SetModel("item_shield"); break;
                default: SetModel("item_health"); break;
            }
            
            LocalScale = new Vector3(20, 20, 20);

            ConnectedToXpa = true;
            ContactGroup = ContactGroup.Items;
            InfluencedByGravity = false;


            TimeLiving = 0;

            if (game.Graphics.ShadowMappingSupported)
            {
                game.World.Sky.Sunlight.ShadowMapLow.Scene.AddDrawable(this);
                game.World.Sky.Sunlight.ShadowMapHigh.Scene.AddDrawable(this);
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        private GameTime time;
        private float hopper=0;
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            TimeLiving += (float)gameTime.ElapsedRealTime.TotalSeconds;
            time = gameTime;
            //LocalPosition = new Vector3(0, (float)(125 + 5 * Math.Pow(Math.Sin(TimeLiving/2),2)), -500);
            if (hopper < 10)
            {
                hopper++;
                LocalPosition = new Vector3(LocalPosition.X, LocalPosition.Y + 0.5f, LocalPosition.Z);
            }
            else
            {
                hopper++;
                LocalPosition = new Vector3(LocalPosition.X, LocalPosition.Y - 0.5f, LocalPosition.Z);
            }

            if (hopper >= 20)
                hopper = 0;

            base.Update(gameTime);
        }

        #region IItem Member

        public int Identifier
        {
            get { return propIdentifier; }
        }

        #endregion

        public override void OnCollision(CollisionEvent e)
        {
            base.OnCollision(e);

            IGameObject collisionObject = e.OtherSolid.UserData as IGameObject;

            if (collisionObject == null)
                return;

            switch (collisionObject.ContactGroup)
            {
                case Engine.Physics.ContactGroup.Player:
                    if (collisionObject is IPlayer)
                       GiveStuff( collisionObject as IPlayer);
                    break;

                default:
                    //InfoMessage("No Action for Collision Group: " + collisionObject.ContactGroup);
                    break;
            }
        }

        private Random rnd = new Random();
        private void GiveStuff(IPlayer player)
        {
             if(player is Player2)
                 switch(desc.ItemType.ToUpper())
                 {
                     case "WEAPON":
                         GiveWeapon(player);
                         Intercom.GiveWeapon();
                         break;

                     case "AMMO":  
                         GiveAmmo(player);
                         Intercom.GiveAmmo();
                         break;

                     case "PLAYER":
                         GivePlayerItem(player);
                         break;

                     default:
                         break;
                 }
             game.GameStates.Hud.DisplayScrollingText(desc.ItemType.ToUpper() + System.Environment.NewLine + desc.NameValue, time);
            ISound powerup = game.Sounds.CreateSound("Ammo");
            powerup.Play();
            game.World.RemoveObject(this);
            this.Dispose();
        }

        private void GivePlayerItem(IPlayer player)
        {
            switch(desc.NameValue.ToUpper())
            {
                case "HEALTH":
                    ((Player2)player).AddHealth(desc.Value);
                    Intercom.GiveHealth();
                    break;

                case "SHIELD":
                    ((Player2)player).AddShield(desc.Value);
                    Intercom.GiveShield();
                    break;

                case "EXTRALIFE":   //TODO: Implement ExtraLive!
                    game.Sounds.CreateSound("ExtraLife").Play();
                    ((Player2) player).Lifes++;
                    Intercom.GiveExtraLife();
                    break;
                case "FUEL":
                    ((Player2)player).Fuel += desc.Value;
                    Intercom.GiveFuel();
                    break;

                default:
                    break;
            }
                
        }

        private void GiveAmmo(IPlayer player)
        {
            ((Player2)player).Weapons.AddAmmo((AmmoType)(Enum.Parse(typeof(AmmoType),desc.NameValue)), desc.Value);
        }

        private void GiveWeapon(IPlayer player)
        {
            ((Player2)player).Weapons.AddWeapon((WeaponType)(Enum.Parse(typeof(WeaponType), desc.NameValue)));
        }

        /// <summary>
        /// Creates the item.
        /// The item is automatically added to the world class object management.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="itemDescription">The item description.</param>
        /// <param name="Position">The position.</param>
        /// <returns></returns>
        public static Item CreateItem(ICanyonShooterGame game, string itemDescription, Vector3 Position)
        {
            if (itemDescription == String.Empty)
                return null;

            Item item = new Item(game,itemDescription);
            item.LocalPosition = Position;
            game.World.AddObject(item);
            return item;
        }

        /// <summary>
        /// Creates the item.
        /// The item is automatically added to the world class object management.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="itemDescription">The item description.</param>
        /// <param name="atObject">At object.</param>
        /// <returns></returns>
        public static Item CreateItem(ICanyonShooterGame game, string itemDescription, IGameObject atObject)
        {
            if(itemDescription == String.Empty)
                return null;

            return CreateItem(game, itemDescription, atObject.GlobalPosition);
        }

        private static Random r = new Random();


        /// <summary>
        /// Randoms the name of the item.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="chance">The chance.</param>
        /// <returns>the itemname, or an empty string, if no item.</returns>
        public static string RandomItemName(IList<string> items, int chance)
        {
            if (items.Count != 0)
            {
                if (r.Next(0, 100) < chance)
                    return items[r.Next(0, items.Count - 1)];
                else
                    return String.Empty;
            }
            else
                return String.Empty;
        }
    }
}


