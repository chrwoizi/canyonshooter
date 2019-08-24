using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.DataLayer.Descriptions;

namespace CanyonShooter.GameClasses.World
{
    public class SunlightObject
    {
        public SunlightObject(ICanyonShooterGame game, SunlightObjectDescription desc)
        {
            game.World.Sky.Sunlight.Color = desc.Color;

        }
    }
}
