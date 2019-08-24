using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Credits
{
    [ContentProcessor]
    public class CreditDescriptionProcessor : ContentProcessor<CreditDescription, CreditDescription>
    {
        public override CreditDescription Process(CreditDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
