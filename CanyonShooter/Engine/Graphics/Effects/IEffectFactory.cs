namespace CanyonShooter.Engine.Graphics.Effects
{
    public interface IEffectFactory
    {

        /// <summary>
        /// Creates the effect.
        /// </summary>
        /// <param name="name">The name of the ParticleEffect.</param>
        /// <returns></returns>
        IEffect CreateEffect(string name);
    }
}
