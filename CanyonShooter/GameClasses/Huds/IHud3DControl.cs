//
//
//  @ Project : CanyonShooter
//  @ File Name : IHudText.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.Huds
{
    /// <summary></summary>
    public interface IHud3DControl : IHudControl
    {
        /// <summary>
        /// the text which will be displayed by this control
        /// </summary>
        void Draw(SpriteBatch sb);
    }
}
