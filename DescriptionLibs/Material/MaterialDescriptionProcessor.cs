using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Material
{
    [ContentProcessor]
    public class MaterialDescriptionProcessor : ContentProcessor<MaterialDescription, MaterialDescription>
    {
        public override MaterialDescription Process(MaterialDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
