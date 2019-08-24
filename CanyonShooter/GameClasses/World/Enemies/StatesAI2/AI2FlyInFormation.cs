using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.AI;
using CanyonShooter.Engine.Helper;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Enemies.StatesAI2
{
    public class AI2FlyInFormation: AIState
    {
        public override string Name
        {
            get { return "FLY_IN_FORMATION"; }
        }


        private EnemyFormation formation;

        public AI2FlyInFormation(EnemyFormation formation)
        {
            this.formation = formation;
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
            EnemyAI2Flo owner = Owner as EnemyAI2Flo;
            if(owner == null)
                return;
            if(owner.IsInFormation)
                owner.UpdateFormation(formation);
            ChangeState("PATROL");
        }
    }
}
