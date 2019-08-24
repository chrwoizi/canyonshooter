using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentProcessor]
    public class MenuDescriptionProcessor : ContentProcessor<MenuDescription, MenuDescription>
    {
        public override MenuDescription Process(MenuDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
