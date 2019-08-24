using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.ParticleEffect
{
    [ContentProcessor]
    public class ParticleEffectDescriptionProcessor : ContentProcessor<ParticleEffectDescription, ParticleEffectDescription>
    {
        public override ParticleEffectDescription Process(ParticleEffectDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
