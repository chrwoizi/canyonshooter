using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentProcessor]
    public class OptionDescriptionProcessor : ContentProcessor<OptionDescription, OptionDescription>
    {
        public override OptionDescription Process(OptionDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
