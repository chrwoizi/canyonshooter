using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Item
{
    [ContentProcessor]
    public class ItemDescriptionProcessor : ContentProcessor<ItemDescription, ItemDescription>
    {
        public override ItemDescription Process(ItemDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
