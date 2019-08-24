using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.AI;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Helper;
using Microsoft.Xna.Framework;
using CanyonShooter.Engine.Graphics.Models;


namespace CanyonShooter.GameClasses.World.Enemies.StatesKamidroneAI
{
    public class FlyToPlayer : AIState
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

        private bool isRed = false;
        public override void Update()
        {
            if (!isRed)
            {
                Owner.Model.Materials[0] = Material.Create(Game, "Red", InstancingType.None);
                isRed = true;
            }
            Vector3 direction = Player.GetDirectionToPlayer(Game, Owner.GlobalPosition, 0);
            if (direction != Vector3.Zero)
            {
                if (direction != Vector3.Zero)
                    Owner.LocalRotation = Helper.RotateTo(direction, new Vector3(0, 0, -1));
                //Waypoint-Flying
                Owner.Velocity = direction * (((KamidroneAI)Owner).Speed + Game.World.Players[0].Velocity.Length());
            }
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
