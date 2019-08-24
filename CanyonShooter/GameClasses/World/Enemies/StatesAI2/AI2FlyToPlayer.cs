using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.AI;
using CanyonShooter.Engine.Helper;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Enemies.StatesAI2
{
    public class AI2FlyToPlayer : AIState
    {
        public override string Name
        {
            get {return "FLY_TO_PLAYER"; }
        }

        public override void OnInit()
        {
            
        }

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
           
        }

        public override void Update()
        {
            Vector3 direction = Player.GetDirectionToPlayer(Game, Owner.GlobalPosition, 0);
            if (direction != Vector3.Zero)
            {
                if (direction != Vector3.Zero)
                    Owner.LocalRotation = Helper.RotateTo(direction, new Vector3(0, 0, -1));
                //Waypoint-Flying
                Owner.Velocity = direction * ((EnemyAI2Flo)Owner).Speed;
            }

            if (PlayerInTargetRange())
                ChangeState("ATTACK_PLAYER");
        }

        private bool PlayerInTargetRange()
        {
            if (Player.GetDistanceToPlayer
                (Game, Owner.GlobalPosition, 0) <= 200)
                return true;
            else
                return false;
        }

    }
}
