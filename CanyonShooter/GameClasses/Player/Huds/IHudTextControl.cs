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
    public interface IHudTextControl : IHudControl
    {
        /// <summary>
        /// the text which will be displayed by this control
        /// </summary>
        string Text {get; set;}
        void Draw(SpriteBatch sb);
    }
}
