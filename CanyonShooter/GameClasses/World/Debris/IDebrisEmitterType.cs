using CanyonShooter.Engine.Physics;

namespace CanyonShooter.GameClasses.World.Debris
{
    public interface IDebrisEmitterType : ITransformable
    {
        /// <summary>
        /// applies position and velocity
        /// </summary>
        /// <param name="obj"></param>
        void Apply(ITransformable obj);

        float MinVelocity { get; set; }
        float MaxVelocity { get; set; }
    }
}
