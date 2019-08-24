using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Profil.Properties
{
    [ContentProcessor]
    public class ProfilPropertiesDescriptionProcessor : ContentProcessor<ProfilPropertiesDescription, ProfilPropertiesDescription>
    {
        public override ProfilPropertiesDescription Process(ProfilPropertiesDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
