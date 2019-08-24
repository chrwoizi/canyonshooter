//
//
//  @ Project : CanyonShooter
//  @ File Name : IHud.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Huds
{
    /// <summary></summary>
    public interface IHud : IGameComponent , IDrawable
    {
        /// <summary>
        /// The name of the hud file from this was created
        /// </summary>
        string Name { get; }

        /// <summary>
        /// returns a control (i.e. text element).
        /// used to get a reference to a control for other classes.
        /// those classes can change the control directly.
        /// for example: the player gets the text element which displays the
        /// scores. when the player gains scores, he can change the displayed
        /// scores directly in the control.
        /// </summary>
        /// <param name="name">Which Control to return.</param>
        /// <returns>The control. Will be null if the control does not exist.</returns>
        IHudControl GetControl(string name);

        void DisplaySoundTitle(string name);

        void DisplayScrollingText(string text, GameTime time);

        void DisplayCountdown(int number);

        /// <summary>
        /// checks if the control exists
        /// </summary>
        /// <param name="name">Which Control to return.</param>
        /// <returns>True if the Control exists.</returns>
        bool ExistsControl(string name);

        /// <summary>
        /// Creates a new text control
        /// </summary>
        /// <param name="name">Name of the new control.</param>
        /// <returns>True if the control was successfully created.</returns>
        bool CreateTextControl(string name, Vector2 pos);

        /// <summary>
        /// Creates a new sprite control
        /// </summary>
        /// <param name="name">Name of the new control.</param>
        /// <returns>True if the control was successfully created.</returns>
        bool CreateSpriteControl(string name, string texture, Vector2 pos, Vector2 size);
    }
}
