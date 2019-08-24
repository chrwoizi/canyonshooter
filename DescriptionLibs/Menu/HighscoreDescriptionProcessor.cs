using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentProcessor]
    public class HighscoreDescriptionProcessor : ContentProcessor<HighscoreDescription, HighscoreDescription>
    {
        public override HighscoreDescription Process(HighscoreDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
