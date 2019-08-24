using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Static
{
    [ContentProcessor]
    public class StaticDescriptionProcessor : ContentProcessor<StaticDescription, StaticDescription>
    {
        public override StaticDescription Process(StaticDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
