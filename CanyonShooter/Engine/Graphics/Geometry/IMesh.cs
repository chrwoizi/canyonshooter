using CanyonShooter.Engine.Physics;
using DescriptionLibs.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public interface IMesh : ITransformable
    {
        /// <summary>
        /// Draws the model
        /// </summary>
        void Draw(GraphicsDevice device);

        /// <summary>
        /// The effect
        /// </summary>
        Effect Effect { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The animation description
        /// </summary>
        MeshAnimation Animation { get; set; }

        /// <summary>
        /// Plays the translation animation
        /// </summary>
        void PlayTranslationAnimation();


        /// <summary>
        /// Determines whether [is playing translation animation].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is playing translation animation]; otherwise, <c>false</c>.
        /// </returns>
        bool IsPlayingTranslationAnimation();

        /// <summary>
        /// Stops the translation animation
        /// </summary>
        void StopTranslationAnimation();

        /// <summary>
        /// Plays the rotation animation
        /// </summary>
        void PlayRotationAnimation();

        /// <summary>
        /// Determines whether [is playing rotation animation].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is playing rotation animation]; otherwise, <c>false</c>.
        /// </returns>
        bool IsPlayingRotationAnimation();

        /// <summary>
        /// Stops the rotation animation
        /// </summary>
        void StopRotationAnimation();

        /// <summary>
        /// Updates the animation
        /// </summary>
        void Update(GameTime gameTime);
    }
}
