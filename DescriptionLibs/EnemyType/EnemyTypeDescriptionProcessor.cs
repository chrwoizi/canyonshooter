using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.EnemyType
{
    [ContentProcessor]
    public class EnemyTypeDescriptionProcessor : ContentProcessor<EnemyTypeDescription, EnemyTypeDescription>
    {
        public override EnemyTypeDescription Process(EnemyTypeDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
