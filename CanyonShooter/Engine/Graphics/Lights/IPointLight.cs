using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics.Lights
{
    /// <summary>
    /// light source which emits light in all directions
    /// </summary>
    public interface IPointLight : ITransformable, ILight
    {
        /// <summary>
        /// Color and intensity.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float LinearAttenuation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float SquaredAttenuation { get; set; }

        /// <summary>
        /// light casts shadows if true
        /// </summary>
        bool Shadows { get; set; }

        /// <summary>
        /// Gets the shadow map.
        /// </summary>
        ShadowMap ShadowMap { get; }
    }
}
