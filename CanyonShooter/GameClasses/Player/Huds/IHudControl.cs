//
//
//  @ Project : CanyonShooter
//  @ File Name : IHudControl.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Huds
{
    /// <summary></summary>
    public interface IHudControl
    {
        /// <summary>
        /// true if this is visible on the parent Hud
        /// </summary>
        bool Visible { get; set; }
        string Name { get; set; }

        HUDEffectType Effect{ get; set;}
        Vector2 Position { get; set; }

        float TimeLiving { get; set; }

        Vector2 Resolution { set;}

        void Update(GameTime gameTime);
    }
}
