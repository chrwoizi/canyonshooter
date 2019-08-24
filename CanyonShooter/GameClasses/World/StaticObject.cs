using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;
using CanyonShooter.DataLayer.Descriptions;
using DescriptionLibs.Static;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter.GameClasses.World
{
    public class StaticObject : Static
    {
        private ICanyonShooterGame game;

        public StaticObject(ICanyonShooterGame game, StaticObjectDescription desc)
            : base(game, desc.Name)
        {
            this.game = game;
            StaticDescription sd = game.Content.Load<StaticDescription>("Content/Statics/" + desc.Name);
            SetModel(sd.Model);

            //TODO: Implement Position relative to canyon from XML Config (additive)
            DataLayer.Level.LevelCache c = game.World.Level.Cache[desc.SegmentId];
            ContactGroup = Engine.Physics.ContactGroup.Statics;

            CanyonSegment = desc.SegmentId;
            // Get the position from current canyon segment
            LocalPosition = game.World.Level.Cache[desc.SegmentId].APos + sd.Position;
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

    }
}
