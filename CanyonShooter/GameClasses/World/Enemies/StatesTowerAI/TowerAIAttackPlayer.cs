using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.AI;
using CanyonShooter.GameClasses.Weapons;

namespace CanyonShooter.GameClasses.World.Enemies.StatesTowerAI
{
    public class TowerAIAttackPlayer : AIState
    {
        public TowerAIAttackPlayer(IPlayer player)
        {
            this.player = player;    
        }
        private IPlayer player;

        public override string Name
        {
            get { return "ATTACK_PLAYER"; }
        }

        public override void OnInit()
        {

        }

        public override void OnEnter()
        {
            //TODO: Geräusch, dass ein Enemy den Player im Visier hat.
            Intercom.PlayerIsInTargetOf("a strong defense tower");

            // We start attack, aim and fire"!
            ((TowerAI)Owner).Weapons.PrimaryWeapon.AimAt(Player.GetDirectionToPlayer(Game, Owner.GlobalPosition, 0));
            ((TowerAI)Owner).Weapons.PrimaryWeapon.BeginFire();

            ((TowerAI)Owner).Weapons.SecondaryWeapon.AimAt(Player.GetDirectionToPlayer(Game, Owner.GlobalPosition, 0));
            ((TowerAI)Owner).Weapons.SecondaryWeapon.BeginFire();
        }

        public override void OnExit()
        {
            ((TowerAI)Owner).Weapons.SecondaryWeapon.EndFire();
            ((TowerAI)Owner).Weapons.PrimaryWeapon.EndFire();
        }

        public override void Update()
        {
            ((TowerAI)Owner).Weapons.PrimaryWeapon.AimAt(Player.GetDirectionToPlayer(Game, Owner.GlobalPosition, 0));


            if (((TowerAI)Owner).Weapons.SecondaryWeapon.AmmoInMagazine <= 0)
            {
                // We have no ammo left, stop firing with the secondary Weapon...
                ((TowerAI)Owner).Weapons.SecondaryWeapon.EndFire();

            }
            else
            {
                // Re-Aim at Player with Rockets
                ((TowerAI)Owner).Weapons.SecondaryWeapon.AimAt(Player.GetDirectionToPlayer(Game, Owner.GlobalPosition, 0));
            }

            if (Player.GetDistanceToPlayer(Game, Owner.GlobalPosition, 0) > 2500)
                ChangeState("OBSERVING");

        }
    }
}
