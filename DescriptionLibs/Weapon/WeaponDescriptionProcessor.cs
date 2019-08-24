using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Weapon
{
    [ContentProcessor]
    public class WeaponDescriptionProcessor : ContentProcessor<WeaponDescription, WeaponDescription>
    {
        public override WeaponDescription Process(WeaponDescription input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
