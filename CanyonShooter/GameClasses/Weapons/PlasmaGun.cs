using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Weapons
{
    public class PlasmaGun : BaseWeapon
    {
        public PlasmaGun(ICanyonShooterGame game, WeaponHolderType weaponHolder, WeaponManager weaponManager)
            : base(game, weaponHolder, "PlasmaGun", weaponManager, "Weapon.PlasmaGun")
        {

        }

        public override void OnFiring(Vector3 startPosition, Vector3 direction)
        {
            base.OnFiring(startPosition, direction);
            PlasmaGunProjectile projectile = new PlasmaGunProjectile(Game, startPosition, direction, this, WeaponHolderType);
            Game.World.AddObject(projectile);
        }

        #region Stinger Rocket Extra Stuff

        #endregion
    }
}
