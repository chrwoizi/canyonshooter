using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.AI;
using CanyonShooter.Engine.Helper;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Enemies.StatesKamidroneAI
{
    public class FlyInFormation: AIState
    {
        public override string Name
        {
            get { return "FLY_IN_FORMATION"; }
        }


        private EnemyFormation formation;

        public FlyInFormation(EnemyFormation formation)
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
            KamidroneAI owner = Owner as KamidroneAI;
            if(owner == null)
                return;
            if(owner.IsInFormation)
                owner.UpdateFormation(formation);
            ChangeState("PATROL");
        }
    }
}
