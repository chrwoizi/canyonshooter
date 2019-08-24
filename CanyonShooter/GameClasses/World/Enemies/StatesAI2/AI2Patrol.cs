using CanyonShooter.Engine.AI;
using CanyonShooter.GameClasses;

namespace CanyonShooter.GameClasses.World.Enemies.StatesAI2
{
    public class AI2Patrol : AIState
    {
        public override string Name
        {
            get { return "PATROL"; }
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void Update()
        {
            if (PlayerNear())
                ChangeState("FLY_TO_PLAYER");
            else
                ChangeState("FLY_TO_WAYPOINT");
        }

        private bool PlayerNear()
        {
            if (Player.GetDistanceToPlayer
                (Game, Owner.GlobalPosition, 0) <= 1000)
                return true;
            else
                return false;
        }

        public override void OnInit()
        {
        }
    }
}