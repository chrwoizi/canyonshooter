using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Weapons
{
    public class StingerRocket : BaseWeapon
    {
        public StingerRocket(ICanyonShooterGame game, WeaponHolderType weaponHolder, WeaponManager weaponManager)
            : base(game, weaponHolder, "StingerRocket", weaponManager, "Weapon.StingerRocket")
        {

        }

        public override void OnFiring(Vector3 startPosition, Vector3 direction)
        {
            base.OnFiring(startPosition, direction);
            StingerRocketProjectile projectile = new StingerRocketProjectile(Game, startPosition, direction, this, WeaponHolderType);
            Game.World.AddObject(projectile);
        }

        #region Stinger Rocket Extra Stuff

        #endregion
    }
}
