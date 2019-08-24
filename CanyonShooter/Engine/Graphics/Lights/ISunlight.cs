using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics.Lights
{
    public interface ISunlight : IGameComponent, IUpdateable
    {
        /// <summary>
        /// Color and intensity
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Direction
        /// </summary>
        Vector3 Direction { get; set; }

        /// <summary>
        /// light casts shadows if true
        /// </summary>
        bool Shadows { get; set; }

        /// <summary>
        /// Gets the low detail shadow map.
        /// </summary>
        ShadowMap ShadowMapLow { get; }

        /// <summary>
        /// Gets the high detail shadow map.
        /// </summary>
        ShadowMap ShadowMapHigh { get; }
    }
}
