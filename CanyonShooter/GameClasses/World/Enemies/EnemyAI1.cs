// The file is not tested or working yet, it's being commited for reference
// only. Please don't instantiate.

using System;
using System.Collections.Generic;
using CanyonShooter.DataLayer.Descriptions;
using CanyonShooter.Engine.Helper;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.GameClasses.World.Enemies
{
    public class EnemyAI1 : GameObject, IEnemy
    {
        #region Properties
        // private Formation formation;

        private Vector3 lastWaypoint;
        private Vector3 nextWaypoint;

        private Vector3 axisPoint; // The starting point of the axis
        private Vector3 axisDirection;

        private int activeFlightPattern;

        float baseSpeed;
        float speedModifier;

        List<Vector3> flightPatternShifts;

        

        #endregion 

        #region IEnemy Member

        public EnemyAI1(ICanyonShooterGame game, EnemyDescription desc)
            : base(game, "Enemy-AI1")
        {
            // hier Konstruktor
        }

        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (passedWaypoint())
            {
                nextFlightPattern();
                makeNextWaypoint(0, new Vector3(0,0,0));
            }
            makeVelocity();
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }


        public override void Dispose()
        {
            base.Dispose();
        }

        public override void OnCollision(CollisionEvent e)
        {
            base.OnCollision(e);
        }

        public int Identifier
        {
            get { return 0; }
        }

        public int Health
        {
            get { return 100; }
        }

        public void ReceiveDamage(int value)
        {
            // hmmm ja das fehlt noch..
        }

        #endregion

        #region helper methods

        private Boolean passedWaypoint()
        {
            if (Vector3.Distance(GlobalPosition,lastWaypoint) > Vector3.Distance(nextWaypoint,lastWaypoint))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void nextFlightPattern()
        {
            activeFlightPattern++;
            if (activeFlightPattern == flightPatternShifts.Count)
            {
                activeFlightPattern = 0;
            }
        }

        private float projectionRatio(Vector3 beingProjected, Vector3 projectedOn)
        {
            projectedOn.Normalize();
            float projectionLength = Vector3.Dot(beingProjected, projectedOn);
            return beingProjected.Length() / projectionLength;
        }

        private void makeVelocity()
        {
            Vector3 temp = Vector3.Subtract(nextWaypoint, GlobalPosition);
            temp.Normalize();
            float tempSpeed = speedModifier * baseSpeed;
            Velocity = Vector3.Multiply(temp, tempSpeed);
        }

        private void makeNextWaypoint(float length, Vector3 axisGap)
        {
        // length: remaining distance to next waypoint
        // axisGap: If the next segment of the basic flight path is not directly adjacent to the old one,
        //    used to place the first new way point correctly.
            Vector3 shiftWorking = flightPatternShifts[activeFlightPattern];
            if (length != 0)
            {
                shiftWorking.Z = length;
            }
            Quaternion waypointShiftRotation = Helper.RotateTo(axisDirection, new Vector3(0, 0, -1));
            shiftWorking = Vector3.Transform(shiftWorking, waypointShiftRotation);
            lastWaypoint = nextWaypoint;
            nextWaypoint = Vector3.Add(lastWaypoint, shiftWorking);
            nextWaypoint = Vector3.Add(nextWaypoint, axisGap);
        }
        
        public void setNewAxis(Vector3 newAxisPoint, Vector3 newAxisDirection, Vector3 axisGap)
        {
            float remainingLength;
            remainingLength = Vector3.Distance(lastWaypoint, nextWaypoint) - Vector3.Dot(Vector3.Subtract(GlobalPosition, lastWaypoint), axisDirection);
            axisDirection = newAxisDirection;
            axisPoint = newAxisPoint;
            makeNextWaypoint(remainingLength, axisGap);
        }

        #endregion
    }
}
