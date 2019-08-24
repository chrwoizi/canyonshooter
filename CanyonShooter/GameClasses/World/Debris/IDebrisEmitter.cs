using CanyonShooter.GameClasses.World.Debris;

namespace CanyonShooter.GameClasses.World
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDebrisEmitter : IGameObject
    {
        IDebrisEmitterType Type { get; set; }

        void Emit(int count);
    }
}
