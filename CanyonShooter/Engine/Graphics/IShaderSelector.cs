using System.Collections.Generic;
using CanyonShooter.Engine.Graphics.Models;
using DescriptionLibs.Material;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public class EffectTechniqueDescription
    {
        public string Effect;
        public string Technique;
    }

    public interface IShaderSelector
    {
        /// <summary>
        /// Registers an effect.
        /// </summary>
        /// <param name="name"></param>
        void RegisterEffect(string name);

        /// <summary>
        /// Creates a EffectTechniqueDescription by a description.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        EffectTechniqueDescription GetEffectTechniqueDescription(ShaderDescription description, InstancingType instancing);
        EffectTechniqueDescription GetEffectTechniqueDescription(List<string> description);

        /// <summary>
        /// Creates an effect with the described shader as current technique
        /// </summary>
        /// <param name="shaderDescription"></param>
        /// <returns></returns>
        Effect CreateEffect(ShaderDescription shaderDescription, InstancingType instancing);
        Effect CreateEffect(EffectTechniqueDescription shaderDescription);
        Effect CreateEffect(List<string> shaderDescription);

        Effect GetSuitableDepthShader(EffectTechnique e);

        void InitializeDepthShaders();
    }
}
