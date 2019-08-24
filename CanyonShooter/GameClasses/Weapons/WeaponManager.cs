using System;
using System.Collections.Generic;
using System.Diagnostics;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.Huds;
using CanyonShooter.GameClasses;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Weapons
{
    public enum WeaponHolderType
    {
        Player,
        Enemy
    }


    public enum WeaponType
    {
        NO_WEAPON,
        ROCKET_STINGER,
        ULTRA_PHASER,
        MINIGUN,
        MINIGUN2,
        PLASMAGUN
    }

    public enum AmmoType
    {
        BULLETS,
        ROCKETS,
        LASERCELLS,
        PLASMABALLS
    }

    public class WeaponManager
    {
        private WeaponHolderType weaponHolderType;

        public Dictionary<AmmoType,int> Ammo = new Dictionary<AmmoType, int>();

        public void SetAmmo(AmmoType ammoType, int value)
        {
            if (!Ammo.ContainsKey(ammoType))
                Ammo.Add(ammoType, value);
            else
                Ammo[ammoType] = value; 
        }
        public int GetAmmo(AmmoType ammoType)
        {
            if (!Ammo.ContainsKey(ammoType))
                return 0;
            return Ammo[ammoType];
        }
        public void AddAmmo(AmmoType ammoType, int value)
        {
            if(!Ammo.ContainsKey(ammoType))
                Ammo.Add(ammoType,value);
            else
                Ammo[ammoType] += value;
        }



        public void ReduceAmmo(AmmoType ammoType, int value)
        {
            if(Ammo.ContainsKey(ammoType))
            {
                if (Ammo[ammoType] >= value)
                {
                    Ammo[ammoType] -= value;
                    if (Ammo[ammoType] <= 0)
                        Ammo.Remove(ammoType);
                }
            }
        }

        public WeaponManager(ICanyonShooterGame game, IModel weaponHolder, WeaponHolderType weaponHolderType)
        {
            this.weaponHolderType = weaponHolderType;

            if(game != null)
                this.game = game;
            else 
                throw new Exception("game can't null!");
            
            weapons.Add(WeaponType.NO_WEAPON,null);
            
            if(weaponHolder != null)
                this.weaponHolder = weaponHolder;
        }

        /// <summary>
        /// WeaponHolder, including the WeaponSlots with information 
        /// about position, rotation and scale
        /// </summary>
        private IModel weaponHolder = null;


        public IModel WeaponHolder
        {
            set
            {
                weaponHolder = value;
                foreach (WeaponType type in weapons.Keys)
                {
                    AssignWeaponSlot(type);
                }
            }

            get
            {
                return weaponHolder;
            }
        }

        /// <summary>
        /// Reference to the game object
        /// </summary>
        private ICanyonShooterGame game = null;

        private IWeapon primaryWeapon;

        /// <summary>
        /// Gets the primary weapon.
        /// </summary>
        /// <value>The primary weapon.</value>
        public IWeapon PrimaryWeapon
        {
            get { return primaryWeapon; }
        }

        private IWeapon secondaryWeapon;
        /// <summary>
        /// Gets the secondary weapon.
        /// </summary>
        /// <value>The secondary weapon.</value>
        public IWeapon SecondaryWeapon
        {
            get { return secondaryWeapon; }
        }

        
        /// <summary>
        /// a dictionary holding the Weapon-Objects
        /// </summary>
        private Dictionary<WeaponType,IWeapon> weapons = new Dictionary<WeaponType, IWeapon>();

        /// <summary>
        /// Adds a weapon by its WeaponType.
        /// </summary>
        /// <param name="type">The type.</param>
        public void AddWeapon(WeaponType type)
        {
            if(game == null)
                return;
            if (game.World == null)
                throw new Exception("World needs to be initialized first!");

            if (weapons.ContainsKey(type) && type == WeaponType.MINIGUN)
                type = WeaponType.MINIGUN2;

            if(!weapons.ContainsKey(type))
            {
                switch (type)
                {
                    case WeaponType.ROCKET_STINGER:
                        weapons.Add(type, new StingerRocket(game, weaponHolderType, this));
                        break;
                    
                    case WeaponType.ULTRA_PHASER:
                        weapons.Add(type, new UltraPhaser(game, weaponHolderType, this));
                        break;

                    case WeaponType.MINIGUN:
                        weapons.Add(type, new Minigun(game, weaponHolderType, this));
                        break;

                    case WeaponType.MINIGUN2:
                        weapons.Add(type, new Minigun(game, weaponHolderType, this));
                        break;

                    case WeaponType.PLASMAGUN:
                        weapons.Add(type, new PlasmaGun(game, weaponHolderType, this));
                        break;

                    default:
                        //TODO: replace with console message ingame
                        Debug.Print("Weapon: " + type.ToString() + " not found. Check the WeaponManagers AddWeapon Method, if you are sure, that this Weapon Exists.");
                        return;
                }

                AssignWeaponSlot(type);
                game.World.AddObject(weapons[type]);
                ((BaseWeapon)weapons[type]).Visible = weaponsVisible;
            }
        }

        /// <summary>
        /// Assigns the weapon slot to the weapon defined by Weapontype type.
        /// Sets the specified position, rotation and scale
        /// </summary>
        /// <param name="type">The type.</param>
        private void AssignWeaponSlot(WeaponType type)
        {
            if(weaponHolder == null)
                return;

            // For rotating the weapons while aiming, we need a special Transformable that hold
            // the correct rotated weapon model.
            // With this, we are able to rotate the connector only.

            ITransformable weaponConnector = new Transformable(game);
            weaponConnector.Parent = weaponHolder;
            weapons[type].Parent = weaponConnector;

            if(weaponHolder.WeaponSlots == null)
                return;

            bool success = false;
            foreach (WeaponSlot slot in weaponHolder.WeaponSlots)
            {
                if (slot.WeaponType == type)
                {
                    //Set start-location and rotation of each weapon on its specified slot.
                    weapons[type].LocalPosition = slot.Position;
                    weapons[type].LocalRotation = 
                        Quaternion.CreateFromAxisAngle(slot.RotationAxis, slot.RotationAngle);
                    weapons[type].LocalScale = slot.Scaling;

                    success = true;
                    break;
                }
            }

            if(!success)
                GraphicalConsole.GetSingleton(game).WriteLine("No WeaponSlot for weapon " + type + " in weapon holder " + weaponHolder.Name,0);
        }



        private bool weaponsVisible = true;

        /// <summary>
        /// Gets or sets a value indicating whether weapons are visible.
        /// </summary>
        /// <value><c>true</c> if [weapons visible]; otherwise, <c>false</c>.</value>
        public bool WeaponsVisible
        {
            get
            {
                return weaponsVisible;
            }
            set
            {
                weaponsVisible = value;
                foreach (BaseWeapon weapon in weapons.Values)
                {
                    if(weapon != null)
                        weapon.Visible = value;
                }

            }
        }

        public void UpdatePlayerWeapons3D(GameTime gameTime,TDxInput.Keyboard keyboard)
        {
            // Waffenaiming

            if (PrimaryWeapon != null) PrimaryWeapon.AimAt();
            if (SecondaryWeapon != null) SecondaryWeapon.AimAt();

            // End Fire
            foreach (KeyValuePair<WeaponType, IWeapon> pair in weapons)
            {
                if (pair.Value != PrimaryWeapon && pair.Value != SecondaryWeapon && pair.Key != WeaponType.NO_WEAPON)
                {
                    pair.Value.EndFire();
                }
            }
            // Waffenfeuer
            if(keyboard.IsKeyDown(1))
            //if (game.Input.IsKeyDown("Player1.PrimaryFire"))
            {
                if (PrimaryWeapon != null) PrimaryWeapon.BeginFire();
            }
            else
            {
                if (PrimaryWeapon != null) PrimaryWeapon.EndFire();
            }
            if (keyboard.IsKeyDown(2))
            //if (game.Input.IsKeyDown("Player1.SecondaryFire"))
            {
                if (SecondaryWeapon != null) SecondaryWeapon.BeginFire();
            }
            else
            {
                if (SecondaryWeapon != null) SecondaryWeapon.EndFire();
            }

            // Waffenwechsel
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select1"))
            {
                SetPrimaryWeapon(WeaponType.ULTRA_PHASER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select2"))
            {
                SetPrimaryWeapon(WeaponType.MINIGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select3"))
            {
                SetPrimaryWeapon(WeaponType.ROCKET_STINGER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select4"))
            {
                SetPrimaryWeapon(WeaponType.PLASMAGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.SwitchUp"))
            {
                SwitchUp(true);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.SwitchDown"))
            {
                SwitchDown(true);
            }

            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select1"))
            {
                SetSecondaryWeapon(WeaponType.ULTRA_PHASER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select2"))
            {
                SetSecondaryWeapon(WeaponType.MINIGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select3"))
            {
                SetSecondaryWeapon(WeaponType.ROCKET_STINGER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select4"))
            {
                SetSecondaryWeapon(WeaponType.PLASMAGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.SwitchUp"))
            {
                SwitchUp(false);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.SwitchDown"))
            {
                SwitchDown(false);
            }
        }

        public void UpdatePlayerWeapons(GameTime gameTime)
        {
            // Waffenaiming

            if (PrimaryWeapon != null) PrimaryWeapon.AimAt();
            if (SecondaryWeapon != null) SecondaryWeapon.AimAt();

            // End Fire
            foreach (KeyValuePair<WeaponType, IWeapon> pair in weapons)
            {
                if (pair.Value != PrimaryWeapon && pair.Value != SecondaryWeapon && pair.Key != WeaponType.NO_WEAPON)
                {
                    pair.Value.EndFire();
                }
            }
            // Waffenfeuer
            if (game.Input.IsKeyDown("Player1.PrimaryFire"))
            {
                if (PrimaryWeapon != null) PrimaryWeapon.BeginFire();
            }
            else
            {
                if (PrimaryWeapon != null) PrimaryWeapon.EndFire();
            }

            if (game.Input.IsKeyDown("Player1.SecondaryFire"))
            {
                if (SecondaryWeapon != null) SecondaryWeapon.BeginFire();
            }
            else
            {
                if (SecondaryWeapon != null) SecondaryWeapon.EndFire();
            }

            // Waffenwechsel
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select1"))
            {
                SetPrimaryWeapon(WeaponType.ULTRA_PHASER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select2"))
            {
                SetPrimaryWeapon(WeaponType.MINIGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select3"))
            {
                SetPrimaryWeapon(WeaponType.ROCKET_STINGER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.Select4"))
            {
                SetPrimaryWeapon(WeaponType.PLASMAGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.SwitchUp"))
            {
                SwitchUp(true);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.PrimaryWeapon.SwitchDown"))
            {
                SwitchDown(true);
            }

            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select1"))
            {
                SetSecondaryWeapon(WeaponType.ULTRA_PHASER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select2"))
            {
                SetSecondaryWeapon(WeaponType.MINIGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select3"))
            {
                SetSecondaryWeapon(WeaponType.ROCKET_STINGER);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.Select4"))
            {
                SetSecondaryWeapon(WeaponType.PLASMAGUN);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.SwitchUp"))
            {
                SwitchUp(false);
            }
            if (game.Input.HasKeyJustBeenPressed("Player1.SecondaryWeapon.SwitchDown"))
            {
                SwitchDown(false);
            }
        }


        /// <summary>
        /// Sets the primary weapon.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetPrimaryWeapon(WeaponType type)
        {
            if (weapons.ContainsKey(type))
                if (secondaryWeapon != weapons[type])
                    primaryWeapon = weapons[type];
            else
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("Weapon not avaible: {0}.", type.ToString()), 0);
        }

        /// <summary>
        /// Sets the secondary weapon.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetSecondaryWeapon(WeaponType type)
        {
            if (weapons.ContainsKey(type))
                if (primaryWeapon != weapons[type])
                secondaryWeapon = weapons[type];
            else
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("Weapon not avaible: {0}.", type.ToString()), 0);
        }

        public void SwitchUp(bool primary)
        {
            //TODO
        }

        public void SwitchDown(bool primary)
        {
            //TODO
        }

        /// <summary>
        /// Gets the weapon.
        /// </summary>
        /// <param name="type">The type of weapon.</param>
        /// <returns>the weapon, or null if weapon is not equiped</returns>
        public IWeapon GetWeapon(WeaponType type)
        {
            if(weapons.ContainsKey(type))
                return weapons[type];
            return null;
            
        }

        public bool HasWeapon(WeaponType type)
        {
            return weapons.ContainsKey(type);
        }

        public WeaponType GetWeaponType(IWeapon weapon)
        {
            foreach (KeyValuePair<WeaponType, IWeapon> pair in weapons)
            {
                if (pair.Value == weapon)
                    return pair.Key;
            }
            return WeaponType.NO_WEAPON;
        }


    }

}
