using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentProcessor]
    public class ScrollBarDescriptionProcessor : ContentProcessor<ScrollBarDescription, ScrollBarDescription>
    {
        public override ScrollBarDescription Process(ScrollBarDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
