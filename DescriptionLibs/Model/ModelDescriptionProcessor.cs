using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Model
{
    [ContentProcessor]
    public class ModelDescriptionProcessor : ContentProcessor<ModelDescription, ModelDescription>
    {
        public override ModelDescription Process(ModelDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
