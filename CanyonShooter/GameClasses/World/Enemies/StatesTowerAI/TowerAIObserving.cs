using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.AI;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Enemies.StatesTowerAI
{
    public class TowerAIObserving : AIState
    {
        public TowerAIObserving(IPlayer player)
        {
            this.player = player;    
        }

        private IPlayer player;

        public override string Name
        {
            get { return "OBSERVING"; }
        }

        private float playerDistance = 0;
        public override void Update()
        {
            playerDistance = Player.GetDistanceToPlayer(Game, Manager.Owner.GlobalPosition, 0);

            if(playerDistance < 1500f)
            {
                // Player is near, aim to him
                ((TowerAI)Manager.Owner).AimToPlayer();
                
            }
            if( playerDistance <= 1000f)
            {
                // Player is near enough, lets attack him!
                ChangeState("ATTACK_PLAYER");
            }

        }


        public override void OnInit()
        {
            Intercom.EnemWithStingerRocketsDetected("a defense Tower");
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }
    }
}
