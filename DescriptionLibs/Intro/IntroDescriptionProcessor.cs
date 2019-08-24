using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Intro
{
    [ContentProcessor]
    public class IntroDescriptionProcessor : ContentProcessor<IntroDescription, IntroDescription>
    {
        public override IntroDescription Process(IntroDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
