using System;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    /// <summary>
    /// This interface represents a default method, which is called on collision.
    /// </summary>
    public interface ICollisionActor : IDisposable
    {
        /// <summary>
        /// Called when this object collides with an other object.
        /// </summary>
        /// <param name="e">The collision event.</param>
        void OnCollision(CollisionEvent e);
    }
}
