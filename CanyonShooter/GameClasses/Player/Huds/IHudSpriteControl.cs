//
//
//  @ Project : CanyonShooter
//  @ File Name : IHudText.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Huds
{
    /// <summary></summary>
    public interface IHudSpriteControl : IHudControl
    {
        /// <summary>
        /// the text which will be displayed by this control
        /// </summary>
        void Draw(SpriteBatch sb);
        Rectangle Rect { set;}

    }
}
