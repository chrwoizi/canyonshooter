using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;
using CanyonShooter.DataLayer.Descriptions;
using DescriptionLibs.Static;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter.GameClasses.World
{
    public class WaypointObject : GameObject
    {
        private ICanyonShooterGame game;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaypointObject"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="desc">The desc.</param>
        public WaypointObject(ICanyonShooterGame game, WaypointObjectDescription desc)
            : base(game,"Waypoint")
        {
            this.game = game;
            ConnectedToXpa = true;
            Static = true;
            SetModel(desc.Model);

            //game.GameStates.Profil.
            secondsToAdd = desc.TimeInSeconds;
            CanyonSegment = desc.SegmentId;
            DataLayer.Level.LevelCache c = game.World.Level.Cache[desc.SegmentId];
            //Vector3 canyonPosition = new Vector3(c.APosX, c.APosY, c.APosZ);
            //Quaternion rotation = Helper.RotateTo(game.World.Level.Cache[desc.SegmentId].ADir, -Vector3.UnitZ);
            //LocalPosition = Vector3.Transform(desc.RelativeSpawnLocation, rotation) + canyonPosition;
            ContactGroup = Engine.Physics.ContactGroup.Waypoint;
            // Get the position from current canyon segment
            LocalPosition = game.World.Level.Cache[desc.SegmentId].APos;
            Vector3 canyonDir = game.World.Level.Cache[desc.SegmentId].ADir;
            canyonDir.Normalize();

            // Rotate the player to zero
            LocalRotation = Quaternion.Identity;
            LocalRotation = Helper.RotateTo(canyonDir, new Vector3(0, 0, -100));
            //Velocity = Vector3.Zero;

            

            if (game.Graphics.ShadowMappingSupported)
            {
                game.World.Sky.Sunlight.ShadowMapLow.Scene.AddDrawable(this);
                game.World.Sky.Sunlight.ShadowMapHigh.Scene.AddDrawable(this);
            }
        }

        private int secondsToAdd = 0;

        public int SecondsToAdd
        {
            get
            {
                return secondsToAdd;
            }
            set
            {
                secondsToAdd = value;
            }
        }
    }
}
