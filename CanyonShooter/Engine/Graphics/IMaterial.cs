//
//
//  @ Project : CanyonShooter
//  @ File Name : IMaterial.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    /// <summary></summary>
    public interface IMaterial
    {
        /// <summary>
        /// textures
        /// </summary>
        Texture[] Textures { get; }

        /// <summary>
        /// Color and intensity of the emitted light.
        /// </summary>
        Color EmissiveColor { get; set; }

        /// <summary>
        /// Color and intensity of the diffuse reflected light.
        /// </summary>
        Color DiffuseColor { get; set; }

        /// <summary>
        /// Color and intensity of the specular reflected light.
        /// </summary>
        Color SpecularColor { get; set; }

        /// <summary>
        /// Shininess of the material. Exponent in Blinn-Phong light calculation. See http://de.wikipedia.org/wiki/Blinn-Beleuchtungsmodell
        /// A value of 40 makes a polished metal appearance. 
        /// A value of 0 makes SpecularColor act as EmissiveColor!! To disable specular light relection set SpecularColor to 0.
        /// </summary>
        float Shininess { get; set; }

        /// <summary>
        /// effect (aka shader)
        /// </summary>
        Effect Effect { get; }

        /// <summary>
        /// name of the material
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Setups for rendering.
        /// </summary>
        void SetupForRendering();
    }
}
