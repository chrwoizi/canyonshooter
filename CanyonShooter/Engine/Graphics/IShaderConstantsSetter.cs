using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public interface IShaderConstantsSetter
    {
        void SetShaderConstant(EffectParameter constant);
    }
}
