using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using CanyonShooter.Engine.AI;
using CanyonShooter.Engine.Helper;
using CanyonShooter.GameClasses.Weapons;

namespace CanyonShooter.GameClasses.World.Enemies.StatesAI2
{
    public class AI2AttackPlayer : AIState
    {
        public override string Name
        {
            get { return "ATTACK_PLAYER"; }
        }

        private WeaponManager weapons;
        public WeaponManager Weapons
        {
            get { return weapons; }
        }

        public AI2AttackPlayer(WeaponManager weaponManager, int attackSkill)
        {
            weapons = weaponManager;
            this.attackSkill = attackSkill;
            rnd = new Random();
        }

        public override void OnInit()
        {
            weapons.SwitchUp(false);
            rnd = new Random((int)Owner.GlobalPosition.Z);
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void Update()
        {
            Vector3 directionToPlayer = Player.GetDirectionToPlayer(Game, Owner.GlobalPosition, 0);
            float playerSpeed = Game.World.Players[0].Velocity.Length();

            Vector3 direction = Game.World.Level.Cache[((EnemyAI2Flo) Owner).CanyonSegment].ADir;
            if(direction != Vector3.Zero)
                direction.Normalize();
            //Backward-Flying
            Owner.Velocity = direction * (playerSpeed * 0.8f);
            if(directionToPlayer != Vector3.Zero)
                Owner.LocalRotation = Helper.RotateTo(directionToPlayer, new Vector3(0, 0, -1));

            //ShootAtPlayer(false); //TODO: FIX THIS FOR NEW WEAPON SYSTEM
        }

        private void ShootAtPlayer(bool weapon)
        {
            // Player.AimAtPlayer(,,,) is really time consuming.
            if (weapon)
            {
                if (weapons.PrimaryWeapon.CanFire)
                {
                    weapons.PrimaryWeapon.FirePause = TimeSpan.FromSeconds(rnd.Next(1, 5));
                    weapons.PrimaryWeapon.Fire(Owner.GlobalPosition, Player.AimAtPlayer(Game, Owner.GlobalPosition, weapons.PrimaryWeapon.ProjectileSpeed, 0));
                }
            }
            else
            {
                if (weapons.SecondaryWeapon.CanFire)
                {
                    weapons.SecondaryWeapon.FirePause = TimeSpan.FromSeconds(rnd.Next(1, 5));
                    weapons.SecondaryWeapon.Fire(Owner.GlobalPosition,Player.AimAtPlayer(Game, Owner.GlobalPosition,weapons.SecondaryWeapon.ProjectileSpeed, 0));
                }
            }
        }

        private Random rnd;
        private int attackSkill;

        private bool CheckSkill()
        {
            return rnd.Next(0, 100) < attackSkill;
        }
    }
}
