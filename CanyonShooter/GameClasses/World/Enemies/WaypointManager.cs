using System.Collections.Generic;
using Microsoft.Xna.Framework;
using CanyonShooter.Engine.Physics;
using System.Diagnostics;

namespace CanyonShooter.GameClasses.World.Enemies
{
    public class WaypointManager
    {
        public WaypointManager(ICanyonShooterGame game, IGameObject parent, int segment)
        {
            this.game = game;
            this.parent = parent;
            CurrentSegment = segment;
        }

        private IGameObject parent;
        private ICanyonShooterGame game;


        public IGameObject FollowObject;



        /// <summary>
        /// Calculates the next waypoint.
        /// 
        /// </summary>
        private void CalculateNextWaypoint()
        {
            if (FollowObject != null) /* follow mode */
            {
                currentWaypoint = FollowObject.GlobalPosition;
                return;
            }

            if(CurrentSegment == 0 )
                return;

            if (game.World.Level.GetDistanceToSegmentConnection(parent.GlobalPosition, CurrentSegment) < 0)
            {
                CurrentSegment--;
                currentWaypoint = game.World.Level.Cache[CurrentSegment].APos;
                CalculateNextWaypoint();
            }
        }

        private Vector3 currentWaypoint;

        public int CurrentSegment
        {
            get
            {
                return parent.CanyonSegment;
            }
            set
            {
                parent.CanyonSegment = value;
            }
        }

        public Vector3 Waypoint
        {
            get
            {
                CalculateNextWaypoint();
                return currentWaypoint;
            }
        }

        public Vector3 DirectionToWaypoint
        {
            get
            {
                direction = -parent.GlobalPosition + Waypoint;
                if (direction != Vector3.Zero)
                    direction.Normalize();
                return direction;
            }
        }

        private Vector3 direction = new Vector3(float.NaN);
        public Vector3 DirectionToWaypointCached
        {
            get
            {
                if (float.IsNaN(direction.X)) return DirectionToWaypoint;
                return direction;
            }
        }


        Queue<Vector3> Waypoints = new Queue<Vector3>();


        public void GenerateWaypoints()
        {
                //TODO Flugmuster wie Spirale usw.
        }
    }
}
