using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using CanyonShooter.Engine.Helper;
using CanyonShooter.GameClasses.Console;

namespace CanyonShooter.GameClasses.World.Enemies
{
    public class EnemyFormation : GameObject, IEnemy
    {
        public WaypointManager Waypoints;
        private ICanyonShooterGame game;

        private List<Vector3> offsets = new List<Vector3>();
        private Dictionary<int,int> mapping = new Dictionary<int, int>();

        public EnemyFormation(ICanyonShooterGame game, Vector3 startPos, int startSegmentId, float speed, List<Vector3> formationPositions):base(game)
        {
            ConnectedToXpa = true;
            InfluencedByGravity = false;
            LocalPosition = startPos;

            offsets = formationPositions;
            //WaypointManager
            Waypoints = new WaypointManager(game, this, startSegmentId);
            this.game = game;
            game.World.AddObject(this);
            Speed = speed;
        }

        public float Speed;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Waypoints.DirectionToWaypointCached != Vector3.Zero)
                LocalRotation = Helper.RotateTo(Waypoints.DirectionToWaypointCached, new Vector3(0, 0, -1));
            //Waypoint-Flying
            Velocity = Waypoints.DirectionToWaypoint * Speed;
   
        }

        public bool RegisterEnemy(int id)
        {
            // Füge Enemy-Slots hinzu, wenn verfügbar.
            if (offsets.Count > mapping.Count)
            {
                mapping.Add(id, mapping.Count);
                return true;
            }
            else
                return false;
        }

        public Vector3 GetPosition(int id)
        {
            if (mapping.ContainsKey(id))
                return Vector3.Transform(offsets[mapping[id]], GlobalTransformation);
            else
                return Vector3.Zero;
        }

        public Quaternion GetRotation(int id)
        {
                return GlobalRotation;
        }

        #region IEnemy Member

        public int Identifier
        {
            get { return -1; }
        }

        public int Health
        {
            get { return 0; }
        }

        public void ReceiveDamage(int value)
        {
            
        }

        #endregion
    }
}
