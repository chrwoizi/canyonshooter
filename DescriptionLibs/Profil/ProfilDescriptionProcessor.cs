using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Profil
{
    [ContentProcessor]
    public class ProfilDescriptionProcessor : ContentProcessor<ProfilDescription, ProfilDescription>
    {
        public override ProfilDescription Process(ProfilDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
