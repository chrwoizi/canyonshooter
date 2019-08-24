//
//
//  @ Project : CanyonShooter
//  @ File Name : IInput.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CanyonShooter.Engine.Input
{
    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }

    /// <summary></summary>
    public interface IInput
    {
        /// <summary>
        /// initializes the input devices and loads the settings from a file.
        /// </summary>
        bool Init();

        /// <summary>
        /// checks if a key is pressed.
        /// </summary>
        /// <param name="name">Name of the key. The names are defined in config files.</param>
        /// <returns>True if key is currently pressed.</returns>
        bool IsKeyDown(string name);

        /// <summary>
        /// checks if a key has been pressed between the previous and the current frame.
        /// </summary>
        /// <param name="name">Name of the key. The names are defined in config files.</param>
        bool HasKeyJustBeenPressed(string name);

        void HandleKeyboardInput(ref string inputText);

        /// <summary>
        /// checks if a key has been released between the previous and the current frame.
        /// </summary>
        /// <param name="name">Name of the key. The names are defined in config files.</param>
        bool HasKeyJustBeenReleased(string name);

        /// <summary>
        /// pixels of mouse movement performed between the previous and the current frame
        /// </summary>
        Vector2 MouseMovement { get; set; }

        /// <summary>
        /// Will catch all new states for keyboard, mouse and the gamepad.
        /// </summary>
        void Update();

        /// <summary>
        /// Binds the specified name to a key.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="keyToBind">The key to bind.</param>
        void Bind(string name, Keys keyToBind);

        /// <summary>
        /// Binds the specified name to a mouse button.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="buttonToBind">The button to bind.</param>
        void Bind(string name, MouseButton buttonToBind);

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool LoadFromFile(string name);

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <value>The mouse position.</value>
        Point MousePosition { get; }
    }
}

