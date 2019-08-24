using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.AI;
using CanyonShooter.Engine.Helper;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Enemies.StatesKamidroneAI
{
    public class FlyToWaypoint: AIState
    {

        public WaypointManager Waypoints;

        public override string Name
        {
            get { return "FLY_TO_WAYPOINT"; }
        }


        private int startSegmentId = 0;
        private float speed = 0;

        public FlyToWaypoint(int segmentId)
        {
            startSegmentId = segmentId;
        }

        public override void OnInit()
        {
            //WaypointManager
            Waypoints = new WaypointManager(Game, Owner, startSegmentId);
        }


        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void Update()
        {
            if (HasState("FLY_IN_FORMATION"))
            {
                ChangeState("FLY_IN_FORMATION");
            }
            else
            {
                if (Waypoints.DirectionToWaypointCached != Vector3.Zero)
                    Owner.LocalRotation = Helper.RotateTo(Waypoints.DirectionToWaypointCached, new Vector3(0, 0, -1));
                //Waypoint-Flying
                Owner.Velocity = Waypoints.DirectionToWaypoint * ((KamidroneAI)Owner).Speed;
                ChangeState("PATROL");
            }
        }
    }
}
